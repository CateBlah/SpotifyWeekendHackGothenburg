using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaylistPool.Models;

namespace PlaylistPool.Repositories
{
    public interface IDatabaseRepository
    {
        Task SaveUserAsync(User user);
    }
}
