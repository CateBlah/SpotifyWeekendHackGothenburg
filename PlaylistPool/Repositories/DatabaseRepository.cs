using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaylistPool.DbContext;
using PlaylistPool.Models;

namespace PlaylistPool.Repositories
{
    public class DatabaseRepository : IDatabaseRepository
    {
        private readonly UserDbContext _dbContext;
        public DatabaseRepository(UserDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void SaveUser(User user)
        {
            _dbContext.Add(user);
        }
    }
}
