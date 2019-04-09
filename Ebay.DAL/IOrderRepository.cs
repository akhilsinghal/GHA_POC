using System;
using System.Collections.Generic;
using System.Data;
using Ebay.Entities;

namespace Ebay.DAL
{
    public interface IOrderRepository
    {
        IDbConnection Connection { get; }

        void Add(Order order);
        void Delete(Guid id);
        IEnumerable<Order> GetAll();
        Order GetByID(Guid id);
        void Update(Order order);
    }
}