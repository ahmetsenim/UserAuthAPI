using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.Abstract
{
    public interface IUserRepository : IGenericRepository<User>
    {
        List<OperationClaim> GetClaims(int userId);
    }
}
