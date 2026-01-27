using System.Collections.Generic;
using System.Linq;

namespace Bai2.Models
{
    public class ProductRepository : IProductRepository
    {
        private List<Product> products = new List<Product>();

        public ProductRepository()
        {
            products.Add(new Product { Id = 1, ProductName = "ERP Solution Package", Price = 60000 });
            products.Add(new Product { Id = 2, ProductName = "Pendrive", Price = 1000 });
            products.Add(new Product { Id = 3, ProductName = "Denim-Men", Price = 1000 });
            products.Add(new Product { Id = 4, ProductName = "Denim-Women", Price = 1000 });
            products.Add(new Product { Id = 5, ProductName = "Pringles", Price = 200 });
        }

        public IEnumerable<Product> GetAll() => products;

        public Product GetById(int id) => products.FirstOrDefault(p => p.Id == id);

        public Product Add(Product p)
        {
            int newId = products.Any() ? products.Max(x => x.Id) + 1 : 1;
            p.Id = newId;
            products.Add(p);
            return p; 
        }

        public Product Update(Product p)
        {
            var item = GetById(p.Id);
            if (item != null)
            {
                item.ProductName = p.ProductName;
                item.Price = p.Price;
                return item; 
            }
            return null;
        }

        public Product Delete(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                products.Remove(item);
                return item;  
            }
            return null;
        }
    }
}