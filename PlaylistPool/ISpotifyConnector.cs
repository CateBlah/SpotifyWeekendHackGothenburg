using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlaylistPool.Models;

namespace PlaylistPool
{
    public interface ISpotifyConnector
    {
        Task<ResponseObject> GetTopArtistsAndTracks(User user, bool tracks);

        Task<string> GetOauthTokenAsync(User user);

        Task<SpotifyUser> GetUser(string authToken);

        Task<SpotifyPlaylist> CreatePlaylistAsync(string currentUserName, string authToken, string playListName);

        Task AddTracksToPlaylist(List<string> topTracks, string currentUset, SpotifyPlaylist playlist, string authToken);
    }
}
