using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestAbsa.Data;
using TestAbsa.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TestAbsa.Services
{
    public class InventoryService : IInventoryService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public InventoryService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            IHttpContextAccessor httpContextAccessor
            )
        {
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        private async Task<int> GetUserOrganizationIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                throw new InvalidOperationException("User context is unavailable. Authentication required.");
            }

            // Get the user ID from claims
            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                throw new InvalidOperationException("User ID not found in claims.");
            }

            // Create a dedicated context to fetch user info
            using var context = await _contextFactory.CreateDbContextAsync();
            var applicationUser = await context.Users
                .Where(u => u.Id == userId)
                .Select(u => new { u.OrganizationId })
                .FirstOrDefaultAsync();

            if (applicationUser == null || applicationUser.OrganizationId == 0)
            {
                throw new InvalidOperationException("User organization ID not found or invalid.");
            }

            return applicationUser.OrganizationId;
        }

        // --- Product Management ---

        public async Task<List<Product>> GetAllProductsAsync(bool includeSuppliers = true)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var query = context.Products
                .Where(p => p.OrganizationId == orgId)
                .AsQueryable();

            if (includeSuppliers)
            {
                query = query.Include(p => p.Supplier);
            }

            var products = await query.ToListAsync();

            Console.WriteLine($"[InventoryService] Loaded {products.Count} products from the database.");

            foreach (var product in products)
            {
                Console.WriteLine($"  → ID: {product.Id}, Name: {product.ItemName}, SKU: {product.SKU}, Supplier: {product?.Supplier?.Name ?? "No Supplier"}");
            }

            return products;
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var product = await context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);

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
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            product.OrganizationId = orgId;

            context.Products.Add(product);
            await context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            // Verify the product belongs to this organization
            var existingProduct = await context.Products
                .FirstOrDefaultAsync(p => p.Id == product.Id && p.OrganizationId == orgId);

            if (existingProduct == null)
                throw new InvalidOperationException("Product not found or access denied.");

            // Ensure correct organization
            product.OrganizationId = orgId;

            // Copy updated values into the tracked entity instead of reattaching
            context.Entry(existingProduct).CurrentValues.SetValues(product);

            await context.SaveChangesAsync();
        }


        // --- Supplier Management ---

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.Suppliers
                .Where(s => s.OrganizationId == orgId)
                .ToListAsync();
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            supplier.OrganizationId = orgId;

            context.Suppliers.Add(supplier);
            await context.SaveChangesAsync();
        }

        // --- Stock Request Management ---

        public async Task AddStockRequestAsync(StockRequest request)
        {
            try
            {
                var orgId = await GetUserOrganizationIdAsync();
                using var context = await _contextFactory.CreateDbContextAsync();

                request.OrganizationId = orgId;
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
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.StockRequests
                .Where(r => r.OrganizationId == orgId)
                .Where(r => r.Status == "Pending")
                .Include(r => r.Product)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<StockRequest>> GetAllStockRequestsAsync()
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.StockRequests
                .Where(r => r.OrganizationId == orgId)
                .Include(r => r.Product)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<StockRequest>> GetStockRequestsByEmployeeAsync(string employeeId)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.StockRequests
                .Where(r => r.OrganizationId == orgId)
                .Include(r => r.Product)
                .Where(r => r.EmployeeId == employeeId)
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<bool> ReviewStockRequestAsync(int requestId, string status, string managerId, string managerName)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var request = await context.StockRequests
                .Include(r => r.Product)
                .FirstOrDefaultAsync(r => r.Id == requestId && r.OrganizationId == orgId);

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
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            purchaseOrder.OrganizationId = orgId;
            purchaseOrder.TotalCost = purchaseOrder.OrderedQuantity * purchaseOrder.UnitCost;
            purchaseOrder.OrderDate = DateTime.UtcNow;
            purchaseOrder.Status = "Ordered";

            context.PurchaseOrders.Add(purchaseOrder);
            await context.SaveChangesAsync();
            return purchaseOrder.Id;
        }

        public async Task<List<PurchaseOrder>> GetAllPurchaseOrdersAsync()
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.PurchaseOrders
                .Where(p => p.OrganizationId == orgId)
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .OrderByDescending(p => p.OrderDate)
                .ToListAsync();
        }

        public async Task<PurchaseOrder?> GetPurchaseOrderByIdAsync(int id)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.PurchaseOrders
                .Where(p => p.OrganizationId == orgId)
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<List<PurchaseOrder>> GetPurchaseOrdersByStatusAsync(string status)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.PurchaseOrders
                .Where(p => p.OrganizationId == orgId)
                .Where(p => p.Status == status)
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .ToListAsync();
        }

        public async Task<List<PurchaseOrder>> GetPendingPurchaseOrdersAsync()
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.PurchaseOrders
                .Where(p => p.OrganizationId == orgId)
                .Where(p => p.Status == "Ordered" || p.Status == "Shipped")
                .Include(p => p.Supplier)
                .Include(p => p.Product)
                .ToListAsync();
        }

        public async Task<bool> UpdatePurchaseOrderStatusAsync(int id, string newStatus, DateTime? statusDate = null)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var order = await context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);

            if (order == null)
                return false;

            order.Status = newStatus;

            switch (newStatus)
            {
                case "Shipped":
                    order.ShippedDate = statusDate ?? DateTime.UtcNow;
                    break;
                case "Delivered":
                    order.DeliveredDate = statusDate ?? DateTime.UtcNow;
                    break;
                case "Cancelled":
                    break;
            }

            context.PurchaseOrders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkAsShippedAsync(int id, DateTime shippedDate)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var order = await context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);
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
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var order = await context.PurchaseOrders
                .Include(p => p.Product)
                .FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);

            if (order == null)
                return false;

            order.ReceivedQuantity += receivedQuantity;

            if (order.Product != null)
            {
                order.Product.CurrentStock += receivedQuantity;
                context.Products.Update(order.Product);
            }

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
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var order = await context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);
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
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var order = await context.PurchaseOrders.FirstOrDefaultAsync(p => p.Id == id && p.OrganizationId == orgId);
            if (order == null)
                return false;

            order.Status = "Cancelled";
            order.Notes = $"Cancelled: {reason}";

            context.PurchaseOrders.Update(order);
            await context.SaveChangesAsync();
            return true;
        }

        public class InventorySummary
        {
            public int TotalProducts { get; set; }
            public int LowStockItems { get; set; }
            public int PendingStockRequests { get; set; }
            public int PendingPurchaseOrders { get; set; }
            public int TotalSuppliers { get; set; }
        }

        public async Task<InventorySummary> GetInventorySummaryAsync()
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var totalProducts = await context.Products.CountAsync(p => p.OrganizationId == orgId);
            var lowStockItems = await context.Products.CountAsync(p => p.OrganizationId == orgId && p.CurrentStock < p.MinLevel);
            var pendingStockRequests = await context.StockRequests.CountAsync(r => r.OrganizationId == orgId && r.Status == "Pending");
            var pendingPurchaseOrders = await context.PurchaseOrders.CountAsync(o => o.OrganizationId == orgId && (o.Status == "Ordered" || o.Status == "Shipped"));
            var totalSuppliers = await context.Suppliers.CountAsync(s => s.OrganizationId == orgId);

            return new InventorySummary
            {
                TotalProducts = totalProducts,
                LowStockItems = lowStockItems,
                PendingStockRequests = pendingStockRequests,
                PendingPurchaseOrders = pendingPurchaseOrders,
                TotalSuppliers = totalSuppliers
            };
        }

    }
}