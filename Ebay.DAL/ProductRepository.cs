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
    public class ProductRepository : IProductRepository
    {
        private string connectionString;

        public ProductRepository(IConfiguration configuration)
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

        public void Add(Product prod)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "INSERT INTO [product].tbl_product (ProductId, ProductName,ProductDesc, Price)"
                                + " VALUES(@ProductId, @ProductName, @ProductDesc, @Price)";
                dbConnection.Open();
                dbConnection.Execute(sQuery, prod);
            }
        }

        public IEnumerable<Product> GetAll()
        {
            using (IDbConnection dbConnection = Connection)
            {
                dbConnection.Open();
                return dbConnection.Query<Product>("SELECT * FROM [product].tbl_product");
            }
        }

        public Product GetByID(Guid id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "SELECT * FROM [product].tbl_product"
                               + " WHERE ProductId = @Id";
                dbConnection.Open();
                return dbConnection.Query<Product>(sQuery, new { Id = id }).FirstOrDefault();
            }
        }

        public void Delete(Guid id)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "DELETE FROM [product].tbl_product"
                             + " WHERE ProductId = @Id";
                dbConnection.Open();
                dbConnection.Execute(sQuery, new { Id = id });
            }
        }

        public void Update(Product prod)
        {
            using (IDbConnection dbConnection = Connection)
            {
                string sQuery = "UPDATE [product].tbl_product SET ProductName = @ProductName,"
                               + " ProductDesc = @ProductDesc, Price= @Price"
                               + " WHERE ProductId = @ProductId";
                dbConnection.Open();
                dbConnection.Query(sQuery, prod);
            }
        }
    }
}
