using System;
using JetBrains.Annotations;

namespace Velvetech.Identity.API.Identity
{
    /// <summary>
    /// Интерфейс для работы с RefreshToken
    /// </summary>
    public interface IRefreshTokenService
    {
        /// <summary>
        /// Установить refreshToken для логина пользователя
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <param name="login"></param>
        /// <param name="cacheExpiration"></param>
        void SetRefreshToken([NotNull] string refreshToken, [NotNull] string login, TimeSpan cacheExpiration);

        /// <summary>
        /// Проверить, что refreshToken есть для логина пользователя
        /// </summary>
        /// <param name="login"></param>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        bool ValidRefreshToken([NotNull] string login, [NotNull] string refreshToken);

        /// <summary>
        /// Удаление токена 
        /// </summary>
        /// <param name="login"></param>
        void DeleteRefreshToken([NotNull] string login);
    }
}