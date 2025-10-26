using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using TestAbsa.Data;
using TestAbsa.Data.Models;
using TestAbsa.Services;

namespace TestAbsa.Services
{
    public class FinanceService : IFinanceService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly ILogger<FinanceService> _logger;
        private readonly IUserContext _userContext;

        public FinanceService(
            IDbContextFactory<ApplicationDbContext> contextFactory,
            ILogger<FinanceService> logger,
            IUserContext userContext)
        {
            _contextFactory = contextFactory;
            _logger = logger;
            _userContext = userContext;
        }

        // Helper method to get context
        private ApplicationDbContext CreateContext() => _contextFactory.CreateDbContext();

        // =====================================================
        // --- CUSTOMER MANAGEMENT ---
        // =====================================================

        public async Task<List<Customer>> GetAllCustomersAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Customers
                    .Where(c => c.OrganizationId == orgId)
                    .OrderBy(c => c.Name)
                    .ToListAsync();


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all customers");
                throw;
            }
        }

        public async Task<List<Customer>> GetActiveCustomersAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Customers
                    .Where(c => c.OrganizationId == orgId)
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.Name)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active customers");
                throw;
            }
        }

        public async Task<Customer?> GetCustomerByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid customer ID", nameof(id));

            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Customers
                    .Where(c => c.OrganizationId == orgId)
                    .Include(c => c.Invoices)
                    .FirstOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving customer {CustomerId}", id);
                throw;
            }
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            try
            {
                using var context = CreateContext();
                customer.CreatedDate = DateTime.UtcNow;
                context.Customers.Add(customer);
                customer.OrganizationId = await GetUserOrganizationIdAsync();
                await context.SaveChangesAsync();
                _logger.LogInformation("Customer {CustomerId} created successfully", customer.Id);
                return customer;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding customer");
                throw;
            }
        }

        public async Task UpdateCustomerAsync(Customer customer)
        {
            if (customer == null)
                throw new ArgumentNullException(nameof(customer));

            try
            {
                using var context = CreateContext();
                context.Customers.Update(customer);
                customer.OrganizationId = await GetUserOrganizationIdAsync();
                await context.SaveChangesAsync();
                _logger.LogInformation("Customer {CustomerId} updated successfully", customer.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating customer {CustomerId}", customer.Id);
                throw;
            }
        }

        public async Task DeleteCustomerAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid customer ID", nameof(id));

            try
            {
                using var context = CreateContext();
                var customer = await context.Customers.FindAsync(id);
                if (customer != null)
                {
                    // Actually remove from database
                    context.Customers.Remove(customer);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Customer {CustomerId} deleted successfully", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting customer {CustomerId}", id);
                throw;
            }
        }

        public async Task<bool> CustomerExistsAsync(int id)
        {
            using var context = CreateContext();
            var orgId = await GetUserOrganizationIdAsync();
            return await context.Customers
                .Where(c => c.OrganizationId == orgId)
                .AnyAsync(c => c.Id == id);
        }

        // =====================================================
        // --- INVOICE MANAGEMENT ---
        // =====================================================

        public async Task<List<Invoice>> GetAllInvoicesAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Invoices
                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceItems)
                    .Where(i => i.OrganizationId == orgId)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all invoices");
                throw;
            }
        }

        public async Task<List<Invoice>> GetInvoicesByCustomerIdAsync(int customerId)
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Invoices
                    .Include(i => i.InvoiceItems)
                    .Where(i => i.CustomerId == customerId)
                    .Where(i => i.OrganizationId == orgId)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices for customer {CustomerId}", customerId);
                throw;
            }
        }

        public async Task<List<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status)
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Invoices
                                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceItems)
                    .Where(i => i.Status == status)
                    .Where(i => i.OrganizationId == orgId)
                    .OrderByDescending(i => i.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoices by status {Status}", status);
                throw;
            }
        }

        public async Task<Invoice?> GetInvoiceByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid invoice ID", nameof(id));

            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Invoices
                                    .Include(i => i.Customer)
                    .Include(i => i.InvoiceItems)
                    .Where(i => i.OrganizationId == orgId)
                    .FirstOrDefaultAsync(i => i.Id == id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice {InvoiceId}", id);
                throw;
            }
        }

        public async Task<Invoice> AddInvoiceAsync(Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            try
            {
                using var context = CreateContext();
                invoice.CreatedDate = DateTime.UtcNow;
                invoice.InvoiceNumber = GenerateInvoiceNumber();
                invoice.OrganizationId = await GetUserOrganizationIdAsync();
                context.Invoices.Add(invoice);
                await context.SaveChangesAsync();
                _logger.LogInformation("Invoice {InvoiceId} created successfully", invoice.Id);
                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding invoice");
                throw;
            }
        }

        public async Task UpdateInvoiceAsync(Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            try
            {
                using var context = CreateContext();
                invoice.OrganizationId = await GetUserOrganizationIdAsync();
                context.Invoices.Update(invoice);
                await context.SaveChangesAsync();
                _logger.LogInformation("Invoice {InvoiceId} updated successfully", invoice.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice {InvoiceId}", invoice.Id);
                throw;
            }
        }

        public async Task DeleteInvoiceAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid invoice ID", nameof(id));

            try
            {
                using var context = CreateContext();
                var invoice = await context.Invoices.FindAsync(id);
                if (invoice != null)
                {
                    context.Invoices.Remove(invoice);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Invoice {InvoiceId} deleted successfully", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice {InvoiceId}", id);
                throw;
            }
        }

        public async Task UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status)
        {
            try
            {
                using var context = CreateContext();
                var invoice = await context.Invoices.FindAsync(invoiceId);
                if (invoice != null)
                {
                    invoice.Status = status;
                    if (status == InvoiceStatus.Paid)
                        invoice.PaidDate = DateTime.UtcNow;
                    invoice.OrganizationId = await GetUserOrganizationIdAsync();

                    await context.SaveChangesAsync();
                    _logger.LogInformation("Invoice {InvoiceId} status updated to {Status}", invoiceId, status);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice status {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task<decimal> GetTotalInvoiceAmountAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Invoices
                    .Where(i => i.OrganizationId == orgId)
                    .SumAsync(i => i.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total invoice amount");
                throw;
            }
        }

        public async Task<decimal> GetTotalPaidInvoicesAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Invoices
                    .Where(i => i.OrganizationId == orgId)
                    .Where(i => i.Status == InvoiceStatus.Paid)
                    .SumAsync(i => i.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total paid invoices");
                throw;
            }
        }

        public async Task<decimal> GetTotalOutstandingInvoicesAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Invoices
                    .Where(i => i.OrganizationId == orgId)
                    .Where(i => i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled)
                    .SumAsync(i => i.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating outstanding invoices");
                throw;
            }
        }

        // =====================================================
        // --- INVOICE ITEMS ---
        // =====================================================

        public async Task<List<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(int invoiceId)
        {
            try
            {
                using var context = CreateContext();
                return await context.Set<InvoiceItem>()
                    .Where(item => item.InvoiceId == invoiceId)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice items for invoice {InvoiceId}", invoiceId);
                throw;
            }
        }

        public async Task AddInvoiceItemAsync(InvoiceItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                using var context = CreateContext();
                context.Set<InvoiceItem>().Add(item);
                await context.SaveChangesAsync();
                await RecalculateInvoiceAmount(item.InvoiceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding invoice item");
                throw;
            }
        }

        public async Task UpdateInvoiceItemAsync(InvoiceItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            try
            {
                using var context = CreateContext();
                context.Set<InvoiceItem>().Update(item);
                await context.SaveChangesAsync();
                await RecalculateInvoiceAmount(item.InvoiceId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice item");
                throw;
            }
        }

        public async Task DeleteInvoiceItemAsync(int id)
        {
            try
            {
                using var context = CreateContext();
                var item = await context.Set<InvoiceItem>().FindAsync(id);
                if (item != null)
                {
                    var invoiceId = item.InvoiceId;
                    context.Set<InvoiceItem>().Remove(item);
                    await context.SaveChangesAsync();
                    await RecalculateInvoiceAmount(invoiceId);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting invoice item {ItemId}", id);
                throw;
            }
        }

        // =====================================================
        // --- EXPENSE MANAGEMENT ---
        // =====================================================

        public async Task<List<Expense>> GetAllExpensesAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Expenses
                    .Where(e => e.OrganizationId == orgId)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all expenses");
                throw;
            }
        }

        public async Task<List<Expense>> GetExpensesByCategoryAsync(ExpenseCategory category)
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Expenses
                    .Where(e => e.OrganizationId == orgId)
                    .Where(e => e.Category == category)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expenses by category {Category}", category);
                throw;
            }
        }

        public async Task<List<Expense>> GetPendingExpensesAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Expenses
                    .Where(e => e.OrganizationId == orgId)
                    .Where(e => !e.IsApproved)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending expenses");
                throw;
            }
        }

        public async Task<List<Expense>> GetApprovedExpensesAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Expenses
                    .Where(e => e.OrganizationId == orgId)
                    .Where(e => e.IsApproved)
                    .OrderByDescending(e => e.Date)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving approved expenses");
                throw;
            }
        }

        public async Task<Expense?> GetExpenseByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid expense ID", nameof(id));

            try
            {
                using var context = CreateContext();
                return await context.Expenses.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving expense {ExpenseId}", id);
                throw;
            }
        }

        public async Task<Expense> AddExpenseAsync(Expense expense)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense));

            try
            {
                using var context = CreateContext();
                expense.Date = DateTime.UtcNow;
                expense.OrganizationId = await GetUserOrganizationIdAsync();
                context.Expenses.Add(expense);
                await context.SaveChangesAsync();
                _logger.LogInformation("Expense {ExpenseId} created successfully", expense.Id);
                return expense;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding expense");
                throw;
            }
        }

        public async Task UpdateExpenseAsync(Expense expense)
        {
            if (expense == null)
                throw new ArgumentNullException(nameof(expense));

            try
            {
                using var context = CreateContext();
                expense.OrganizationId = await GetUserOrganizationIdAsync();
                context.Expenses.Update(expense);
                await context.SaveChangesAsync();
                _logger.LogInformation("Expense {ExpenseId} updated successfully", expense.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating expense {ExpenseId}", expense.Id);
                throw;
            }
        }

        public async Task DeleteExpenseAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid expense ID", nameof(id));

            try
            {
                using var context = CreateContext();
                var expense = await context.Expenses.FindAsync(id);
                if (expense != null)
                {
                    context.Expenses.Remove(expense);
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Expense {ExpenseId} deleted successfully", id);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting expense {ExpenseId}", id);
                throw;
            }
        }

        public async Task ApproveExpenseAsync(int id, string approvedBy)
        {
            try
            {
                using var context = CreateContext();
                var expense = await context.Expenses.FindAsync(id);
                if (expense != null)
                {
                    expense.IsApproved = true;
                    expense.ApprovedBy = approvedBy;
                    expense.ApprovedDate = DateTime.UtcNow;
                    expense.OrganizationId = await GetUserOrganizationIdAsync();
                    await context.SaveChangesAsync();
                    _logger.LogInformation("Expense {ExpenseId} approved by {ApprovedBy}", id, approvedBy);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving expense {ExpenseId}", id);
                throw;
            }
        }

        public async Task<decimal> GetTotalExpensesAsync()
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Expenses
                    .Where(e => e.OrganizationId == orgId)
                    .Where(e => e.IsApproved)
                    .SumAsync(e => e.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total expenses");
                throw;
            }
        }

        public async Task<decimal> GetTotalExpensesByCategoryAsync(ExpenseCategory category)
        {
            try
            {
                using var context = CreateContext();
                var orgId = await GetUserOrganizationIdAsync();
                return await context.Expenses
                    .Where(e => e.OrganizationId == orgId)
                    .Where(e => e.Category == category && e.IsApproved)
                    .SumAsync(e => e.Amount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating total expenses by category {Category}", category);
                throw;
            }
        }

        // =====================================================
        // --- PDF GENERATION ---
        // =====================================================

        public async Task<byte[]> GenerateInvoicePdfAsync(int invoiceId)
        {
            try
            {
                var invoice = await GetInvoiceByIdAsync(invoiceId);
                if (invoice == null)
                    throw new Exception($"Invoice {invoiceId} not found");

                if (invoice.Customer == null)
                    throw new Exception($"Customer not found for invoice {invoiceId}");

                using var stream = new MemoryStream();
                var document = new PdfDocument();
                document.Info.Title = $"Invoice {invoice.InvoiceNumber}";

                var page = document.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;

                var gfx = XGraphics.FromPdfPage(page);
                var titleFont = new XFont("Arial", 20, XFontStyle.Bold);
                var headingFont = new XFont("Arial", 12, XFontStyle.Bold);
                var normalFont = new XFont("Arial", 10, XFontStyle.Regular);

                double yPos = 40;
                var blackBrush = new XSolidBrush(XColor.FromArgb(0, 0, 0));
                var blackPen = new XPen(XColor.FromArgb(0, 0, 0), 1);

                // Header
                gfx.DrawString("INVOICE", titleFont, blackBrush, new XRect(40, yPos, page.Width, 30), XStringFormats.TopLeft);
                yPos += 40;

                gfx.DrawString($"Invoice #: {invoice.InvoiceNumber}", normalFont, blackBrush, new XRect(40, yPos, 300, 20), XStringFormats.TopLeft);
                gfx.DrawString($"Status: {invoice.Status}", normalFont, blackBrush, new XRect(400, yPos, 150, 20), XStringFormats.TopLeft);
                yPos += 20;

                gfx.DrawString($"Date: {invoice.CreatedDate:yyyy-MM-dd}", normalFont, blackBrush, new XRect(40, yPos, 300, 20), XStringFormats.TopLeft);
                if (invoice.DueDate.HasValue)
                    gfx.DrawString($"Due: {invoice.DueDate.Value:yyyy-MM-dd}", normalFont, blackBrush, new XRect(400, yPos, 150, 20), XStringFormats.TopLeft);
                yPos += 30;

                // Customer
                gfx.DrawString("Bill To:", headingFont, blackBrush, new XRect(40, yPos, 200, 20), XStringFormats.TopLeft);
                yPos += 20;
                gfx.DrawString(invoice.Customer.Name, normalFont, blackBrush, new XRect(40, yPos, 300, 20), XStringFormats.TopLeft);
                yPos += 15;
                gfx.DrawString(invoice.Customer.Email, normalFont, blackBrush, new XRect(40, yPos, 300, 20), XStringFormats.TopLeft);
                yPos += 15;
                gfx.DrawString(invoice.Customer.Phone ?? "", normalFont, blackBrush, new XRect(40, yPos, 300, 20), XStringFormats.TopLeft);
                yPos += 30;

                // Items Table Header
                gfx.DrawString("Description", headingFont, blackBrush, new XRect(40, yPos, 200, 20), XStringFormats.TopLeft);
                gfx.DrawString("Qty", headingFont, blackBrush, new XRect(300, yPos, 50, 20), XStringFormats.TopLeft);
                gfx.DrawString("Unit Price", headingFont, blackBrush, new XRect(350, yPos, 80, 20), XStringFormats.TopLeft);
                gfx.DrawString("Total", headingFont, blackBrush, new XRect(450, yPos, 80, 20), XStringFormats.TopLeft);
                yPos += 20;
                gfx.DrawLine(blackPen, 40, yPos, 550, yPos);
                yPos += 10;

                // Items
                if (invoice.InvoiceItems != null && invoice.InvoiceItems.Any())
                {
                    foreach (var item in invoice.InvoiceItems)
                    {
                        gfx.DrawString(item.Description, normalFont, blackBrush, new XRect(40, yPos, 250, 20), XStringFormats.TopLeft);
                        gfx.DrawString(item.Quantity.ToString(), normalFont, blackBrush, new XRect(300, yPos, 50, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{item.UnitPrice:C}", normalFont, blackBrush, new XRect(350, yPos, 80, 20), XStringFormats.TopLeft);
                        gfx.DrawString($"{item.Total:C}", normalFont, blackBrush, new XRect(450, yPos, 80, 20), XStringFormats.TopLeft);
                        yPos += 20;
                    }
                }

                yPos += 10;
                gfx.DrawLine(blackPen, 40, yPos, 550, yPos);
                yPos += 20;

                // Totals
                var subtotal = invoice.InvoiceItems?.Sum(i => i.Total) ?? 0;
                gfx.DrawString($"Subtotal: {subtotal:C}", headingFont, blackBrush, new XRect(400, yPos, 150, 20), XStringFormats.TopLeft);
                yPos += 20;
                gfx.DrawString($"Total: {invoice.Amount:C}", headingFont, blackBrush, new XRect(400, yPos, 150, 20), XStringFormats.TopLeft);

                // Footer
                yPos += 50;
                gfx.DrawString("Thank you for your business!", normalFont, blackBrush, new XRect(40, yPos, 500, 20), XStringFormats.TopLeft);

                document.Save(stream, false);
                return stream.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice PDF for {InvoiceId}", invoiceId);
                throw;
            }
        }

        // =====================================================
        // --- DASHBOARD STATS ---
        // =====================================================

        public async Task<Dictionary<string, decimal>> GetDashboardStatisticsAsync()
        {
            try
            {
                var orgId = await GetUserOrganizationIdAsync();

                using var context = CreateContext();
                var totalInvoices = await context.Invoices.Where(i => i.OrganizationId == orgId).SumAsync(i => i.Amount);
                var paidInvoices = await context.Invoices.Where(i => i.OrganizationId == orgId && i.Status == InvoiceStatus.Paid).SumAsync(i => i.Amount);
                var outstanding = await context.Invoices.Where(i => i.OrganizationId == orgId && i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Cancelled).SumAsync(i => i.Amount);
                var totalExpenses = await context.Expenses.Where(e => e.OrganizationId == orgId && e.IsApproved).SumAsync(e => e.Amount);

                return new Dictionary<string, decimal>
                {
                    { "TotalInvoices", totalInvoices },
                    { "PaidInvoices", paidInvoices },
                    { "OutstandingInvoices", outstanding },
                    { "TotalExpenses", totalExpenses },
                    { "NetRevenue", paidInvoices - totalExpenses }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating dashboard statistics");
                throw;
            }
        }

        // =====================================================
        // --- PRIVATE HELPERS ---
        // =====================================================

        private string GenerateInvoiceNumber()
        {
            var year = DateTime.UtcNow.Year;
            var month = DateTime.UtcNow.Month;
            var random = new Random().Next(1000, 9999);
            return $"INV-{year}{month:D2}-{random}";
        }

        private async Task RecalculateInvoiceAmount(int invoiceId)
        {
            using var context = CreateContext();
            var invoice = await context.Invoices
                .Include(i => i.InvoiceItems)
                .FirstOrDefaultAsync(i => i.Id == invoiceId);

            if (invoice != null)
            {
                invoice.Amount = invoice.InvoiceItems.Sum(item => item.Total);
                await context.SaveChangesAsync();
            }
        }

        private async Task<int> GetUserOrganizationIdAsync()
        {
            var orgId = await _userContext.GetOrganizationIdAsync();

            if (orgId == null || orgId == 0)
                throw new InvalidOperationException("User organization ID not found or invalid.");

            return orgId.Value; // safely extract the int from int?
        }


    }
}