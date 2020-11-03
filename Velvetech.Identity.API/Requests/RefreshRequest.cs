using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace Velvetech.Identity.API.Requests
{
    /// <summary>
    /// Запрос на обновление токена
    /// </summary>
    [PublicAPI]
    public class RefreshRequest
    {
        /// <summary>
        /// Токен для обновления
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }
}