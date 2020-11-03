using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Velvetech.Identity.API.Data;
using Velvetech.Identity.API.Entities;
using Velvetech.Identity.API.Entities.Requests;
using Velvetech.Identity.API.Identity;
using Velvetech.Identity.API.Requests;

namespace Velvetech.Identity.API.Controllers
{
    /// <summary>
    /// Идентификация
    /// </summary>
    [ApiController]
    [Produces("application/json")]
    [Route("/[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly IdentityDbContext _dbContext;

        public IdentityController(
            ITokenService tokenService,
            IdentityDbContext dbContext,
            IMapper mapper,
            ILogger<IdentityController> logger
        )
        {
            _tokenService = tokenService;
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Регистрация
        /// </summary>
        /// <param name="request">Запрос на регистрацию</param>
        /// <returns></returns>
        [HttpPost("SignIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> SignIn(SignInRequest request)
        {
            if (await CheckUserExist(request.Email)) return BadRequest("login занят");

            var userCreateRequest = _mapper.Map<CreateUserRequest>(request);

            var user = Entities.User.Create(userCreateRequest);
            await _dbContext.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            return "Регистрация прошла успешно";
        }

        /// <summary>
        /// Вход
        /// </summary>
        /// <param name="request">Запрос для входа</param>
        /// <returns></returns>
        [HttpPost("LogIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Token>> LogIn(LoginRequest request)
        {
            var identity = await GetIdentity(request.Login, request.Password);

            if (identity == null) return BadRequest("Invalid username or password.");

            var token = _tokenService.GetToken(identity);

            _tokenService.AddRefreshToken(request.Login, token.RefreshToken);

            return token;
        }

        /// <summary>
        /// Обновить токен
        /// </summary>
        /// <param name="request">Запрос на обновление токена из refresh token</param>
        /// <returns></returns>
        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<Token>> RefreshToken(RefreshRequest request)
        {
            var refreshTokenOld = new JwtSecurityToken(request.RefreshToken);

            // Refresh токен истек
            //
            if (DateTime.UtcNow > refreshTokenOld.ValidTo)
            {
                _logger.LogWarning("Refresh token истек");
                return Unauthorized("Refresh token expired");
            }

            var claimLogin = GetClaimLogin(refreshTokenOld);

            // Нет идентификационных данных в токене для логина пользователя
            //
            if (claimLogin == null)
            {
                _logger.LogWarning("Невалидный refresh token");
                return BadRequest("Invalid refresh token.");
            }

            var login = claimLogin.Value;

            var identity = await GetIdentity(login);

            // Идентификационные данные не найдены
            //
            if (identity == null) return BadRequest("Invalid refresh token.");

            if (!_tokenService.ValidRefreshToken(login, request.RefreshToken))
            {
                _logger.LogWarning($"Для логина {login} не совпадает RefreshToken");
                return Unauthorized();
            }

            var token = _tokenService.GetToken(identity);

            _tokenService.UpdateRefreshToken(login, token.RefreshToken);

            return token;
        }

        /// <summary>
        /// Получить идентификационные данные для логина
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        private static Claim GetClaimLogin(JwtSecurityToken refreshToken)
        {
            return refreshToken.Claims.FirstOrDefault(x => x.Type == ClaimsIdentity.DefaultNameClaimType);
        }

        /// <summary>
        /// Получить идентификационные данные пользователя
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private UserIdentity GetUserIdentity(User user)
        {
            return _mapper.Map<UserIdentity>(user);
        }

        /// <summary>
        /// Получить идентификационные данные
        /// </summary>
        /// <param name="login">Логин</param>
        /// <returns></returns>
        private async Task<ClaimsIdentity> GetIdentity(string login)
        {
            var user = await _dbContext.Users
                .Where(x => x.Login == login)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning($"Пользователь не найден {login}");
                return null;
            }

            var userIdentity = GetUserIdentity(user);

            return GetClaimsIdentity(userIdentity);
        }

        /// <summary>
        /// Получить идентификационные данные
        /// </summary>
        /// <param name="login">Логин</param>
        /// <param name="password">Пароль</param>
        /// <returns></returns>
        private async Task<ClaimsIdentity> GetIdentity(string login, string password)
        {
            // TODO: добавить шифрование пароля 
            var hashPassword = password;

            var user = await _dbContext.Users
                .Where(x => x.Login == login)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                _logger.LogWarning($"Пользователь не найден {login}");
                return null;
            }

            if (!user.CheckPassword(hashPassword))
            {
                _logger.LogWarning($"Неверный логин {login} или пароль");
                return null;
            }

            var userIdentity = GetUserIdentity(user);

            return GetClaimsIdentity(userIdentity);
        }

        /// <summary>
        /// Получить идентификационные данные
        /// </summary>
        /// <param name="userIdentity">Пользователь</param>
        /// <returns></returns>
        private static ClaimsIdentity GetClaimsIdentity([NotNull] UserIdentity userIdentity)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, userIdentity.Login)
                // Для ролей
                // new Claim(ClaimsIdentity.DefaultRoleClaimType, userIdentity.Role)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token",
                ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            return claimsIdentity;
        }

        /// <summary>
        /// Проверить, что пользователь существует
        /// </summary>
        /// <param name="login"></param>
        /// <returns></returns>
        private async Task<bool> CheckUserExist([NotNull] string login)
        {
            var user = await _dbContext.Users
                .Where(x => x.Login == login)
                .FirstOrDefaultAsync();
            return user != null;
        }
    }

    /// <summary>
    /// Идентификационные данные пользователя 
    /// </summary>
    [PublicAPI]
    public class UserIdentity
    {
        /// <summary>
        /// Логин
        /// </summary>
        public string Login { get; set; }
    }
}