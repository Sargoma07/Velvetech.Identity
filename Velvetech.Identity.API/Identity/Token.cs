using JetBrains.Annotations;

namespace Velvetech.Identity.API.Identity
{
    /// <summary>
    /// Токен
    /// </summary>
    [PublicAPI]
    public class Token
    {
        /// <summary>
        /// Токен для доступа
        /// </summary>
        public string AccessToken { get; set; }

        /// <summary>
        /// Токен для обновления 
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// Время истечения токена доступа
        /// </summary>
        public long Expires { get; set; }
    }
}