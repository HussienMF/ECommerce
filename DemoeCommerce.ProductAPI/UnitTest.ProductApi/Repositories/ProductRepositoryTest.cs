using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using ProductApi.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace UnitTest.ProductApi.Repositories
{
    public class ProductRepositoryTest
    {
        private readonly ProductDbContext productDbContext;
        private readonly ProductRepository productRepository;

        public ProductRepositoryTest()
        {
            var options = new DbContextOptionsBuilder<ProductDbContext>()
                .UseInMemoryDatabase(databaseName: "ProductDb").Options;

            productDbContext = new ProductDbContext(options);
            productRepository = new ProductRepository(productDbContext);
        }

        //Create Product
        [Fact]
        public async Task CreateAsync_WhenProductAlreadyExist_ReturnErrorResponse()
        {
            //arrange
            var existingProduct = new Product { Name = "ExistingProduct" };
            productDbContext.Products.Add(existingProduct);
            await productDbContext.SaveChangesAsync();

            // Act
            var result = await productRepository.CreateAsenc(existingProduct);

            // Assert
            result.Should().NotBeNull();
            result.FLag.Should().BeFalse();
            result!.Messege.Should().Be("ExistingProduct already added");
        }

        [Fact]
        public async Task CreateAsync_WhenProductDoesNotExist_AddProductAndReturnsSuccessResponse()
        {
            //arrange
            var product = new Product() { Name = "New Product" };

            //Act

            var result = await productRepository.CreateAsenc(product);

            //Assert

            result.Should().NotBeNull();
            result.FLag.Should().BeTrue();
            result.Messege.Should().Be("New Product added to database successfully");
        }

        [Fact]
        public async Task DeleteAsync_WhenProductIsFound_ReturnSuccessResponse()
        {
            //Arrange
            var product = new Product() { Id = 1, Name = "Existing Product", Price = 90m, Quantity = 6 };
            productDbContext.Products.Add(product);

            //Act
            var result = await productRepository.DeleteAsenc(product);

            //Assert
            result.Should().NotBeNull();
            result.FLag.Should().BeTrue();
            result.Messege.Should().Be("Existing Product is deleted successfully");
        }

        //[Fact]
        //public async Task DeleteAsync_WhenProductIsNotFound_ReturnNotFoundResponse()
        //{
        //    //Arrange
        //    var product = new Product() { Id = 2, Name = "NonExistingProduct", Price = 90m, Quantity = 6 };

        //    //Act
        //    var result = await productRepository.DeleteAsenc(product);

        //    //Assert
        //    result.Should().NotBeNull();
        //    result.FLag.Should().BeFalse();
        //    result.Messege.Should().Be("NonExistingProduct not found");
        //}

        [Fact]
        public async Task FindByIdAsync_WhenProductIsFound_ReturnsProduct()
        {
            //Arrange
            var product = new Product { Id = 1, Name = "ExistingProduct", Price = 90m, Quantity = 6 };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();

            //Act
            var result = await productRepository.FindByIdAsenc(product.Id);

            //Assret
            result.Should().NotBeNull();
            result.Id.Should().Be(1);
            result.Name.Should().Be("ExistingProduct");
        }

        [Fact]
        public async Task FindByIdAsync_WhenProductIsNotFound_ReturnNull()
        {
            var result = await productRepository.FindByIdAsenc(8);
            result.Should().BeNull();
        }

        [Fact]
        //public async Task GetAllAsync_WhenProductsAreFound_ReturnProducts()
        //{
        //    var products = new List<Product>()
        //    {
        //        new() { Id = 23, Name = "P2" },
        //    };

        //    productDbContext.Products.AddRange(products);
        //    await productDbContext.SaveChangesAsync();

        //    var result = await productRepository.GetAllAsenc();

        //    result.Should().NotBeNull();
        //    result.Count().Should().Be(1);
        //    result.Should().Contain(p => p.Name == "P2");
        //}

        [Fact]
        public async Task GetAllAsync_WhenProductsAreNotFound_ReturnNull()
        {
            var result = await productRepository.GetAllAsenc();

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task GetByAsync_WhenProductIsFound_ReturnProduct()
        {
            var product = new Product() { Id = 3, Name = "P1" };
            productDbContext.Products.Add(product);
            await productDbContext.SaveChangesAsync();
            Expression<Func<Product, bool>> predicate = p => p.Name == "P1";

            var result = await productRepository.GetByAsenc(predicate);

            result.Should().NotBeNull();
            result.Name.Should().Be("P1");
        }

        //[Fact]
        //public async Task GetByAsync_WhenProductIsNotFound_ReturnNull()
        //{
        //    Expression<Func<Product, bool>> predicate = p => p.Name == "P23";

        //    var result = await productRepository.GetByAsenc(predicate);

        //    result.Should().NotBeNull();
        //    result.Name.Should().Be("P3");
        //}


    }
}
