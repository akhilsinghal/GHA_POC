using System;
using System.Collections.Generic;
using System.Data;
using Ebay.Entities;

namespace Ebay.DAL
{
    public interface IProductRepository
    {
        IDbConnection Connection { get; }

        void Add(Product prod);
        void Delete(Guid id);
        IEnumerable<Product> GetAll();
        Product GetByID(Guid id);
        void Update(Product prod);
    }
}