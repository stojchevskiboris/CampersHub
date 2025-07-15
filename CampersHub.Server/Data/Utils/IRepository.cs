namespace CampersHub.Server.Data.Utils
{
    public interface IRepository<T> where T : class
    {
        T Get(int id);
        IEnumerable<T> GetAll();
        T Create(T entity);
        bool Update(T entity);
        bool Delete(int id);
    }
}
