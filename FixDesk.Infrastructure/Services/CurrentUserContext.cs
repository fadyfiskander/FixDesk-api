using FixDesk.Application.Common.Interfaces;
using FixDesk.Domain.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using FixDesk.Infrastructure.Identity.Infrastructure.Authentication;

namespace Infrastructure.Identity
{
    public class CurrentUserContext : ICurrentUserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
    
        public Guid UserId
        {
            get
            {
                var id = User?.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                         User?.FindFirstValue(ClaimNames.UserId) ??
                         User?.FindFirstValue(ClaimTypes.NameIdentifier);

                return Guid.TryParse(id, out var userId) ? userId : Guid.Empty;
            }
        }

        public UserRole Role
        {
            get
            {
                var val = User?.FindFirstValue(ClaimNames.UserRole) ?? User?.FindFirstValue("organization_role");
                if (val == null)
                    throw new ArgumentException("Organization role is not found.");
                Enum.TryParse<UserRole>(val, out var role);
                return role;
            }
        }

        public bool IsOwner => Role == UserRole.Owner;

    }
}
