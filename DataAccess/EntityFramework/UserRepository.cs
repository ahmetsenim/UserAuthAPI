using UserAuthAPI.DataAccess.Abstract;
using UserAuthAPI.Models.Concrete;

namespace UserAuthAPI.DataAccess.EntityFramework
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ProjectDbContext context) : base(context)
        {
        }

        //public List<OperationClaim> GetClaims(int userId)
        //{
        //    var result = (from user in _context.Users
        //                  join userGroup in _context.UserGroups on user.Id equals userGroup.UserId
        //                  join groupClaim in _context.GroupClaims on userGroup.GroupId equals groupClaim.GroupId
        //                  join operationClaim in _context.OperationClaims on groupClaim.ClaimId equals operationClaim.Id
        //                  where user.Id == userId
        //                  select new
        //                  {
        //                      operationClaim.Name
        //                  }).Union(from user in _context.Users
        //                           join userClaim in _context.UserClaims on user.Id equals userClaim.UserId
        //                           join operationClaim in _context.OperationClaims on userClaim.ClaimId equals operationClaim.Id
        //                           where user.Id == userId
        //                           select new
        //                           {
        //                               operationClaim.Name
        //                           });

        //    return result.Select(x => new OperationClaim { Name = x.Name }).Distinct().ToList();
        //}

    }
}
