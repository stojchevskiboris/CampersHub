namespace CampersHub.Server.Services.ViewModels.Users
{
    public class UserRegisterModel
    {
        public int? Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConfirmPassword { get; set; }

        public string DateOfBirth { get; set; }

        public string? Phone { get; set; }
    }
}
