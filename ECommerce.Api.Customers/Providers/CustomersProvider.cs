using AutoMapper;
using ECommerce.Api.Customers.Db;
using ECommerce.Api.Customers.Interfaces;
using ECommerce.Api.Customers.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Customers.Providers
{
    public class CustomersProvider : ICustomersProvider
    {
        private readonly CustomersDbContext dbContext;
        private readonly ILogger<CustomersProvider> logger;
        private readonly IMapper mapper;

        public CustomersProvider(CustomersDbContext dbContext, ILogger<CustomersProvider> logger, IMapper mapper)
        {
            this.dbContext = dbContext;
            this.logger = logger;
            this.mapper = mapper;

            SeedData();
        }

        private void SeedData()
        {
            if(!dbContext.Customers.Any())
            {
                dbContext.Customers.Add(new Db.Customer { Id=1, Name="Martin", Address="Isabel Fernandez 1719" });
                dbContext.Customers.Add(new Db.Customer { Id = 2, Name = "Carolina", Address = "Anchordoqui 898" });
                dbContext.Customers.Add(new Db.Customer { Id = 3, Name = "Santi", Address = "Isabel Fernandez 1719" });
                dbContext.Customers.Add(new Db.Customer { Id = 4, Name = "Bruma", Address = "Espora 4675" });
                dbContext.SaveChanges();
            }
        }

        public async Task<(bool IsSuccess, Models.Customer Customer, string ErrorMessage)> GetCustomerAsync(int id)
        {
            try
            {
                logger?.LogInformation("Querying Customers");

                var customer = await dbContext.Customers.FirstOrDefaultAsync(p => p.Id == id);

                if (customer != null)
                {
                    logger?.LogInformation("Customer Found");
                    var result = mapper.Map<Db.Customer, Models.Customer>(customer);
                    return (true, result, null);
                }
                return (false, null, "Not found.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }

        public async Task<(bool IsSuccess, IEnumerable<Models.Customer> Customers, string ErrorMessage)> GetCustomersAsync()
        {
            try
            {
                logger?.LogInformation("Querying Customers");

                var customers = await dbContext.Customers.ToListAsync();

                if (customers != null && customers.Any())
                {
                    logger?.LogInformation($"{customers.Count} customer(s) found.");
                    var result = mapper.Map<IEnumerable<Db.Customer>, IEnumerable<Models.Customer>>(customers);
                    return (true, result, null);
                }
                return (false, null, "Not found.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex.ToString());
                return (false, null, ex.Message);
            }
        }
    }
}
