using System.Collections.Generic;
using System.Threading.Tasks;
using TestAbsa.Data.Models;

namespace TestAbsa.Services
{
    public interface IFinanceService
    {
        // --- Customer Management ---
        Task<List<Customer>> GetAllCustomersAsync();
        Task<List<Customer>> GetActiveCustomersAsync();
        Task<Customer?> GetCustomerByIdAsync(int id);
        Task<Customer> AddCustomerAsync(Customer customer);
        Task UpdateCustomerAsync(Customer customer);
        Task DeleteCustomerAsync(int id);
        Task<bool> CustomerExistsAsync(int id);

        // --- Invoice Management ---
        Task<List<Invoice>> GetAllInvoicesAsync();
        Task<List<Invoice>> GetInvoicesByCustomerIdAsync(int customerId);
        Task<List<Invoice>> GetInvoicesByStatusAsync(InvoiceStatus status);
        Task<Invoice?> GetInvoiceByIdAsync(int id);
        Task<Invoice> AddInvoiceAsync(Invoice invoice);
        Task UpdateInvoiceAsync(Invoice invoice);
        Task DeleteInvoiceAsync(int id);
        Task UpdateInvoiceStatusAsync(int invoiceId, InvoiceStatus status);
        Task<byte[]> GenerateInvoicePdfAsync(int invoiceId);

        // --- Invoice Items ---
        Task<List<InvoiceItem>> GetInvoiceItemsByInvoiceIdAsync(int invoiceId);
        Task AddInvoiceItemAsync(InvoiceItem item);
        Task UpdateInvoiceItemAsync(InvoiceItem item);
        Task DeleteInvoiceItemAsync(int id);

        // --- Expense Management ---
        Task<List<Expense>> GetAllExpensesAsync();
        Task<List<Expense>> GetExpensesByCategoryAsync(ExpenseCategory category);
        Task<List<Expense>> GetPendingExpensesAsync();
        Task<List<Expense>> GetApprovedExpensesAsync();
        Task<Expense?> GetExpenseByIdAsync(int id);
        Task<Expense> AddExpenseAsync(Expense expense);
        Task UpdateExpenseAsync(Expense expense);
        Task DeleteExpenseAsync(int id);
        Task ApproveExpenseAsync(int id, string approvedBy);
        Task<decimal> GetTotalExpensesAsync();
        Task<decimal> GetTotalExpensesByCategoryAsync(ExpenseCategory category);

        // --- Dashboard Statistics ---
        Task<Dictionary<string, decimal>> GetDashboardStatisticsAsync();

        // --- Totals ---
        Task<decimal> GetTotalInvoiceAmountAsync();
        Task<decimal> GetTotalPaidInvoicesAsync();
        Task<decimal> GetTotalOutstandingInvoicesAsync();
    }
}