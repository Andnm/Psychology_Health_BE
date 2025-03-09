using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories.Repository
{
    public class AvatarRepository : IAvatarRepository
    {
        private readonly ApplicationDbContext _context;

        public AvatarRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAvatarAsync(Guid accountId, string avatarPath)
        {
            var account = await _context.Accounts.FindAsync(accountId);
            if (account != null)
            {
                account.ImgUrl = avatarPath;
                await _context.SaveChangesAsync();
            }
        }
    }
}
