using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Velvetech.Identity.API.Infrastructure
{
    /// <summary>
    /// Класс расширений для <see cref="IDistributedCache"/>
    /// </summary>
    public static class DistributedCacheExtensions
    {
        /// <summary>
        /// Поместить объект в кэш
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="cache">Интерфейс распределённого кэша</param>
        /// <param name="key">Ключ</param>
        /// <param name="value">Помещаемый объект</param>
        /// <param name="expiration">Время истечения кэша относительно текущего момента</param>
        public static void SetObject<T>(this IDistributedCache cache, [NotNull] string key, [NotNull] T value,
            TimeSpan expiration)
        {
            var json = JsonConvert.SerializeObject(value);
            cache.SetString(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
        }

        /// <summary>
        /// Поместить объект в кэш
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="cache">Интерфейс распределённого кэша</param>
        /// <param name="key">Ключ</param>
        /// <param name="value">Помещаемый объект</param>
        /// <param name="expiration">Абсолютная дата истечения срока действия для записи кэша</param>
        public static void SetObject<T>(this IDistributedCache cache, [NotNull] string key, [NotNull] T value,
            DateTime expiration)
        {
            var json = JsonConvert.SerializeObject(value);
            cache.SetString(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expiration
            });
        }

        /// <summary>
        /// Поместить объект в кэш
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="cache">Интерфейс распределённого кэша</param>
        /// <param name="key">Ключ</param>
        /// <param name="value">Помещаемый объект</param>
        /// <param name="cacheOptions">Опции распределённого кэша</param>
        public static void SetObject<T>(this IDistributedCache cache, [NotNull] string key, [NotNull] T value,
            DistributedCacheEntryOptions cacheOptions)
        {
            var json = JsonConvert.SerializeObject(value);
            cache.SetString(key, json, cacheOptions);
        }

        /// <summary>
        /// Получить объект из кэша
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="cache">Интерфейс распределённого кэша</param>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        [CanBeNull]
        public static T GetObject<T>(this IDistributedCache cache, [NotNull] string key)
        {
            var json = cache.GetString(key);
            return json != null ? JsonConvert.DeserializeObject<T>(json) : default(T);
        }

        /// <summary>
        /// Поместить объект в кэш
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="cache">Интерфейс распределённого кэша</param>
        /// <param name="key">Ключ</param>
        /// <param name="value">Помещаемый объект</param>
        /// <param name="expiration">Время истечения кэша относительно текущего момента</param>
        public static async Task SetObjectAsync<T>(this IDistributedCache cache, [NotNull] string key,
            [NotNull] T value, TimeSpan expiration)
        {
            var json = JsonConvert.SerializeObject(value);
            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
        }

        /// <summary>
        /// Поместить объект в кэш
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="cache">Интерфейс распределённого кэша</param>
        /// <param name="key">Ключ</param>
        /// <param name="value">Помещаемый объект</param>
        /// <param name="expiration">Абсолютная дата истечения срока действия для записи кэша</param>
        public static async Task SetObjectAsync<T>(this IDistributedCache cache, [NotNull] string key,
            [NotNull] T value, DateTime expiration)
        {
            var json = JsonConvert.SerializeObject(value);
            await cache.SetStringAsync(key, json, new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = expiration
            });
        }

        /// <summary>
        /// Поместить объект в кэш
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="cache">Интерфейс распределённого кэша</param>
        /// <param name="key">Ключ</param>
        /// <param name="value">Помещаемый объект</param>
        /// <param name="cacheOptions">Опции распределённого кэша</param>
        public static async Task SetObjectAsync<T>(this IDistributedCache cache, [NotNull] string key,
            [NotNull] T value, DistributedCacheEntryOptions cacheOptions)
        {
            var json = JsonConvert.SerializeObject(value);
            await cache.SetStringAsync(key, json, cacheOptions);
        }

        /// <summary>
        /// Получить объект из кэша
        /// </summary>
        /// <typeparam name="T">Тип объекта</typeparam>
        /// <param name="cache">Интерфейс распределённого кэша</param>
        /// <param name="key">Ключ</param>
        /// <returns></returns>
        [CanBeNull]
        public static async Task<T> GetObjectAsync<T>(this IDistributedCache cache, [NotNull] string key)
        {
            var json = await cache.GetStringAsync(key);
            return json != null ? JsonConvert.DeserializeObject<T>(json) : default(T);
        }
    }
}