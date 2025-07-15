using CampersHub.Server.Services.Interfaces;
using CampersHub.Server.Services.ViewModels.Users;
using Microsoft.AspNetCore.Mvc;

namespace CampersHub.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }


        [HttpGet("GetC")]
        public int GetC()
        {
            return 210;
        }

        [HttpGet("GetS")]
        public List<UserViewModel> GetS()
        {
            return _userService.GetAllUsers();
        }
    }
}
