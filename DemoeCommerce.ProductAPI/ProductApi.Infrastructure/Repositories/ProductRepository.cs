using eCommerce.SharedLibrary.Interface;
using eCommerce.SharedLibrary.Logs;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using ProductApi.Application.Interfaces;
using ProductApi.Domain.Entities;
using ProductApi.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace ProductApi.Infrastructure.Repositories
{
    public class ProductRepository(ProductDbContext context) : IProduct
    {
        public async Task<Response> CreateAsenc(Product entity)
        {
            try
            {
                var getProduct = await GetByAsenc(_ => _.Name!.Equals(entity.Name));
                if (getProduct is not null && !string.IsNullOrEmpty(getProduct.Name))
                    return new Response(false, $"{entity.Name} already added");

                var currentEntity = context.Products.Add(entity).Entity;
                await context.SaveChangesAsync();

                if (currentEntity is not null && currentEntity.Id > 0)
                    return new Response(true, $"{entity.Name} added to database successfully");
                else
                    return new Response(false, "Error occured adding new product");
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                return new Response(false, "Error occured adding new product");
            }
        }

        public async Task<Response> DeleteAsenc(Product entity)
        {
            try
            {
                var product = await FindByIdAsenc(entity.Id);
                if (product is null) 
                    return new Response(false, $"{entity.Name} not found");
                context.Products.Remove(product);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is deleted successfully");

            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                return new Response(false, "Error occured adding new product");
            }
        }


        public async Task<Product> FindByIdAsenc(int id)
        {
            try
            {
                var product = await context.Products.FindAsync(id);
                return product is not null ? product : null!;


            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                throw new Exception("Error occured retriving product");
            }
        }

        public async Task<IEnumerable<Product>> GetAllAsenc()
        {
            try
            {
                var propducts = await context.Products.AsNoTracking().ToListAsync();
                return propducts is not null ? propducts : null!;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                throw new InvalidOperationException("Error occured retriving product");
            }
        }

        public async Task<Product> GetByAsenc(Expression<Func<Product, bool>> predicate)
        { 
            try
            {
                var product = await context.Products.Where(predicate).FirstOrDefaultAsync();
                return product is not null ? product : null!;
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                throw new InvalidOperationException("Error occured retriving product");
            }
        }

        public async Task<Response> UpdateAsenc(Product entity)
        {
            try
            {
                var product = await FindByIdAsenc(entity.Id);
                if (product is null)
                    return new Response(false, $"{entity.Name} does not exist");

                context.Entry(product).State = EntityState.Detached;
                context.Products.Update(entity);
                await context.SaveChangesAsync();
                return new Response(true, $"{entity.Name} is updated successfully");
            }
            catch (Exception ex)
            {
                // Log the original exeption
                LogException.LogExceptions(ex);

                //display scary-free messege to yhe client
                return new Response(false, "Error occured updating existing product");
            }
        }

        
    }
}
