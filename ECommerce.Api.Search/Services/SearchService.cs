using ECommerce.Api.Search.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ECommerce.Api.Search.Services
{
    public class SearchService : ISearchService
    {
        private readonly IOrdersService ordersService;
        private readonly IProductsService productsService;
        private readonly ICustomerService customerService;

        public SearchService(IOrdersService ordersService, IProductsService productsService, ICustomerService customerService)
        {
            this.ordersService = ordersService;
            this.productsService = productsService;
            this.customerService = customerService;
        }

        public async Task<(bool IsSuccess, dynamic SearchResults)> SearchAsync(int customerId)
        {
            var ordersResult = await ordersService.GetOrdersAsync(customerId);
            var productsResult = await productsService.GetProductsAsync();
            var customerResult = await customerService.GetCustomerAsync(customerId);

            if (ordersResult.IsSuccess)
            {
                foreach (var order in ordersResult.Orders)
                {
                    foreach (var item in order.Items)
                    {
                        item.ProductName = productsResult.IsSuccess ? productsResult.Products.FirstOrDefault(p => p.Id == item.ProductId)?.Name : "Product Name is not available";
                    }
                }
                return (true, new { Orders = ordersResult.Orders, CustomerInfo = customerResult.IsSuccess ? customerResult.Customer : new Models.Customer { Name = "Customer Information not available" }});
            }
            return (false, null);
        }
    }
}
