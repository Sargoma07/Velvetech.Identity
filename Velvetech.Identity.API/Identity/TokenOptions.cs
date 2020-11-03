using System.Text;
using JetBrains.Annotations;
using Microsoft.IdentityModel.Tokens;

namespace Velvetech.Identity.API.Identity
{
    /// <summary>
    /// Опции для JWT токена авторизации
    /// </summary>
    [PublicAPI]
    public class TokenOptions
    {
        /// <summary>
        /// Издатель токена
        /// </summary>
        public string Issuer { get; set; }

        /// <summary>
        /// Потребитель токена
        /// </summary>
        public string Audience { get; set; }

        /// <summary>
        /// Refresh ключ.
        /// <remarks>Ключ должен быть больше 15 символов</remarks>
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Время истечения token в секундах
        /// </summary>
        public int LifetimeSec { get; set; }

        /// <summary>
        /// Время истечения refresh token в секундах.
        /// </summary>
        public int RefreshLifetimeSec { get; set; }

        /// <summary>
        /// Refresh ключ.
        /// <remarks>Ключ должен быть больше 15 символов</remarks>
        /// </summary>
        public string RefreshKey { get; set; }

        /// <summary>
        /// Получить ключ для шифрования access token
        /// </summary>
        /// <returns></returns>
        public SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Key));
        }

        /// <summary>
        /// Получить ключ для шифрования refresh token
        /// </summary>
        /// <returns></returns>
        public SymmetricSecurityKey GetSymmetricSecurityKeyForRefresh()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(RefreshKey));
        }
    }
}