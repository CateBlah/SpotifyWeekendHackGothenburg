using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task SaveUserAsync(User user)
        {
            _dbContext.Add(user);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _dbContext.Users.ToListAsync();
        }
    }
}
