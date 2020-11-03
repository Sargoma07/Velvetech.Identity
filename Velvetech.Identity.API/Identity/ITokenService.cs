using System.Security.Claims;
using JetBrains.Annotations;

namespace Velvetech.Identity.API.Identity
{
    /// <summary>
    /// Интерфейс сервис для работы с авторизационным токеном JWT
    /// </summary>
    public interface ITokenService
    {
        /// <summary>
        /// Получить токен
        /// </summary>
        /// <param name="claimsIdentity">Идентификационные данные</param>
        /// <returns>Токен</returns>
        [NotNull]
        Token GetToken([NotNull] ClaimsIdentity claimsIdentity);

        /// <summary>
        /// Проверить refresh token для пользователя
        /// </summary>
        /// <param name="login">login</param>
        /// <param name="refreshTokenForCheck">refresh token для проверки</param>
        /// <returns>true-валидный,false - невалидный</returns>
        bool ValidRefreshToken([NotNull] string login, [NotNull] string refreshTokenForCheck);

        /// <summary>
        /// Добавить refresh token пользователю
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="refreshToken">refresh token</param>
        void AddRefreshToken([NotNull] string login, [NotNull] string refreshToken);

        /// <summary>
        /// Обновить refresh token пользователю
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="refreshToken">refresh token</param>
        void UpdateRefreshToken([NotNull] string login, [NotNull] string refreshToken);
    }
}