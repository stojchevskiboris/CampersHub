using CampersHub.Server.Data.Interfaces;
using CampersHub.Server.Data.Utils;
using CampersHub.Server.Domain.Models;

namespace CampersHub.Server.Data.Implementations
{
    public class MediaRepository : Repository<Media>, IMediaRepository
    {
        private readonly CampersHubDbContext _context;

        public MediaRepository(CampersHubDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
