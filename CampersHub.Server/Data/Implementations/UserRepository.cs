using CampersHub.Server.Data.Interfaces;
using CampersHub.Server.Data.Utils;
using CampersHub.Server.Domain.Models;

namespace CampersHub.Server.Data.Implementations
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly CampersHubDbContext _context;

        public UserRepository(CampersHubDbContext context) : base(context)
        {
            _context = context;
        }

        public User GetByUsername(string username)
        {
            return _context.Users.FirstOrDefault(u => u.Username == username);
        }
    }
}
