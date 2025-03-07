using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Repositories.Abstraction
{
    public interface IAuditable
    {
        DateTimeOffset CreateAt { get; set; }
        DateTimeOffset? ModifiedAt { get; set; }
    }
}
