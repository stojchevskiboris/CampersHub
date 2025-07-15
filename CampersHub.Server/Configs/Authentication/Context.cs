using CampersHub.Server.Services.ViewModels.Users;

namespace CampersHub.Server.Configs.Authentication
{
    public static class Context
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public static int GetCurrentUserId()
        {
            if (_httpContextAccessor?.HttpContext?.Items["User"] is UserViewModel user)
            {
                return user.Id;
            }

            throw new Exception("User ID not found in the context.");
        }
    }
}
