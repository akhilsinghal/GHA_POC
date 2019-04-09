using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Ebay.Entities;
using System.Linq;
using Dapper;
using Ebay.Utilities.Configuration.Model;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;

namespace Ebay.DAL
{
    public class OrderRepository : IOrderRepository
    {
        private string connectionString;

        private string baseQuery = "SELECT a.OrderId,a.CustomerId,a.CreationDate,c.ProductId,c.ProductName,c.ProductDesc,c.Price FROM tbl_order a join tbl_orderdetail b on a.OrderId = b.OrderId join tbl_product c on b.ProductId = c.ProductId";
        public OrderRepository(IConfiguration configuration)
        {
            this.connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(connectionString);
            }
        }

        public void Add(Order order)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string orderDetailsQuery = "INSERT INTO tbl_orderdetail VALUES (@Id, @OrderId,@ProductId)";
                string orderQuery = "INSERT INTO tbl_order VALUES (@OrderId,@CustomerId,@CreationDate)";

                IDbTransaction transaction = dbConnection.BeginTransaction();
                try
                {
                    dbConnection.Execute(orderQuery, new { OrderId = order.OrderId, CustomerId = order.CustomerId, CreationDate = DateTime.Now }, transaction);
                    dbConnection.Execute(orderDetailsQuery, order.Products, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }

        public IEnumerable<Order> GetAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                var lookup = new Dictionary<Guid, Order>();

                var query = dbConnection.Query<Order, Product, Order>(
                    baseQuery
                    , (o, p) =>
                    {
                        Order order;
                        if (!lookup.TryGetValue(o.OrderId, out order))
                            lookup.Add(o.OrderId, order = o);
                        if (order.Products == null)
                            order.Products = new List<Product>();
                        order.Products.Add(p);
                        return order;
                    }).AsQueryable();

                return query.ToList();
            }
        }

        public Order GetByID(Guid id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = baseQuery + " WHERE a.OrderId = @Id";
                dbConnection.Open();
                var lookup = new Dictionary<Guid, Order>();

                return dbConnection.Query<Order, Product, Order>(sQuery, (o, p) =>
                {
                    Order order;
                    if (!lookup.TryGetValue(o.OrderId, out order))
                        lookup.Add(o.OrderId, order = o);
                    if (order.Products == null)
                        order.Products = new List<Product>();
                    order.Products.Add(p);
                    return order;
                }).FirstOrDefault();
            }
        }

        public void Delete(Guid id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                string orderDetailsQuery = "DELETE FROM tbl_orderdetail WHERE OrderId = @id";
                string orderQuery = "DELETE FROM tbl_order WHERE OrderId = @id";

                IDbTransaction transaction = dbConnection.BeginTransaction();
                try
                {
                    dbConnection.Execute(orderDetailsQuery, new { OrderId = id }, transaction);
                    dbConnection.Execute(orderQuery, new { OrderId = id }, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }

        }

        public void Update(Order order)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string deleteorderDetailsQuery = "DELETE FROM tbl_orderdetail WHERE OrderId = @id";
                string orderDetailsQuery = "INSERT INTO tbl_orderdetail VALUES (@Id, @OrderId,@ProductId)";                

                IDbTransaction transaction = dbConnection.BeginTransaction();
                try
                {
                    dbConnection.Execute(deleteorderDetailsQuery, new { OrderId = order.OrderId }, transaction);
                    dbConnection.Execute(orderDetailsQuery, order.Products, transaction);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
