# config-server

Simple config management server with end-to-end encryption. All configs are encrypted using authenticated encryption. 

Features:
- Authenticated encryption using AES
    - KeySize = 256;
    - BlockSize = 128;
    - Mode = CBC;
    - Padding = PKCS7;
- Combination key consisting of
    - SQL row id column
    - hash of auth key 
    - encryption key
- Encryption key is never stored in the database
- Actual auth key never leaves the database, its hash is only sent over the wire
- Without having both 1) auth key and 2) encryption key it is impossible to decrypt. And this is called: "end-to-end encryption"

URL: https://simple-config-server.herokuapp.com/

```
// Create a fresh API key (or combination key)
[POST]  /api          => apiKey

// Retrieve the config given the API key
[GET]   /api/{apiKey} => config

// Update the config given the API key
[PUT]   /api/{apiKey} => config
```
NOTE: 
- Server is free to use. However that its hosted on free-tier heroku postgres so please be mindful.

Rules:
1. any config greater than 32KB is banned
2. any config not used in the last 30 days automatically will be deleted

Dependencies:
- .NET Core ^3.1
- [authenticated-encryption](https://github.com/trustpilot/nuget-authenticated-encryption)

TODO:
- add rate limiter
