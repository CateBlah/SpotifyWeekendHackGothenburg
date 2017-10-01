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
var url = 'http://10f838bf.ngrok.io/api/user';

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
        req.session.currentUser = body.refresh_token;
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
          url: url
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
    url: url
  };

  request(options, function (err, response, body) {
    if (err) {
      console.error('error: ', err)
      throw err
    }
    var users = JSON.parse(response.body);
    
    var returnObject = {users: users, currentUser: req.session.currentUser}
    res.send(returnObject);
  })
})

app.post('api/spotify/generate-playlist', function (req, res) {

})

console.log('Listening on 8888');
app.listen(8888);
