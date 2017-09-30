using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistPool.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string AccessToken { get; set; }
    }
}
