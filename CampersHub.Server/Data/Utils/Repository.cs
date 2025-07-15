using Microsoft.EntityFrameworkCore;

namespace CampersHub.Server.Data.Utils
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly CampersHubDbContext _context;
        private readonly DbSet<T> _dbSet;

        public Repository(CampersHubDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        T IRepository<T>.Create(T entity)
        {
            _dbSet.Add(entity);
            _context.SaveChanges();
            return entity;
        }

        bool IRepository<T>.Delete(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity != null)
            {
                _dbSet.Remove(entity);
                _context.SaveChanges();
                return true;
            }
            return false;
        }

        T IRepository<T>.Get(int id)
        {
            return _dbSet.Find(id);
        }

        IEnumerable<T> IRepository<T>.GetAll()
        {
            return _dbSet.ToList();
        }

        bool IRepository<T>.Update(T entity)
        {
            try
            {
                _dbSet.Update(entity);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
