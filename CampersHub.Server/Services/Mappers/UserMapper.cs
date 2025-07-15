using CampersHub.Server.Domain.Models;
using CampersHub.Server.Services.ViewModels.Users;

namespace CampersHub.Server.Services.Mappers
{
    public static class UserMapper
    {
        public static UserViewModel MapToViewModel(this User user)
        {
            if (user == null)
                return null;

            var model = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                DateOfBirth = user.DateOfBirth,
                CreatedAt = user.CreatedAt,
                ModifiedAt = user.ModifiedAt
            };

            return model;
        }

        public static List<UserViewModel> MapToViewModelList(this List<User> users)
        {
            return users.Select(x => x.MapToViewModel()).ToList();
        }
    }
}