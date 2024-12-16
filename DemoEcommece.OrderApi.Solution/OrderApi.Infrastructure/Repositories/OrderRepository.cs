using eCommerce.SharedLibrary.Interface;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using OrderApi.Application.Interfaces;
using OrderApi.Domain.Entities;
using OrderApi.Infrastructure.Data;
using System.Linq.Expressions;

namespace OrderApi.Infrastructure.Repositories
{
    public class OrderRepository(OrdreDbContext context) : IOrder
    {
        public async Task<Response> CreateAsenc(Order entity)
        {
            try
            {
                var order = context.Orders.Add(entity).Entity;
                await context.SaveChangesAsync();
                return order.Id > 0 ? new Response(true, "Order placed successfully") : 
                    new Response(false, "Error occurred while placing order");
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                return new Response(false, "Error occured While placing order");
            }
        }

        public async Task<Response> DeleteAsenc(Order entity)
        {
            try
            {
                var order = await FindByIdAsenc(entity.Id);
                if (order is null)
                    return new Response(false, "Order not found");

                context.Orders.Remove(order);
                await context.SaveChangesAsync();
                return new Response(true, "Order successfully deleted");
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                return new Response(false, "Error occured While placing order");
            }
        }

        public async Task<Order> FindByIdAsenc(int id)
        {
            try
            {
                var order = await context.Orders!.FindAsync(id);
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                throw new Exception("Error occured While placing order");
            }
        }

        public async Task<IEnumerable<Order>> GetAllAsenc()
        {
            try
            {
                var orders = await context.Orders!.AsNoTracking().ToListAsync();
                return orders is not null ? orders : null!;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                throw new Exception("Error occured While retreving orders");
            }
        }

        public async Task<Order> GetByAsenc(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var order = await context.Orders.Where(predicate).FirstOrDefaultAsync()!;
                return order is not null ? order : null!;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                throw new Exception("Error occured While placing order");
            }
        }

        public async Task<IEnumerable<Order>> GetOrdersAsync(Expression<Func<Order, bool>> predicate)
        {
            try
            {
                var orders = await context.Orders.Where(predicate).ToListAsync();
                return orders is not null ? orders : null!;
            }
            
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                throw new Exception("Error occured While placing order");
            }
        }

        public async Task<Response> UpdateAsenc(Order entity)
        {
            try
            {
                var order = await FindByIdAsenc(entity.Id);
                if (order is null)
                    return new Response(false, "Order not found");

                context.Entry(order).State = EntityState.Detached;
                context.Orders.Update(order);
                await context.SaveChangesAsync();
                return new Response(true, "Order is updated successfully");
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                return new Response(false, "Error occured While placing order");
            }
        }
    }
}
