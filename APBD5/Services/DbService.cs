using Microsoft.Extensions.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace APBD5.Services
{
    public class DbService : IDbService
    {
        private IConfiguration configuration;

        public DbService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public async Task<int> CreateProductWarehouse(int productId, int warehouseId, int amount, DateTime createdAt)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                await con.OpenAsync();
                com.CommandText = "INSERT INTO Product_Warehouse (IdProductWarehouse, IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt) VALUES ((SELECT MAX(IdProductWarehouse) FROM Product_Warehouse) + 1, @warehouseId, @productId, (SELECT IdOrder FROM Order WHERE IdProduct =@productId AND Amount =@amount), @amount, ((SELECT Price FROM Product WHERE IdProduct =@productId) * @amount), @createdAt)";
                com.Parameters.AddWithValue("productId", productId);
                com.Parameters.AddWithValue("amount", amount);
                com.Parameters.AddWithValue("warehouseId", warehouseId);
                com.Parameters.AddWithValue("createdAt", createdAt);
                await com.ExecuteNonQueryAsync();
                return GetProductWarehouseId(productId, warehouseId, amount);
            }
        }

        public async Task<int> CreateProductWarehouseWithProcedure(int productId, int warehouseId, int amount, DateTime createdAt)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand("AddProductToWarehouse", con);
                com.CommandType = CommandType.StoredProcedure;
                com.Parameters.AddWithValue("productId", productId);
                com.Parameters.AddWithValue("amount", amount);
                com.Parameters.AddWithValue("warehouseId", warehouseId);
                com.Parameters.AddWithValue("createdAt", createdAt);
                await con.OpenAsync();
                await com.ExecuteNonQueryAsync();
                return GetProductWarehouseId(productId, warehouseId, amount);
            }
        }

        public int GetProductWarehouseId(int productId, int warehouseId, int amount)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT IdProductWarehouse from Product_Warehouse Where IdProduct = @productId AND IdWarehouse = @warehouseId AND Amount = @amount AND Price = ((SELECT Price FROM Product WHERE IdProduct = @productId) * @amount)";
                com.Parameters.AddWithValue("productId", productId);
                com.Parameters.AddWithValue("amount", amount);
                com.Parameters.AddWithValue("warehouseId", warehouseId);
                return (int)com.ExecuteScalar();
            }
        }

        public bool IsDateCorrect(DateTime date, int productId, int amount)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT CreatedAt FROM Order WHERE IdProduct =@productId AND Amount =@amount";
                com.Parameters.AddWithValue("productId", productId);
                com.Parameters.AddWithValue("amount", amount);
                return (DateTime)com.ExecuteScalar() < date;
            }
        }

        public bool OrderCompleted(int productId, int amount)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT count(IdProductWarehouse) from Product_Warehouse Where IdOrder = (SELECT IdOrder FROM Order WHERE IdProduct =@productId AND Amount =@amount)";
                com.Parameters.AddWithValue("productId", productId);
                com.Parameters.AddWithValue("amount", amount);
                return (int)com.ExecuteScalar() == 0;
            }
        }

        public bool OrderExists(int productId, int amount)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT COUNT(idOrder) FROM Order WHERE IdProduct =@productId AND Amount =@amount";
                com.Parameters.AddWithValue("productId", productId);
                com.Parameters.AddWithValue("amount", amount);
                return (int)com.ExecuteScalar() > 0;
            }
        }

        public bool ProductExists(int productId)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT COUNT(idProduct) FROM Product WHERE idProduct =@productId";
                com.Parameters.AddWithValue("@productId", productId);
                return (int)com.ExecuteScalar() > 0;
                
            }
        }

        public void UpdateTime(int productId, int amount)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                con.Open();
                com.CommandText = "UPDATE Order SET FulfilledAt = GETDATE() WHERE IdOrder = (SELECT idOrder FROM Order WHERE IdProduct =@productId AND Amount =@amount)";
                com.Parameters.AddWithValue("productId", productId);
                com.Parameters.AddWithValue("amount", amount);
                com.ExecuteNonQuery();
            }
        }

        public bool WarehouseExists(int warehouseId)
        {
            using (SqlConnection con = new SqlConnection(configuration.GetConnectionString("ProductionDb")))
            {
                SqlCommand com = new SqlCommand();
                com.Connection = con;
                con.Open();
                com.CommandText = "SELECT COUNT(idWarehouse) FROM Warehouse WHERE idWarehouse =@warehouseId";
                com.Parameters.AddWithValue("warehouseId", warehouseId);
                return (int)com.ExecuteScalar() > 0;
            }
        }
    }
}
