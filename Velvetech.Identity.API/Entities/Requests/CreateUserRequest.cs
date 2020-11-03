using JetBrains.Annotations;

namespace Velvetech.Identity.API.Entities.Requests
{
    /// <summary>
    /// Запрос на создание пользователя
    /// </summary>
    [PublicAPI]
    public class CreateUserRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Login
        /// </summary>
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; set; }
    }
}