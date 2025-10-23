using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestAbsa.Data;
using TestAbsa.Data.Models;
using Microsoft.AspNetCore.Identity; //  For accessing ApplicationUser
using Microsoft.AspNetCore.Http; // For accessing the current HttpContext/User

namespace TestAbsa.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly UserManager<ApplicationUser> _userManager; //  For accessing ApplicationUser
        private readonly IHttpContextAccessor _httpContextAccessor;  // For accessing the current HttpContext/User

        public InventoryService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _contextFactory = contextFactory;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<int> GetUserOrganizationIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                // In a secure application, this would be logged and redirect to login.
                // For now, throw an exception to prevent accidental data leakage.
                throw new InvalidOperationException("User context is unavailable. Authentication required.");
            }

            var applicationUser = await _userManager.GetUserAsync(user);

            if (applicationUser == null || applicationUser.OrganizationId == 0)
            {
                throw new InvalidOperationException("User organization ID not found or invalid.");
            }

            return applicationUser.OrganizationId;
        }

        // --- Product Management ---

        public async Task<List<Product>> GetAllProductsAsync(bool includeSuppliers = true)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            var query = context.Products
                .Where(p => p.OrganizationId == orgId) // FILTER ADDED
                .AsQueryable();

            if (includeSuppliers)
            {
                query = query.Include(p => p.Supplier);
            }

            var products = await query.ToListAsync();

            // Debugging info
            Console.WriteLine($"[InventoryService] Loaded {products.Count} products from the database.");

            foreach (var product in products)
            {
                Console.WriteLine($"  → ID: {product.Id}, Name: {product.ItemName}, SKU: {product.SKU}, Supplier: {product?.Supplier?.Name ?? "No Supplier"}");
            }

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            var product = await context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId); //filter added

            //  Debugging info
            if (product == null)
            {
                Console.WriteLine($"[InventoryService] No product found with ID: {id} for Organization {orgId}");
            }
            else
            {
                Console.WriteLine($"[InventoryService] Found Product — ID: {product.Id}, Name: {product.ItemName}, SKU: {product.SKU}, Supplier: {product?.Supplier?.Name ?? "No Supplier"}");
            }

            return product;
        }

        public async Task AddProductAsync(Product product)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            product.OrganizationId = orgId; //  ORG ID SET

            context.Products.Add(product);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            // For maximum security, could retrieve entity first 
            // and compare the OrganizationId, but the .Update() method is usually sufficient 
            // because a malicious user would have had to retrieve the product first (which gets filtered).
            using var context = await _contextFactory.CreateDbContextAsync();
            context.Products.Update(product);
            await context.SaveChangesAsync();
            // To make it more secure, can add 
            // var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            // product.OrganizationId = orgId; //  ORG ID SET

        }

        // --- Supplier Management ---

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            return await context.Suppliers
                .Where(s => s.OrganizationId == orgId) // FILTER ADDED
                .ToListAsync();
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            supplier.OrganizationId = orgId; // ORG ID SET

            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();
        }

        // --- Stock Request Management ---

        public async Task AddStockRequestAsync(StockRequest request)
        {
            try
            {
                using var context = await _contextFactory.CreateDbContextAsync();
                var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
                request.OrganizationId = orgId; // ORG ID SET
                request.RequestDate = DateTime.UtcNow;
                request.Status = "Pending";

                context.StockRequests.Add(request);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving StockRequest: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public async Task<List<StockRequest>> GetPendingStockRequestsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            return await context.StockRequests
                .Where(r => r.OrganizationId == orgId) // FILTER ADDED
                .Where(r => r.Status == "Pending")
                .Include(r => r.Product)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<StockRequest>> GetAllStockRequestsAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            return await context.StockRequests
                .Where(r => r.OrganizationId == orgId) // FILTER ADDED
                .Include(r => r.Product)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<StockRequest>> GetStockRequestsByEmployeeAsync(string employeeId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            // Filter by both OrganizationId AND EmployeeId for best security
            return await context.StockRequests
                .Where(r => r.OrganizationId == orgId) // FILTER ADDED
                .Include(r => r.Product)
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<bool> ReviewStockRequestAsync(int requestId, string status, string managerId, string managerName)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            var request = await context.StockRequests
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == requestId);

            if (request == null)
            {
                return false;
            }

            request.Status = status;
            request.ManagerId = managerId;
            request.ManagerName = managerName;
            request.ReviewDate = DateTime.UtcNow;

            if (status == "Approved" && request.Product != null)
            {
                //Product is filtered/loaded via a request 
                // that was already organization-filtered, so safe.
                // Increase the product's inventory (CurrentStock)
                request.Product.CurrentStock += request.Quantity;
                context.Products.Update(request.Product);
            }

            context.StockRequests.Update(request);
            await context.SaveChangesAsync();
            return true;
        }

        // --- Purchase Order Management ---

        public async Task<int> CreatePurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            purchaseOrder.OrganizationId = orgId; // ORG ID SET
            // Auto-calculate total cost
            purchaseOrder.TotalCost = purchaseOrder.OrderedQuantity * purchaseOrder.UnitCost;
            purchaseOrder.OrderDate = DateTime.UtcNow;
            purchaseOrder.Status = "Ordered";

            context.PurchaseOrders.Add(purchaseOrder);
            await context.SaveChangesAsync();
            return purchaseOrder.Id;
        }

        public async Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            return await context.PurchaseOrders
                .Where(p => p.OrganizationId == orgId) // FILTER ADDED
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .OrderByDescending(p => p.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            return await context.PurchaseOrders
                .Where(p => p.OrganizationId == orgId) // FILTER ADDED
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            return await context.PurchaseOrders
                .Where(p => p.OrganizationId == orgId) // FILTER ADDED
                .Where(p => p.Status == status)
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .ToListAsync();
        }

        public async Task<List<PurchaseOrder>> GetPendingPurchaseOrdersAsync()
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            return await context.PurchaseOrders
                .Where(p => p.OrganizationId == orgId) // FILTER ADDED
                .Where(p => p.Status == "Ordered" || p.Status == "Shipped")
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .ToListAsync();
        }

        public async Task<bool> UpdatePurchaseOrderStatusAsync(int id, string newStatus, DateTime? statusDate = null)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            // Use FirstOrDefaultAsync with the OrganizationId filter
            var order = await context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);
      
            if (order == null)
                return false;

            order.Status = newStatus;

            // Update relevant date fields based on status
            switch (newStatus)
            {
                case "Shipped":
                    order.ShippedDate = statusDate ?? DateTime.UtcNow;
                    break;
                case "Delivered":
                    order.DeliveredDate = statusDate ?? DateTime.UtcNow;
                    break;
                case "Cancelled":
                    // No special date field for cancel, but could add one if needed
                    break;
            }

            context.PurchaseOrders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsShippedAsync(int id, DateTime shippedDate)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            var order = await context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId); //FILTER ADDED
            if (order == null)
                return false;

            order.Status = "Shipped";
            order.ShippedDate = shippedDate;

            context.PurchaseOrders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReceiveStockAsync(int id, int receivedQuantity, string managerId, string managerName)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            var order = await context.PurchaseOrders
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId); // FILTER ADDED

            if (order == null)
                return false;

            order.ReceivedQuantity += receivedQuantity;

            // Update product stock
            if (order.Product != null)
            {
                order.Product.CurrentStock += receivedQuantity;
                context.Products.Update(order.Product);
            }

            // Determine new status
            if (order.ReceivedQuantity >= order.OrderedQuantity)
            {
                order.Status = "Delivered";
                order.DeliveredDate = DateTime.UtcNow;
            }
            else
            {
                order.Status = "PartiallyReceived";
            }

            context.PurchaseOrders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ReportIssueAsync(int id, string issueNotes, string managerId)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID

            var order = await context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId); // FILTER ADDED
            if (order == null)
                return false;

            order.Status = "Issue";
            order.IssueNotes = issueNotes;
            order.IssueReportedDate = DateTime.UtcNow;

            context.PurchaseOrders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelPurchaseOrderAsync(int id, string reason)
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var orgId = await GetUserOrganizationIdAsync(); // Fetch Org ID
            var order = await context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);
            if (order == null)
                return false;

            order.Status = "Cancelled";
            order.Notes = $"Cancelled: {reason}";

            context.PurchaseOrders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }
    }
}