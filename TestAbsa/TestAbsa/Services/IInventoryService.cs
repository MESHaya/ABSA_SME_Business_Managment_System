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
        Task<List<StockRequest>> GetStockRequestsByEmployeeAsync(string employeeId);


        Task<bool> ReviewStockRequestAsync(int requestId, string status, string managerId, string managerName);

        // --- Purchase Order Management ---
        // Create and retrieve purchase orders
        Task<int> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder);
        Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync();
        Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id);
        Task<List<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status);
        Task<List<PurchaseOrder>> GetPendingPurchaseOrdersAsync(); // Not yet delivered

        // Update purchase order status
        Task<bool> UpdatePurchaseOrderStatusAsync(int id, string newStatus, DateTime? statusDate = null);

        // Mark as shipped
        Task<bool> MarkAsShippedAsync(int id, DateTime shippedDate);

        // Receive stock (partial or full)
        Task<bool> ReceiveStockAsync(int id, int receivedQuantity, string managerId, string managerName);

        // Report issue
        Task<bool> ReportIssueAsync(int id, string issueNotes, string managerId);

        // Cancel purchase order
        Task<bool> CancelPurchaseOrderAsync(int id, string reason);
    }
}
    

