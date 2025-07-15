using CampersHub.Server.Data.Interfaces;
using CampersHub.Server.Data.Utils;
using CampersHub.Server.Domain.Models;

namespace CampersHub.Server.Data.Implementations
{
    public class PostRepository : Repository<Post>, IPostRepository
    {
        private readonly CampersHubDbContext _context;

        public PostRepository(CampersHubDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
