using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.EntityFramework
{
    public class ResetPasswordOTPRepository : GenericRepository<ResetPasswordOTP>, IResetPasswordOTPRepository
    {
        public ResetPasswordOTPRepository(ProjectDbContext context) : base(context)
        {
        }
    }
}
