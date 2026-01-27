using System.Collections.Generic;

namespace Bai2.Models
{
    public interface IProductRepository
    {
        IEnumerable<Product> GetAll();
        Product GetById(int id);
        Product Add(Product p);
        Product Update(Product p);
        Product Delete(int id);
    }
}