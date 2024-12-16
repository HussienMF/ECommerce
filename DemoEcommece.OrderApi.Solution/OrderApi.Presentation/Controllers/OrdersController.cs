using eCommerce.SharedLibrary.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using OrderApi.Application.Services;
using System.Runtime.InteropServices;

namespace OrderApi.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController(IOrder orderInterface, IOrderService orderService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders()
        {
            //await Task.Delay(4000);
            var orders = await orderInterface.GetAllAsenc();
            if (!orders.Any())
                return NotFound("No order detected in the database");

            var(_, list) = OrderConversion.FromEntity(null, orders);
            return !list!.Any() ? NotFound() : Ok(list);
                
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrder(int id)
        {
            //Get signfle product from repo

            var order = await orderInterface.FindByIdAsenc(id);
            if (order is null)
                return NotFound("Product requested not found");
            //converet data from entity to DTO and return

            var (_order, _) = OrderConversion.FromEntity(order, null!);
            //return _order is not null ? Ok(_order) : NotFound($"Product not found");
            return Ok(_order);
        }

        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<OrderDTO>> GetClientOrders(int clientId)
        {
            if (clientId <= 0)
                return BadRequest("Invalid data provider");

            var orders = await orderService.GetOrdersByClientId(clientId);
            return !orders.Any() ? NotFound() : Ok(orders);
        }

        [HttpGet("details/{orderId:int}")]
        public async Task<ActionResult<OrderDTO>> GetOrderDetails(int orderId)
        {
            if (orderId <= 0)
                return BadRequest("Invalid data provider");

            var orderDetail = await orderService.GetOrderDetails(orderId);
            return orderDetail.OrderId > 0 ? Ok(orderDetail) : NotFound("No order found");
        }


        [HttpPost]
        public async Task<ActionResult<Response>> CreateOrder(OrderDTO orderDTO)
        {
            //check model if all data annotations are passed.
            if (!ModelState.IsValid)
                return BadRequest("Incomplete data submitted");

            //convert to entity
            var getEntity = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.CreateAsenc(getEntity);
            return response.FLag ? Ok(response) : BadRequest(response);

        }

        [HttpPut]
        public async Task<ActionResult<Response>> UpdateProduct(OrderDTO orderDTO)
        {
            // check model is all data annotations are passed
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            //convert to entity
            var order = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.UpdateAsenc(order);
            return response.FLag ? Ok(response) : BadRequest(response);
        }

        [HttpDelete]
        public async Task<ActionResult<Response>> DeleteProduct(OrderDTO orderDTO)
        {
            //convert to entity
            var order = OrderConversion.ToEntity(orderDTO);
            var response = await orderInterface.DeleteAsenc(order);
            return response.FLag ? Ok(response) : BadRequest(response);
        }
    }
}
