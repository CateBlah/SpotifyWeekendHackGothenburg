using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PlaylistPool.Models;

namespace PlaylistPool
{
    public class SpotifyConnector : ISpotifyConnector
    {
        string client_id = "8725fac77956401eaff27f10b2058a85";
        string client_secret = "2e89ce0618074fc59b24cbd0f045271a";

        public async Task<ResponseObject> GetTopArtistsAndTracks(User user, bool tracks)
        {
            var tracksOrArtists = tracks ? "tracks" : "artists";
            using (var httpClient = await CreateHttpClientAsync(user))
            {
                using (var response = await httpClient.GetAsync($"me/top/{tracksOrArtists}?limit=50"))
                {
                    response.EnsureSuccessStatusCode();
                    var responseBody = await response.Content.ReadAsStringAsync();
                    var deserializedResponseBody = JsonConvert.DeserializeObject<ResponseObject>(responseBody);
                    return deserializedResponseBody;
                }
            }
        }

        private async Task<HttpClient> CreateHttpClientAsync(User user)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.spotify.com/v1/"),
            };

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", await GetOauthTokenAsync(user));
            ////httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user.AccessToken);

            return httpClient;
        }

        public async Task<string> GetOauthTokenAsync(User user)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://accounts.spotify.com/"),
            };

            var authorizationString = $"{client_id}:{client_secret}";
            var authorizationStringBytes = Encoding.UTF8.GetBytes(authorizationString);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue($"Basic", $"{Convert.ToBase64String(authorizationStringBytes)}");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "refresh_token"),
                new KeyValuePair<string, string>("refresh_token", user.RefreshToken),
            });

            using (var response = await httpClient.PostAsync($"api/token", content))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonConvert.DeserializeObject<TokenResponse>(responseBody);
                return deserializedContent.access_token;
            }
        }

        public async Task<SpotifyUser> GetUser(string authToken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.spotify.com/"),
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            using (var response = await httpClient.GetAsync($"v1/me"))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonConvert.DeserializeObject<SpotifyUser>(responseBody);
                return deserializedContent;
            }

        }

        public async Task<SpotifyPlaylist> CreatePlaylistAsync(string currentUserName, string authToken, string playListName)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.spotify.com/"),
            };


            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
            var contentDictionary = new Dictionary<string, string>
            {
                { "name", $"{playListName} {Guid.NewGuid().ToString()}"}
            };
            var content = new StringContent(JsonConvert.SerializeObject(contentDictionary), Encoding.UTF8, "application/json");
            using (var response = await httpClient.PostAsync($"v1/users/{currentUserName}/playlists/", content))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonConvert.DeserializeObject<SpotifyPlaylist>(responseBody);
                return deserializedContent;
            }
        }

        public async Task AddTracksToPlaylist(List<string> topTracks, string currentUser, SpotifyPlaylist playlist, string authToken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.spotify.com/"),
            };

            var urlFix = "";
            foreach (var topTrack in topTracks)
            {
                if(topTrack == topTracks.First())
                {
                    urlFix += "uris=";
                }
                urlFix += $"{topTrack}";
                if (topTrack == topTracks.Last()) continue;
                urlFix += ",";
            }

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            HttpContent content = null;
            using (var response = await httpClient.PostAsync($"v1/users/{currentUser}/playlists/{playlist.id}/tracks?{urlFix}", content))
            {
                response.EnsureSuccessStatusCode();
            }

            // POST https://api.spotify.com/v1/users/{user_id}/playlists/{playlist_id}/tracks
        }

        public async Task<AudioFeature> GetAudioFeatures(IEnumerable<Item> tracks, string authToken)
        {
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.spotify.com/"),
            };

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);

            var urlFix = "";
            foreach (var track in tracks)
            {
                if (track == tracks.First())
                {
                    urlFix += "ids=";
                }
                urlFix += $"{track.id}";
                if (track == tracks.Last()) continue;
                urlFix += ",";
            }

            using (var response = await httpClient.GetAsync($"v1/audio-features?{urlFix}"))
            {
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadAsStringAsync();
                var deserializedContent = JsonConvert.DeserializeObject<AudioFeature>(responseBody);
                return deserializedContent;
            }
        }
    }
}
