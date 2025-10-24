using TestAbsa.Data.Models;

namespace TestAbsa.Services
{
    public interface IHRService
    {
        Task<HRService.HRSummary> GetHRSummaryAsync();
        Task<List<LeaveRequest>> GetAllLeaveRequestsAsync();
        Task SubmitLeaveRequestAsync(LeaveRequest request);
        Task<bool> ReviewLeaveRequestAsync(int id, string status, string? managerId, string? managerComments = null);
        Task<List<TimesheetEntry>> GetTimesheetEntriesAsync(string? employeeId = null);
        Task AddTimesheetEntryAsync(TimesheetEntry entry);
        Task<bool> ApproveTimesheetAsync(int id, string managerId);
        Task<bool> RejectTimesheetAsync(int id, string managerId, string reason);

        Task<List<LeaveRequest>> GetRecentLeaveRequestsAsync(int count = 5);

    }
}
