using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlaylistPool.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PlaylistPool.Controllers
{
    [Route("api/spotify")]
    public class SpotifyController : Controller
    {
        private readonly ISpotifyConnector _spotifyConnector;
        public SpotifyController(ISpotifyConnector spotifyConnector)
        {
            _spotifyConnector = spotifyConnector;
        }

        [HttpPost("generate-playlist")]
        public async Task GeneratePlaylistAsync([FromBody]IEnumerable<User> users)
        {
            var topTracksCollections = new List<ResponseObject>();

            var currentUserAuthToken = await _spotifyConnector.GetOauthTokenAsync(users.First());
            var currentUserInfo = await  _spotifyConnector.GetUser(currentUserAuthToken);
            var currentUserName = currentUserInfo.id;

            foreach (var user in users)
            {
                topTracksCollections.Add(await _spotifyConnector.GetTopArtistsAndTracks(user, tracks: true));
            }

            var topTracks = topTracksCollections.SelectMany(x => x.items)
                .Select(x => x.uri)
                .OrderBy(a => Guid.NewGuid())
                .Take(20)
                .ToList();

            var createdPlaylist = await _spotifyConnector.CreatePlaylistAsync(currentUserName, currentUserAuthToken);
            await _spotifyConnector.AddTracksToPlaylist(topTracks, currentUserName, createdPlaylist, currentUserAuthToken);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
