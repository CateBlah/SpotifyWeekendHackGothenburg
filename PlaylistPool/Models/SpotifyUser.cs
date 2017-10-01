﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlaylistPool.Models
{

    public class SpotifyUser
    {
        public string country { get; set; }
        public string display_name { get; set; }
        public string email { get; set; }
        public External_Urls external_urls { get; set; }
        public Followers followers { get; set; }
        public string href { get; set; }
        public string id { get; set; }
        public Image[] images { get; set; }
        public string product { get; set; }
        public string type { get; set; }
        public string uri { get; set; }
    }

    public class Followers
    {
        public string href { get; set; }
        public int total { get; set; }
    }

}
