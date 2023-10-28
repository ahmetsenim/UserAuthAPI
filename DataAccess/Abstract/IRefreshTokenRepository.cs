using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.Abstract
{
    public interface IRefreshTokenRepository : IGenericRepository<RefreshToken>
    {
    }
}
