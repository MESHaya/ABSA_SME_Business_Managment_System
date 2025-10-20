using Microsoft.EntityFrameworkCore;
using TestAbsa.Data.Models; // Ensure this is the correct namespace for your models
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using TestAbsa.Data;

namespace TestAbsa.Services
{
    public class InventoryService : IInventoryService
    {
        // Must use 'ApplicationDbContext' from 'SmeApp.Data' namespace
        private readonly ApplicationDbContext _context;

        public InventoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        // --- Product Management ---

        public async Task<List<Product>> GetAllProductsAsync(bool includeSuppliers = true)
        {
            var query = _context.Products.AsQueryable();

            if (includeSuppliers)
            {
                query = query.Include(p => p.Supplier);
            }

            var products = await query.ToListAsync();

            // 🧠 Debugging info
            Console.WriteLine($"[InventoryService] Loaded {products.Count} products from the database.");

            foreach (var product in products)
            {
                Console.WriteLine($"  → ID: {product.Id}, Name: {product.ItemName}, SKU: {product.SKU}, Supplier: {product?.Supplier?.Name ?? "No Supplier"}");
            }

            return products;
        }


        public async Task<Product?> GetProductByIdAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Supplier)
                .FirstOrDefaultAsync(p => p.Id == id);

            // 🧠 Debugging info
            if (product == null)
            {
                Console.WriteLine($"[InventoryService] No product found with ID: {id}");
            }
            else
            {
                Console.WriteLine($"[InventoryService] Found Product — ID: {product.Id}, Name: {product.ItemName}, SKU: {product.SKU}, Supplier: {product?.Supplier?.Name ?? "No Supplier"}");
            }

            return product;
        }


        public async Task AddProductAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // --- Supplier Management ---

        public async Task<List<Supplier>> GetAllSuppliersAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task AddSupplierAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        // --- Stock Request Management ---


        public async Task AddStockRequestAsync(StockRequest request)
        {
            try
            {
                request.RequestDate = DateTime.UtcNow;
                request.Status = "Pending";

                _context.StockRequests.Add(request);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error saving StockRequest: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public async Task<List<StockRequest>> GetPendingStockRequestsAsync()
        {
            return await _context.StockRequests
                .Where(r => r.Status == "Pending")
                .Include(r => r.Product)
                .OrderBy(r => r.RequestDate)
                .ToListAsync();
        }

        public async Task<List<StockRequest>> GetAllStockRequestsAsync()
        {
            return await _context.StockRequests
                .Include(r => r.Product) // include related product info
                .OrderByDescending(r => r.RequestDate)
                .ToListAsync();
        }


        public async Task<bool> ReviewStockRequestAsync(int requestId, string status, string managerId, string managerName)
        {
            var request = await _context.StockRequests
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
                // Increase the product's inventory (CurrentStock)
                request.Product.CurrentStock += request.Quantity;
                _context.Products.Update(request.Product);
            }

            _context.StockRequests.Update(request);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
