using CampersHub.Server.Data.Utils;
using CampersHub.Server.Domain.Models;

namespace CampersHub.Server.Data.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        User GetByUsername(string username);
    }
}
