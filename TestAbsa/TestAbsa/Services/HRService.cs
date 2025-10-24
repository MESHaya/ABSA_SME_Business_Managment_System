using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TestAbsa.Data;
using TestAbsa.Data.Models;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TestAbsa.Services
{
    public class HRService : IHRService
    {
        private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public HRService(IDbContextFactory<ApplicationDbContext> contextFactory, IHttpContextAccessor httpContextAccessor)
        {
            _contextFactory = contextFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        // Get organization ID from logged-in user
        private async Task<int> GetUserOrganizationIdAsync()
        {
            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity.IsAuthenticated)
                throw new InvalidOperationException("User context unavailable.");

            var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                throw new InvalidOperationException("User ID missing from claims.");

            using var context = await _contextFactory.CreateDbContextAsync();
            var org = await context.Users
                .Where(u => u.Id == userId)
                .Select(u => u.OrganizationId)
                .FirstOrDefaultAsync();

            if (org == 0)
                throw new InvalidOperationException("Organization not found for user.");

            return org;
        }

        // --- HR DASHBOARD SUMMARY ---
        public class HRSummary
        {
            public int PendingLeaveRequests { get; set; }
            public int ApprovedLeaveRequests { get; set; }
            public int RejectedLeaveRequests { get; set; }
            public int TotalTimesheetEntries { get; set; }
            public int PendingTimesheets { get; set; }
            public int TotalLeaveRequests => PendingLeaveRequests + ApprovedLeaveRequests + RejectedLeaveRequests;
            public double TotalHoursWorked { get; set; }
        }

        public async Task<HRSummary> GetHRSummaryAsync()
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var pendingLeave = await context.LeaveRequests.CountAsync(l => l.OrganizationId == orgId && l.Status == "Pending");
            var approvedLeave = await context.LeaveRequests.CountAsync(l => l.OrganizationId == orgId && l.Status == "Approved");
            var rejectedLeave = await context.LeaveRequests.CountAsync(l => l.OrganizationId == orgId && l.Status == "Rejected");

            var totalTimesheets = await context.TimesheetEntries.CountAsync(t => t.OrganizationId == orgId);
            var pendingTimesheets = await context.TimesheetEntries.CountAsync(t => t.OrganizationId == orgId && !t.IsApproved && !t.IsRejected);

            var totalHoursWorked = await context.TimesheetEntries
         .Where(t => t.OrganizationId == orgId)
         .SumAsync(t => (double?)t.HoursWorked) ?? 0;

            return new HRSummary
            {
                PendingLeaveRequests = pendingLeave,
                ApprovedLeaveRequests = approvedLeave,
                RejectedLeaveRequests = rejectedLeave,
                TotalTimesheetEntries = totalTimesheets,
                PendingTimesheets = pendingTimesheets,
                TotalHoursWorked = totalHoursWorked
            };

        }

        // --- LEAVE REQUESTS ---

        public async Task<List<LeaveRequest>> GetAllLeaveRequestsAsync()
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.LeaveRequests
                .Where(l => l.OrganizationId == orgId)
                .Include(l => l.Employee)
                .OrderByDescending(l => l.RequestDate)
                .ToListAsync();
        }

        public async Task<List<LeaveRequest>> GetRecentLeaveRequestsAsync(int count = 5)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            return await context.LeaveRequests
                .Where(l => l.OrganizationId == orgId)
                .Include(l => l.Employee)
                .OrderByDescending(l => l.RequestDate)
                .Take(count)
                .ToListAsync();
        }

        public async Task SubmitLeaveRequestAsync(LeaveRequest request)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            request.OrganizationId = orgId;
            request.Status = "Pending";
            request.RequestDate = DateTime.UtcNow;
            request.TotalDays = (int)(request.EndDate - request.StartDate).TotalDays + 1;

            context.LeaveRequests.Add(request);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ReviewLeaveRequestAsync(int id, string status, string? managerId, string? managerComments = null)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var request = await context.LeaveRequests.FirstOrDefaultAsync(l => l.Id == id && l.OrganizationId == orgId);
            if (request == null) return false;

            request.Status = status;
            request.ManagerId = managerId;
            request.ManagerComments = managerComments;
            request.ReviewedDate = DateTime.UtcNow;

            context.LeaveRequests.Update(request);
            await context.SaveChangesAsync();
            return true;
        }

        // --- TIMESHEET ENTRIES ---

        public async Task<List<TimesheetEntry>> GetTimesheetEntriesAsync(string? employeeId = null)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var query = context.TimesheetEntries
                .Where(t => t.OrganizationId == orgId)
                .Include(t => t.Employee)
                .AsQueryable();

            if (!string.IsNullOrEmpty(employeeId))
                query = query.Where(t => t.EmployeeId == employeeId);

            return await query.OrderByDescending(t => t.WorkDate).ToListAsync();
        }

        public async Task AddTimesheetEntryAsync(TimesheetEntry entry)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            entry.OrganizationId = orgId;
            entry.CreatedDate = DateTime.UtcNow;

            context.TimesheetEntries.Add(entry);
            await context.SaveChangesAsync();
        }

        public async Task<bool> ApproveTimesheetAsync(int id, string managerId)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var entry = await context.TimesheetEntries.FirstOrDefaultAsync(t => t.Id == id && t.OrganizationId == orgId);
            if (entry == null) return false;

            entry.IsApproved = true;
            entry.ApprovedByManagerId = managerId;
            entry.ApprovedDate = DateTime.UtcNow;

            context.TimesheetEntries.Update(entry);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectTimesheetAsync(int id, string managerId, string reason)
        {
            var orgId = await GetUserOrganizationIdAsync();
            using var context = await _contextFactory.CreateDbContextAsync();

            var entry = await context.TimesheetEntries.FirstOrDefaultAsync(t => t.Id == id && t.OrganizationId == orgId);
            if (entry == null) return false;

            entry.IsRejected = true;
            entry.RejectionReason = reason;
            entry.RejectedByManagerId = managerId;
            entry.RejectedDate = DateTime.UtcNow;

            context.TimesheetEntries.Update(entry);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
