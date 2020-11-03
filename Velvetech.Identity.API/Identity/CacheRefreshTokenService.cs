using System;
using Microsoft.Extensions.Caching.Distributed;
using Velvetech.Identity.API.Infrastructure;

namespace Velvetech.Identity.API.Identity
{
    /// <summary>
    /// Сервис для работы с RefreshToken в кэш
    /// </summary>
    public class CacheRefreshTokenService : IRefreshTokenService
    {
        private readonly IDistributedCache _cache;

        private const string CacheKey = "RefreshToken";

        public CacheRefreshTokenService(IDistributedCache cache)
        {
            _cache = cache;
        }

        /// <inheritdoc />
        public void SetRefreshToken(string refreshToken, string login, TimeSpan cacheExpiration)
        {
            var cacheKey = GetCacheKey(login);
            _cache.SetObject(cacheKey, refreshToken, cacheExpiration);
        }

        /// <inheritdoc />
        public bool ValidRefreshToken(string login, string refreshToken)
        {
            var cacheKey = GetCacheKey(login);
            var cacheRefreshToken = _cache.GetObject<string>(cacheKey);
            return cacheRefreshToken != null && cacheRefreshToken == refreshToken;
        }

        /// <inheritdoc />
        public void DeleteRefreshToken(string login)
        {
            var cacheKey = GetCacheKey(login);
            _cache.Remove(cacheKey);
        }

        /// <summary>
        /// Получить ключ для кэша
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private static string GetCacheKey(string login)
        {
            return CacheKey + login;
        }
    }
}