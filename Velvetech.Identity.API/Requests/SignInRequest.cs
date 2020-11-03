using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Velvetech.Identity.API.Requests
{
    /// <summary>
    /// Запрос на регистрацию
    /// </summary>
    [PublicAPI]
    public class SignInRequest
    {
        /// <summary>
        /// Email
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Пароль
        /// </summary>
        [Required]
        [MinLength(8, ErrorMessage = "Некорректный пароль. Длина меньше 8 символов.")]
        public string Password { get; set; }
    }
}