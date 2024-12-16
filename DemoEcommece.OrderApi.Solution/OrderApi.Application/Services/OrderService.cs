using OrderApi.Application.DTOs;
using OrderApi.Application.DTOs.Conversions;
using OrderApi.Application.Interfaces;
using Polly.Registry;
using System.Net.Http.Json;
using System.Text.Json;
namespace OrderApi.Application.Services
{
    public class OrderService(IOrder orderInterface, HttpClient httpClient,
        ResiliencePipelineProvider<string> resiliencePipeline) : IOrderService
    {
        //Get Product
        public async Task<ProductDTO> GetProduct(int productId)
        {
            // Call Product ApI using HttpClient
            //Redirect this call to the API Getway since product Api is not response to outsiders
            var getProduct = await httpClient.GetAsync($"/api/products/{productId}");
            if (!getProduct.IsSuccessStatusCode)
                return null!;
            var product = await getProduct.Content.ReadFromJsonAsync<ProductDTO>();
            return product!;
        }

        //Get User
        public async Task<AppUserDTO> GetUser(int userId)
        {
            // Call Product ApI using HttpClient
            //Redirect this call to the API Getway since product Api is not response to outsiders
            var getUser = await httpClient.GetAsync($"/api/authentication/{userId}");
            if (!getUser.IsSuccessStatusCode)
                return null!;

            var product = await getUser.Content.ReadFromJsonAsync<AppUserDTO>();
            return product!;

        }

        //Get ORDER DETAILS BY ID
        public async Task<OrderDetailsDTO> GetOrderDetails(int orderId)
        {
            //prepare Order
            var order = await orderInterface.FindByIdAsenc(orderId);  
            if (order is null || order!.Id <= 0)
                return null!;

            var retryPipeline = resiliencePipeline.GetPipeline("my-retry-pipeline");

            //Prepare Product 
            var productDTO = await retryPipeline.ExecuteAsync(async token => await GetProduct(order.ProductId));

            //Prepare Client
            var appUserDTO = await retryPipeline.ExecuteAsync(async token => await GetUser(order.ClientId));

            //Populate order Details 
            return new OrderDetailsDTO(
                order.Id,
                productDTO.Id,
                appUserDTO.Id,
                appUserDTO.Name,
                appUserDTO.Email,
                appUserDTO.Address,
                appUserDTO.TelephoneNumber,
                productDTO.Name,
                order.PurchaseQuatity,
                productDTO.Price,
                productDTO.Quantity * order.PurchaseQuatity,
                order.OrderedDate
                );

        }
        

        public async Task<IEnumerable<OrderDTO>> GetOrdersByClientId(int clientId)
        {
            var orders = await orderInterface.GetOrdersAsync(o => o.ClientId == clientId);  
            if (!orders.Any()) return null!;

            var (_, _orders) = OrderConversion.FromEntity(null, orders);
            return _orders!;
        }
    }
}
