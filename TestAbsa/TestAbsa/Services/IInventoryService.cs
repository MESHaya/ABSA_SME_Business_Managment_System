using System.Collections.Generic;
using System.Threading.Tasks;
using TestAbsa.Data.Models; // Ensure this is the correct namespace for your models

namespace TestAbsa.Services
{
    // Interface for Dependency Injection
    public interface IInventoryService
    {
        // Product Management
        Task<List<Product>> GetAllProductsAsync(bool includeSuppliers = true);
        Task AddProductAsync(Product product);
        Task<Product?> GetProductByIdAsync(int id);
        Task UpdateProductAsync(Product product);

        // Supplier Management
        Task<List<Supplier>> GetAllSuppliersAsync();
        Task AddSupplierAsync(Supplier supplier);

        // Stock Request Management
        Task AddStockRequestAsync(StockRequest request);
        Task<List<StockRequest>> GetPendingStockRequestsAsync();
        // Updated signature to match the business logic
        Task<List<StockRequest>> GetAllStockRequestsAsync(); 
        Task<bool> ReviewStockRequestAsync(int requestId, string status, string managerId, string managerName);
    }
}
