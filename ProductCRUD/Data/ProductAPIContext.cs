using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ProductCRUD.Model;
using System.Data;

namespace ProductCRUD.Data
{
    public class ProductAPIContext
    {
        private readonly IDbConnection _dbConnection;

        public ProductAPIContext(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        // Add
        public async Task AddProduct(Product Product)
        {
            var sql = "INSERT INTO Products (Name,Description,Price) VALUES (@Name, @Description, @Price)";
            await _dbConnection.ExecuteAsync(sql, Product);
        }

        // Read All
        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            var sql = "SELECT Id,Name,Description,Price FROM Products";
            return await _dbConnection.QueryAsync<Product>(sql);
        }

        // Get the Product By Id 
        public async Task<Product?> GetProductById(int id)
        {
            var sql = "SELECT Id,Name,Description,Price FROM Products WHERE Id = @Id";
            return await _dbConnection.QuerySingleOrDefaultAsync<Product>(sql, new { Id = id });
        }

        // Update the Product By Id 
        public async Task UpdateProduct(Product Product)
        {
            var sql = "UPDATE Products SET Name = @Name, Description = @Description, Price = @Price WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, Product);
        }

        // Delete the Product By Id 
        public async Task DeleteProduct(int id)
        {
            var sql = "DELETE FROM Products WHERE Id = @Id";
            await _dbConnection.ExecuteAsync(sql, new { Id = id });
        }
    }
}
