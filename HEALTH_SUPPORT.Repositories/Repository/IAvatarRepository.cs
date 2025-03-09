using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories.Repository
{
    public interface IAvatarRepository
    {
        Task UpdateAvatarAsync(Guid accountId, string avatarPath);
    }
}
