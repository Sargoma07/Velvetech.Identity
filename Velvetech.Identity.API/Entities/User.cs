using System;
using JetBrains.Annotations;
using Velvetech.Identity.API.Entities.Requests;

namespace Velvetech.Identity.API.Entities
{
    /// <summary>
    /// Пользователь
    /// </summary>
    public class User 
    {
        private User()
        {
        }

        public long Id { get; set; }

        public DateTime CreatedDate { get; private set; }

        public DateTime? UpdatedDate { get; private set; }

        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; private set; }

        /// <summary>
        /// Пароль
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// e-mail
        /// </summary>
        public string Email { get; private set; }

        /// <summary>
        /// Проверка пароля
        /// </summary>
        /// <param name="password">Пароль для проверки</param>
        /// <returns></returns>
        public bool CheckPassword([NotNull] string password) => Password == password;

        /// <summary>
        /// Создание пользователя 
        /// </summary>
        /// <param name="request">Запрос на создание пользователя</param>
        /// <returns></returns>
        public static User Create([NotNull] CreateUserRequest request)
        {
            return new User
            {
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow,
                Login = request.Login,
                Password = request.Password,
                Email = request.Email
            };
        }
    }
}