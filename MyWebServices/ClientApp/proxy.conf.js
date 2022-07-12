const { env } = require('process');

const target = env.ASPNETCORE_HTTPS_PORT ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}` :
  env.ASPNETCORE_URLS ? env.ASPNETCORE_URLS.split(';')[0] : 'http://localhost:17811';

const PROXY_CONFIG = [
  {
    context: [
      "/api/v1/",
   ],
    target: target,
    secure: false
  }
]

module.exports = PROXY_CONFIG;
