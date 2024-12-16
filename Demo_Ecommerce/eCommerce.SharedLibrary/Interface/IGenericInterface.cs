using eCommerce.SharedLibrary.Responses;
using System.Linq.Expressions;

namespace eCommerce.SharedLibrary.Interface
{
    public interface IGenericInterface<T> where T : class
    {
        Task<Response> CreateAsenc(T entity);
        Task<Response> UpdateAsenc(T entity);
        Task<Response> DeleteAsenc(T entity);
        Task<IEnumerable<T>> GetAllAsenc(); 
        Task<T> FindByIdAsenc(int id); 
        Task<T> GetByAsenc(Expression<Func<T, bool>> predicate);


    }
}
