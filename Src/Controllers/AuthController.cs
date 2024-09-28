using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using RestApi.Src.Dto;
using RestApi.Src.Services;
using RestApi.Src.Utils;
using RestApi.Src.Validations;
using RestApi.Src.Validations.Cmd;

namespace RestApi.Src.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserService userService;
        private readonly IMediator _mediator;
        private readonly PasswordHashing passwordHashing;
        private readonly JwtService jwtService;

        private DateTime GetExpirationDate() => DateTime.UtcNow.AddMinutes(30);

        public AuthController(IConfiguration _config, IMediator mediator)
        {
            userService = new(_config);
            jwtService = new(_config);
            _mediator = mediator;
            passwordHashing = new();
        }

        private void AppendCookie(string key, string value, DateTime expirationTime)
        {
            DeleteCookie(key);
            Response.Cookies.Append(
                key,
                value,
                new CookieOptions
                {
                    HttpOnly = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = expirationTime,
                }
            );
        }

        private void DeleteCookie(string key)
        {
            if (Request.Cookies.ContainsKey(key))
            {
                Response.Cookies.Delete(key);
            }
        }

        [HttpPost("register")]
        [Consumes("application/json")]
        public async Task<IActionResult> Register([FromBody] RegisterCmd req)
        {
            await _mediator.Send(req);
            int userId = -1;
            req.Passwd = passwordHashing.Hash(req.Passwd);

            try
            {
                userId = await userService.CreateUser(req);

                string token = jwtService.GenerateAccessToken(
                    new Dto.JwtClaimDto { UserId = userId, Email = req.Email },
                    GetExpirationDate()
                );
                AppendCookie("access_token", token, GetExpirationDate().AddMinutes(-1));

                return Ok(new { userId, email = req.Email });
            }
            catch (SqlException e)
            {
                if (e.Number == 2627)
                {
                    return Conflict(new { property = "email", message = "email already existed" });
                }
                throw;
            }
        }

        [HttpPost("login")]
        [Consumes("application/json")]
        public async Task<IActionResult> Login([FromBody] LoginCmd req)
        {
            await _mediator.Send(req);
            try
            {
                LoginDto result = await userService.GetUserForLogin(req);
                if (passwordHashing.Verify(req.Passwd, result.Passwd))
                {
                    string token = jwtService.GenerateAccessToken(
                        new Dto.JwtClaimDto { UserId = result.UserId, Email = result.Email },
                        GetExpirationDate()
                    );
                    AppendCookie("access_token", token, GetExpirationDate().AddMinutes(-1));
                    return Ok(new { userId = result.UserId });
                }
                return Unauthorized(string.Empty);
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(string.Empty);
            }
        }

        [HttpGet("logout")]
        public IActionResult Logout()
        {
            DeleteCookie("access_token");
            return Ok();
        }

        [Authorize]
        [HttpGet("verify-login")]
        public IActionResult VerifyLogin()
        {
            return Ok();
        }
    }
}
