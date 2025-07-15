
using CampersHub.Server.Configs.Authentication;
using CampersHub.Server.Services.ViewModels.Users;

namespace CampersHub.Server.Services.Interfaces
{
    public interface IUserService
    {
        #region UserServices
        List<UserViewModel> GetAllUsers();
        UserViewModel GetUserById(int id);
        UserViewModel GetCurrentUserDetails();
        UserViewModel? GetCurrentUserDetailsOrDefault(string id);
        UserViewModel GetUserByUsername(string username);
        bool CheckUsername(string username);
        UserViewModel CreateUser(UserRegisterModel model);
        UserViewModel UpdateUser(UserViewModel user);
        bool DeleteUser(int id);
        bool ChangePassword(PasswordViewModel user);
        #endregion

        #region AuthenticationServices
        AuthenticateResponse Authenticate(AuthenticateRequest model);
        AuthenticateResponse AuthenticateWithJwt(string token);
        #endregion
    }
}
