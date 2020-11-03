using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Velvetech.Identity.API.Identity
{
    /// <summary>
    /// Сервис для работы с авторизационным токеном JWT
    /// </summary>
    public class TokenService : ITokenService
    {
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly TokenOptions _tokenOptions;

        public TokenService(IRefreshTokenService refreshTokenService, IOptionsMonitor<TokenOptions> tokenOptionsMonitor)
        {
            _refreshTokenService = refreshTokenService;
            _tokenOptions = tokenOptionsMonitor.CurrentValue;
        }

        /// <inheritdoc />
        public Token GetToken(ClaimsIdentity claimsIdentity)
        {
            var now = DateTime.UtcNow;
            var expiresAccessToken = now.Add(GetExpirationAccessToken());
            var expiresRefreshToken = now.Add(GetExpirationRefreshToken());

            var accessToken = GetAccessToken(claimsIdentity, now, expiresAccessToken);

            var refreshToken = GetRefreshToken(claimsIdentity, now, expiresRefreshToken);

            var expiresAccessTokenUnixTime = new DateTimeOffset(expiresAccessToken).ToUnixTimeSeconds();

            var token = new Token
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                Expires = expiresAccessTokenUnixTime
            };
            return token;
        }

        /// <inheritdoc />
        public bool ValidRefreshToken(string login, string refreshTokenForCheck)
        {
            return _refreshTokenService.ValidRefreshToken(login, refreshTokenForCheck);
        }

        /// <inheritdoc />
        public void AddRefreshToken(string login, string refreshToken)
        {
            _refreshTokenService.DeleteRefreshToken(login);

            _refreshTokenService.SetRefreshToken(refreshToken, login, GetExpirationRefreshToken());
        }

        /// <inheritdoc />
        public void UpdateRefreshToken(string login, string refreshToken)
        {
            _refreshTokenService.DeleteRefreshToken(login);

            _refreshTokenService.SetRefreshToken(refreshToken, login, GetExpirationRefreshToken());
        }

        /// <summary>
        /// Получить авторизационный токен
        /// </summary>
        /// <param name="claimsIdentity">Идентификационные данные</param>
        /// <param name="dateFrom">Дата начала действия токена</param>
        /// <param name="dateExpires">Дата истечения токена</param>
        /// <returns></returns>
        private string GetAccessToken(ClaimsIdentity claimsIdentity, DateTime dateFrom, DateTime dateExpires)
        {
            // создаем JWT-токен
            var jwt = new JwtSecurityToken(
                _tokenOptions.Issuer,
                _tokenOptions.Audience,
                notBefore: dateFrom,
                claims: claimsIdentity.Claims,
                expires: dateExpires,
                signingCredentials: new SigningCredentials(
                    _tokenOptions.GetSymmetricSecurityKey(),
                    SecurityAlgorithms.HmacSha256));

            var accessToken = new JwtSecurityTokenHandler().WriteToken(jwt);
            return accessToken;
        }

        /// <summary>
        /// Получить токен для обновления токена авторизации
        /// </summary>
        /// <param name="claimsIdentity">Идентификационные данные</param>
        /// <param name="dateFrom">Дата начала действия токена</param>
        /// <param name="dateExpires">Дата истечения токена</param>
        /// <returns></returns>
        private string GetRefreshToken(ClaimsIdentity claimsIdentity, DateTime dateFrom, DateTime dateExpires)
        {
            // создаем JWT Refresh-токен
            var jwtRefresh = new JwtSecurityToken(
                _tokenOptions.Issuer,
                _tokenOptions.Audience,
                notBefore: dateFrom,
                claims: claimsIdentity.Claims,
                expires: dateExpires,
                signingCredentials: new SigningCredentials(_tokenOptions.GetSymmetricSecurityKeyForRefresh(),
                    SecurityAlgorithms.HmacSha256));

            var refreshToken = new JwtSecurityTokenHandler().WriteToken(jwtRefresh);
            return refreshToken;
        }

        /// <summary>
        /// Получить время истечения авторизационного токена
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetExpirationAccessToken()
        {
            return TimeSpan.FromSeconds(_tokenOptions.LifetimeSec);
        }

        /// <summary>
        /// Получить время истечения refresh token
        /// </summary>
        /// <returns></returns>
        private TimeSpan GetExpirationRefreshToken()
        {
            return TimeSpan.FromSeconds(_tokenOptions.RefreshLifetimeSec);
        }
    }
}