using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.EntityFramework
{
    public class RefreshTokenRepository : GenericRepository<RefreshToken>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
