using CampersHub.Server.Common.Exceptions;
using CampersHub.Server.Common.Helpers;
using CampersHub.Server.Configs.Authentication;
using CampersHub.Server.Data.Interfaces;
using CampersHub.Server.Domain.Models;
using CampersHub.Server.Services.Interfaces;
using CampersHub.Server.Services.Mappers;
using CampersHub.Server.Services.ViewModels.Users;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CampersHub.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMediaRepository _mediaRepository;
        private readonly AppSettings _appSettings;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(
            IUserRepository userRepository,
            IMediaRepository mediaRepository,
            IOptions<AppSettings> appSettings,
            IHttpContextAccessor httpContextAccessor)
        {
            _userRepository = userRepository;
            _mediaRepository = mediaRepository;
            _appSettings = appSettings.Value;
            _httpContextAccessor = httpContextAccessor;
        }


        public List<UserViewModel> GetAllUsers()
        {
            return _userRepository.GetAll()
                .ToList()
                .MapToViewModelList();
        }

        public UserViewModel GetUserById(int id)
        {
            var user = GetUserDomainById(id);

            return user.MapToViewModel();
        }

        public UserViewModel GetCurrentUserDetails()
        {
            var currentUserId = Context.GetCurrentUserId();
            var user = GetUserDomainById(currentUserId);

            return user.MapToViewModel();
        }

        public UserViewModel? GetCurrentUserDetailsOrDefault(string userId)
        {
            try
            {
                int id = int.Parse(userId.Trim());
                var user = GetUserDomainById(id);
                return user.MapToViewModel();
            }
            catch (Exception ex)
            {
                return null;
            }
        }    
       
        public UserViewModel GetUserByUsername(string username)
        {
            var user = _userRepository.GetByUsername(username.Trim());
            if (user == null)
            {
                throw new CustomException($"No user found with username: {username}");
            }

            return user.MapToViewModel();
        }

        
        

        public bool CheckUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return false;
            }

            var allUsernames = _userRepository.GetAll().Select(x => x.Username.ToLower()).ToList();

            foreach (var existingUsername in allUsernames)
            {
                if (existingUsername == username.ToLower())
                {
                    return false;
                }
            }

            return true;
        }

        public UserViewModel CreateUser(UserRegisterModel model)
        {
            if (_userRepository.GetAll().Where(x => x.Username == model.Username).Any())
            {
                throw new CustomException("There is already registered user with the provided username");
            }

            if (model.Password != model.ConfirmPassword)
            {
                throw new CustomException("Passwords must match");
            }

            User user = new User();
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Username = model.Username;
            user.Password = PasswordHelper.HashPassword(PasswordHelper.DecryptString(model.Password));
            user.DateOfBirth = DateTime.Parse(model.DateOfBirth);
            user.CreatedAt = DateTime.UtcNow;
            user.ModifiedAt = DateTime.UtcNow;

            var newUser = _userRepository.Create(user);
            return newUser.MapToViewModel();
        }

        public UserViewModel UpdateUser(UserViewModel model)
        {
            var currentUserId = Context.GetCurrentUserId();
            var user = GetUserDomainById(currentUserId);

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.DateOfBirth = model.DateOfBirth;
            user.ModifiedAt = DateTime.UtcNow;

            _userRepository.Update(user);

            return user.MapToViewModel();
        }

        public bool DeleteUser(int id)
        {
            var user = GetUserDomainById(id);
            _userRepository.Delete(id);

            user = _userRepository.Get(id);
            if (user == null)
            {
                return true;
            }

            return false;
        }

        public bool ChangePassword(PasswordViewModel model)
        {
            if (model == null)
            {
                throw new CustomException("Invalid parameters");
            }

            if (string.IsNullOrEmpty(model.OldPassword)
                || string.IsNullOrEmpty(model.NewPassword)
                || string.IsNullOrEmpty(model.ConfirmPassword))
            {
                throw new CustomException("Invalid parameters");
            }

            var currentUserId = Context.GetCurrentUserId();

            var user = GetUserDomainById(currentUserId);

            var hashedOldPassword = user.Password;
            var hashedNewPassword = PasswordHelper.HashPassword(PasswordHelper.DecryptString(model.NewPassword));

            if (PasswordHelper.HashPassword(PasswordHelper.DecryptString(model.OldPassword)) != hashedOldPassword)
            {
                throw new CustomException("Old password is not correct!");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                throw new CustomException("Passwords do not match!");
            }

            if (hashedOldPassword == hashedNewPassword)
            {
                throw new CustomException("New password cannot be same as old password!");
            }

            //if (!PasswordHelper.CheckPasswordStrength(model.NewPassword))
            //{
            //    throw new CustomException("Passwords must contain at least 8 characters!");
            //}

            user.Password = hashedNewPassword;
            _userRepository.Update(user);

            return true;
        }

        public bool UpdateProfilePicture(string imageUrl, long fileLength, string contentType)
        {
            try
            {
                var currentUserId = Context.GetCurrentUserId();
                var user = GetUserDomainById(currentUserId);

                Media media = new Media()
                {
                    Url = imageUrl,
                    FileType = contentType,
                    FileSize = (int)fileLength,
                    CreatedAt = DateTime.UtcNow,
                    ModifiedAt = DateTime.UtcNow,
                };

                _mediaRepository.Create(media);

                _userRepository.Update(user);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model)
        {
            var user = _userRepository.GetByUsername(model.Username);
            if (user == null)
            {
                return null; // Nonexisting Username
            }

            var checkPassword = PasswordHelper.HashPassword(PasswordHelper.DecryptString(model.Password));
            if (checkPassword != user.Password)
            {
                return null; // Incorrect passowrd
            }

            var token = GenerateJwtToken(user);

            LogUserDetails(user.Id);

            return new AuthenticateResponse(user, token);
        }

        public AuthenticateResponse AuthenticateWithJwt(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new CustomException("Session expired, please log in again.");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            try
            {
                // Validate the token
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero // No delay for token expiration
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(claim => claim.Type == "id").Value);

                // Get the user by ID
                var user = _userRepository.Get(userId);
                if (user == null)
                {
                    throw new CustomException("User not found");
                }

                // Generate a new JWT token for the response
                var newToken = GenerateJwtToken(user);

                LogUserDetails(userId);

                return new AuthenticateResponse(user, newToken);
            }
            catch (Exception)
            {
                return null;
                //throw new CustomException("Session expired, please log in again.");
            }
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        private User GetUserDomainById(int id)
        {
            var user = _userRepository.Get(id);
            if (user == null)
            {
                throw new CustomException($"No user found with id: {id}");
            }

            return user;
        }

        private void LogUserDetails(int userId)
        {
            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString();
            Log.Information("User authenticated, UserId: {UserId}, IP: {IP}", userId, ip ?? "Unknown");
        }
    }
}
