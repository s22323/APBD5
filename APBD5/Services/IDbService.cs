namespace APBD5.Services
{
    public interface IDbService
    {
        bool ProductExists(int productId);
        bool WarehouseExists(int warehouseId);
        bool OrderExists(int productId, int amount);
        bool OrderCompleted(int productId, int amount);
        bool IsDateCorrect(DateTime date, int productId, int amount);
        public void UpdateTime(int productId, int amount);
        public Task<int> CreateProductWarehouse(int productId, int warehouseId, int amount, DateTime createdAt);
        public Task<int> CreateProductWarehouseWithProcedure(int productId, int warehouseId, int amount, DateTime createdAt);
        public int GetProductWarehouseId(int productId, int warehouseId, int amount);
    }
}
