using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TestAbsa.Data; // adjust namespace
using TestAbsa.Services;

namespace TestAbsa.Services
{
    public interface IUserContext
    {
        string? UserId { get; }
        int? OrganizationId { get; }

        Task<int?> GetOrganizationIdAsync();

    }

    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _dbContext;

        private int? _organizationId;

        public UserContext(IHttpContextAccessor httpContextAccessor, ApplicationDbContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        public string? UserId =>
            _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        public int? OrganizationId
        {
            get
            {
                if (_organizationId != null)
                    return _organizationId;

                var user = _dbContext.Users
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Id == UserId);

                _organizationId = user?.OrganizationId;
                return _organizationId;
            }
        }

        public async Task<int?> GetOrganizationIdAsync()
        {
            if (_organizationId != null)
                return _organizationId;

            var userId = UserId;
            if (string.IsNullOrEmpty(userId))
                return null;

            var user = await _dbContext.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == userId);

            _organizationId = user?.OrganizationId;
            return _organizationId;
        }


    }
}
