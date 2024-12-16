using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProductApi.Application.DTOs;
using ProductApi.Application.DTOs.Conversions;
using ProductApi.Application.Interfaces;

namespace ProductApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class ProductsController(IProduct productInterface) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetProduct()
        {
            //Get all product from repo
            var products = await productInterface.GetAllAsenc();

            if (!products.Any())
                return NotFound("No products deteced in the database");
            //converet data from entity to DTO and return
            var (_, list) = ProductConversions.FromEntity(null!, products);
            return list!.Any() ? Ok(list) : NotFound("No products found");
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ProductDTO>> GetProduct(int id)
        {
            //Get signfle product from repo

            var product = await productInterface.FindByIdAsenc(id);
            if (product is null)
                return NotFound("Product requested not found");
            //converet data from entity to DTO and return

            var (_product, _) = ProductConversions.FromEntity(product, null!);
            return _product is not null ? Ok(_product) : NotFound($"Product not found {product.Name}");
        }

        [HttpPost]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<Response>> CreateProduct(ProductDTO product)
        {
            // check model is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //convert to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productInterface.CreateAsenc(getEntity);
            return response.FLag is true ? Ok(response) : BadRequest(response); 
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> UpdateProduct(ProductDTO product)
        {
            // check model is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //convert to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productInterface.UpdateAsenc(getEntity);
            return response.FLag is true ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Response>> DeleteProduct(ProductDTO product)
        {
            //convert to entity
            var getEntity = ProductConversions.ToEntity(product);
            var response = await productInterface.DeleteAsenc(getEntity);
            return response.FLag is true ? Ok(response) : BadRequest(response);
        }
    }
}
