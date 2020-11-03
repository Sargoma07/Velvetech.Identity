using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Velvetech.Identity.API.Requests
{
    /// <summary>
    /// Запрос на вход
    /// </summary>
    [PublicAPI]
    public class LoginRequest
    {
        /// <summary>
        /// Логин
        /// </summary>
        [Required]
        public string Login { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [MinLength(8, ErrorMessage = "Некорректный пароль. Длина меньше 8 символов.")]
        public string Password { get; set; }
    }
}