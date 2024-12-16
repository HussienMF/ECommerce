using OrderApi.Domain.Entities;
using System.Diagnostics.CodeAnalysis;

namespace OrderApi.Application.DTOs.Conversions
{
    public static class OrderConversion
    {
        public static Order ToEntity(OrderDTO order) => new()
        {
            Id = order.Id,
            ClientId = order.ClientId,
            ProductId = order.ProductId,
            OrderedDate = order.OrderedDate,
            PurchaseQuatity = order.PurchaseQuantity
        };

        public static (OrderDTO?, IEnumerable<OrderDTO>?) FromEntity(Order? order, IEnumerable<Order>? orders)
        {
            //return single
            if (order is not null || orders is null)
            {
                var singleOrder = new OrderDTO(
                    order!.Id,
                    order.ClientId,
                    order.ProductId,
                    order.PurchaseQuatity,
                    order.OrderedDate);

                return(singleOrder, null);
            }

            // return list
            if (orders is not null || order is null)
            {
                var _orders = orders!.Select(o =>
                new OrderDTO(
                    o.Id,
                    o.ClientId,
                    o.ProductId,
                    o.PurchaseQuatity,
                    o.OrderedDate
                    ));

                return (null, _orders);
            }

            return(null, null);
        }
    }
}
