using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.EntityFramework
{
    public class OTPRepository : GenericRepository<OTP>, IOTPRepository
    {
        public OTPRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
