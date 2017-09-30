using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlaylistPool.Models;

namespace PlaylistPool.DbContext
{
    public class UserDbContext : Microsoft.EntityFrameworkCore.DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options)
        { }

        public DbSet<User> Users { get; set; }
    }
}
