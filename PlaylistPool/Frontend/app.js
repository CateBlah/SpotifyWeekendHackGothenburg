/**
 * This is an example of a basic node.js script that performs
 * the Authorization Code oAuth2 flow to authenticate against
 * the Spotify Accounts.
 *
 * For more information, read
 * https://developer.spotify.com/web-api/authorization-guide/#authorization_code_flow
 */

var express = require('express'); // Express web server framework
var cookieSession = require('cookie-session')
var request = require('request'); // "Request" library
var querystring = require('querystring');
var cookieParser = require('cookie-parser');

var client_id = '8725fac77956401eaff27f10b2058a85'; // Your client id
var client_secret = '2e89ce0618074fc59b24cbd0f045271a'; // Your secret
var redirect_uri = 'http://localhost:8888/callback'; // Your redirect uri

/**
 * Generates a random string containing numbers and letters
 * @param  {number} length The length of the string
 * @return {string} The generated string
 */
var generateRandomString = function (length) {
  var text = '';
  var possible = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

  for (var i = 0; i < length; i++) {
    text += possible.charAt(Math.floor(Math.random() * possible.length));
  }
  return text;
};

var stateKey = 'spotify_auth_state';

var app = express();
var getUserUrl = 'http://2cb61383.ngrok.io/api/spotify/get-users';
var postUserUrl = 'http://2cb61383.ngrok.io/api/user';

app.use(express.static(__dirname + '/public'))
  .use(cookieParser());
app.use(cookieSession({
  name: 'session',
  keys: ['test'],
}))

app.get('/login', function (req, res) {

  var state = generateRandomString(16);
  res.cookie(stateKey, state);

  // your application requests authorization
  var scope = 'user-read-private user-read-email user-top-read playlist-read-private user-read-recently-played playlist-modify-public';
  res.redirect('https://accounts.spotify.com/authorize?' +
    querystring.stringify({
      response_type: 'code',
      client_id: client_id,
      scope: scope,
      redirect_uri: redirect_uri,
      state: state
    }));
});

app.get('/callback', function (req, res) {
  var code = req.query.code || null;
  var state = req.query.state || null;
  var storedState = req.cookies ? req.cookies[stateKey] : null;

  if (state === null || state !== storedState) {
    res.redirect('/#' +
      querystring.stringify({
        error: 'state_mismatch'
      }));
  } else {
    res.clearCookie(stateKey);
    var authOptions = {
      url: 'https://accounts.spotify.com/api/token',
      form: {
        code: code,
        redirect_uri: redirect_uri,
        grant_type: 'authorization_code'
      },
      headers: {
        'Authorization': 'Basic ' + (new Buffer(client_id + ':' + client_secret).toString('base64'))
      },
      json: true
    };

    request.post(authOptions, function (error, response, body) {
      if (!error && response.statusCode === 200) {
        req.session.currentUser = body.refresh_token; //CurrentUser

        var access_token = body.access_token,
        refresh_token = body.refresh_token;
        
        var body2 = {
          userId: body.id,
          accessToken: access_token,
          refreshToken: refresh_token
        }
        
        var options = {
          method: 'post',
          body: body2,
          headers: {
            "Content-Type": "application/json"
          },
          json: true,
          url: postUserUrl
        }

        request(options, function (err, response, body) {
          if (err) {
            console.error('error posting json: ', err)
            throw err
          }
          res.redirect("main.html")
        })

      } else {
        res.redirect('/#' +
          querystring.stringify({
            error: 'invalid_token'
          }));
      }
    });
  }
});

app.get('/users', function (req, res) {
  var options = {
    method: 'get',
    url: getUserUrl
  };

  request(options, function (err, response, body) {
    if (err) {
      console.error('error: ', err)
      throw err
    }
    console.log(response);
    var users = JSON.parse(response.body);
    var returnObject = {users: users, currentUser: req.session.currentUser}
    res.send(returnObject);
  })
})

app.post('/createPlaylist', function(req, res) {
  var createPlaylistUrl = "http://2cb61383.ngrok.io/api/spotify/generate-playlist";

  req.body = [
    {
        "userId": "1992a929-9c47-4f2a-bee2-e2c8ca2cd1d9",
        "accessToken": "BQC42IR-ogJMFKOaeov6fcqY_5ulYdhaFlX4NQEA02h1nSOquz7VzTZAmR05xlMrabBGePt9ZU0fOz2UeJqQew33CMoeNT7BrGsaJ2TbbmC-hIMgm9QFMaq1nU-Z3o_ZG2UdWWig4tJl5yXqyhWZ_5WrlPYMIKNvc99Kyq3vG8k8fu2AxekA4IVqirok6TCL3zBA4ersdowkwty3QuNquJCGLAs_XrRDcnXE",
        "refreshToken": "AQCY_TkWwW8ScKbAB2ybkVxzWXN455Dw9Gq4kwD-BfedPqyUoEWbqsK-8WvFY7QiTO944YsqKV4AXMOTlvA8cxEysFiWfhQwi2Z9c5llveJl1x7OI1MigUKiiEXNuJK7FFs"
    },
    {
        "userId": "56bfb5d3-300a-41b2-8b48-087e76a71230",
        "accessToken": "BQC0UO7cMYON0LDmtLH2DVh-e-6MSwV5AuhjF8qEWi_OLC5lW-0OkuIt_CL_n44F2GvsexZ4Mo_dB-0zX_RelZAJqathga0Dkid_rc8gDBBpZLCpuy8XinMFf2DNr2iWUudKLrGxkS-m_VQoZ-aBenUHXE1ayeLySXaGmoJ8rqEt3VLJwcJuJInJxJN2eyVf",
        "refreshToken": "AQBq3_346SDyWxZVRfgITLmeWU_ggEp644vZ4H72_YfF5mHxsuYjrdfPge_LkVJfrR_MT2eRGQmumKMDHB0tMzGMTIW1YA4LpD14BsZEI-9Ky0QM4bODpbiVryXEda64K20"
    },
    {
        "userId": "b4ce46c5-1051-4f78-ad40-ca0b91b84356",
        "accessToken": "BQDdTuyw09gu-12NDd1J8gZN2DJNbCKgAWiNXAt5oXkvUGilGdXwGkeztCrLAf57XxfUcokxdEFvj_49QmRxkOI61i7fd3Sq-ouOsuLAldbwZwjTgQ4ib8hp_nVHR8mgiP-uEbAy8LvarbHR1CoZumfOwlfkQl_oiAUyYNd4ydBfoOIoJfGG_PMB",
        "refreshToken": "AQBkJEru6bbQPkxwQPtEgz-MR1WoxLimObG82ZPB_x7q3Ka0sEW3HXNiiuAcgGfsBvqauejMxBpA7caIUUGINxLWYjbWc0b2GfutRriCXHJTdP_cHArHXAqGLEWCvf3YOIE"
    }
]
  var options = {
    method: 'post',
    body: req.body,
    headers: {
      "Content-Type": "application/json"
    },
    json: true,
    url: createPlaylistUrl
  }

  request(options, function(req, response) {
    res.send(response.body);
  })
});

console.log('Listening on 8888');
app.listen(8888);
