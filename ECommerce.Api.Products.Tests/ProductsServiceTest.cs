using ECommerce.Api.Products.Providers;
using System;
using Xunit;
using Microsoft.EntityFrameworkCore;
using ECommerce.Api.Products.Db;
using ECommerce.Api.Products.Profiles;
using AutoMapper;
using System.Threading.Tasks;
using System.Linq;

namespace ECommerce.Api.Products.Tests
{
    public class ProductsServiceTest
    {
        [Fact]
        public async Task GetProductsRerturnAllProducts()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>().UseInMemoryDatabase(nameof(GetProductsRerturnAllProducts)).Options;
            var dbContext = new ProductsDbContext(options);
            CreateData(dbContext);

            var productsProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productsProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);
            var products = await productsProvider.GetProductsAsync();

            Assert.True(products.IsSuccess);
            Assert.True(products.Products.Any());
            Assert.Null(products.ErrorMessage);
        }

        [Fact]
        public async Task GetProductReturnsProductsUsingValidId()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>().UseInMemoryDatabase(nameof(GetProductReturnsProductsUsingValidId)).Options;
            var dbContext = new ProductsDbContext(options);
            CreateData(dbContext);

            var productsProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productsProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);
            var products = await productsProvider.GetProductAsync(1);

            Assert.True(products.IsSuccess);
            Assert.NotNull(products.Product);
            Assert.True(products.Product.Id == 1);
            Assert.Null(products.ErrorMessage);
        }

        [Fact]
        public async Task GetProductReturnsProductsUsingInvalidId()
        {
            var options = new DbContextOptionsBuilder<ProductsDbContext>().UseInMemoryDatabase(nameof(GetProductReturnsProductsUsingInvalidId)).Options;
            var dbContext = new ProductsDbContext(options);
            CreateData(dbContext);

            var productsProfile = new ProductProfile();
            var configuration = new MapperConfiguration(cfg => cfg.AddProfile(productsProfile));
            var mapper = new Mapper(configuration);

            var productsProvider = new ProductsProvider(dbContext, null, mapper);
            var products = await productsProvider.GetProductAsync(-1);

            Assert.False(products.IsSuccess);
            Assert.Null(products.Product);
            Assert.NotNull(products.ErrorMessage);
        }

        private void CreateData(ProductsDbContext dbContext)
        {
            for (int i = 1; i <= 10; i++)
            {
                dbContext.Products.Add(new Product
                {
                    Id = i,
                    Name = Guid.NewGuid().ToString(),
                    Inventory = i + 10,
                    Price = (decimal)(i * 3.14)
                });
            }
            dbContext.SaveChanges();
        }
    }
}
