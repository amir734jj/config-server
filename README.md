# config-server

Simple config management server

Server is free to use. However that its hosted on free-tier heroku postgres so please be mindful.

URL: https://simple-config-server.herokuapp.com/

```
// Create a fresh apiKey
[POST]  /api          => apiKey

// Retrieve the config
[GET]   /api/{apiKey} => config

// Update the config
[PUT]   /api/{apiKey} => config

```

Features:
- any config greater than or equal to 32KB is banned
- any config not used in the last 30 days automatically will be deleted

TODO:
- add rate limiter
