using eCommerce.SharedLibrary.Responses;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Presentation.Controllers;

namespace UnitTest.ProductApi.Controller
{
    public class ProductControllerTest
    {
        private readonly IProduct productInterface;
        private readonly ProductsController productsController;

        public ProductControllerTest()
        {
            //set up dependecies
            productInterface = A.Fake<IProduct>();

            //Set up System under Test - SUT
            productsController = new ProductsController(productInterface);
        }

        //Get All Products
        [Fact]
        public async Task GetProducts_WhenProductsExists_ReturnOkREsponseWithProduct()
        {
            // Arrange
            var products = new List<Product>()
            {
                new(){ Id = 1, Name="Product 1", Quantity= 10, Price =100.70m },
                new(){ Id = 2, Name="Product 2", Quantity= 100, Price =1010.70m }
            };

            //set up fake response for GetALlAsync methode
            A.CallTo(() => productInterface.GetAllAsenc()).Returns(products);

            //Act
            var result = await productsController.GetProduct();

            //Assert
            var OkResult = result.Result as OkObjectResult;
            OkResult.Should().NotBeNull();
            OkResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var returnedProducts = OkResult.Value as IEnumerable<ProductDTO>;
            returnedProducts.Should().NotBeNull();
            returnedProducts.Should().HaveCount(2);
            returnedProducts!.First().id.Should().Be(1);
            returnedProducts!.Last().id.Should().Be(2);

        }

        [Fact]
        public async Task GetProducts_WhenProductsExists_ReturnNotFoundResponse()
        {
            //Arrange
            var products = new List<Product>();

            //set up fake response for GetALlAsync methode
            A.CallTo(() => productInterface.GetAllAsenc()).Returns(products);

            //Act
            var result = await productsController.GetProduct();

            //Assert
            var notFoundResult = result.Result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(StatusCodes.Status404NotFound);

            var message = notFoundResult.Value as string;
            message.Should().Be("No products deteced in the database");
        }

        [Fact]
        public async Task CreateProducts_WhenModelStateIsInvalid_ReturnBadRequest()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.30m);
            productsController.ModelState.AddModelError("Name", "Required");

            //Act
            var result = await productsController.CreateProduct(productDTO);

            //Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        }

        [Fact]
        public async Task CreateProducts_WhenCreateIsSuccessful_ReturnOkResponse()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 34, 67.30m);
            var response = new Response(true, "Created");

            //Act
            A.CallTo(() => productInterface.CreateAsenc(A<Product>.Ignored)).Returns(response);
            var result = await productsController.CreateProduct(productDTO);

            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Messege.Should().Be("Created");
            responseResult!.FLag.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProducts_WhenCreateFails_ReturnBadRequestResponse()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 89, 45.30m);
            var response = new Response(false, "Failed");

            //Act
            A.CallTo(() => productInterface.CreateAsenc(A<Product>.Ignored)).Returns(response);
            var result = await productsController.CreateProduct(productDTO);

            //Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult!.Should().NotBeNull();
            responseResult!.Messege.Should().Be("Failed");
            responseResult!.FLag.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateProduct_WhenUpdateIsSucceessful_ReturnOkResponse()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 89, 45.30m);
            var response = new Response(true, "Update");

            //Act
            A.CallTo(() => productInterface.UpdateAsenc(A<Product>.Ignored)).Returns(response);
            var result = await productsController.UpdateProduct(productDTO);

            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Messege.Should().Be("Update");
            responseResult!.FLag.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProducts_WhenUpdateFails_ReturnBadRequestResponse()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 89, 45.30m);
            var response = new Response(false, "Update Failed");

            //Act
            A.CallTo(() => productInterface.UpdateAsenc(A<Product>.Ignored)).Returns(response);
            var result = await productsController.UpdateProduct(productDTO);

            //Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult!.Should().NotBeNull();
            responseResult!.Messege.Should().Be("Update Failed");
            responseResult!.FLag.Should().BeFalse();
        }

        [Fact]
        public async Task UpdateProduct_WhenDeleteIsSucceessful_ReturnOkResponse()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 89, 45.30m);
            var response = new Response(true, "Deleted successfully");

            //Act
            A.CallTo(() => productInterface.DeleteAsenc(A<Product>.Ignored)).Returns(response);
            var result = await productsController.DeleteProduct(productDTO);

            //Assert
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

            var responseResult = okResult.Value as Response;
            responseResult!.Messege.Should().Be("Deleted successfully");
            responseResult!.FLag.Should().BeTrue();
        }

        [Fact]
        public async Task CreateProducts_WhenDeleteFails_ReturnBadRequestResponse()
        {
            //Arrange
            var productDTO = new ProductDTO(1, "Product 1", 89, 45.30m);
            var response = new Response(false, "Delete Failed");

            //Act
            A.CallTo(() => productInterface.DeleteAsenc(A<Product>.Ignored)).Returns(response);
            var result = await productsController.DeleteProduct(productDTO);

            //Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

            var responseResult = badRequestResult.Value as Response;
            responseResult!.Should().NotBeNull();
            responseResult!.Messege.Should().Be("Delete Failed");
            responseResult!.FLag.Should().BeFalse();
        }
    }
}
