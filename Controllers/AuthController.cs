using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using viki_01.Entities;
using viki_01.Services;

namespace viki_01.Controllers
{
    [Route("api/Users")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private IAuthService authService;

        public AuthController(IAuthService authService)
        {
            this.authService = authService;
        }

        /// <summary>
        /// Creates a new user in the database
        /// </summary>
        /// <param name="credentials">User credentials</param>
        /// <returns></returns>
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public IActionResult Register([FromBody] RegistrationCredentials credentials)
        {
            try
            {
                var user = authService.Register(credentials);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new ErrorResponse(ex.Message));
            }
        }

        /// <summary>
        /// Authenticate a user with provided credentials
        /// </summary>
        /// <param name="credentials">User credentials</param>
        /// <returns>JWT and renewal token pair</returns>
        [HttpPost("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public ActionResult<AuthResponse> Authenticate([FromBody] SignInCredentials credentials)
        {
            var authResponse = authService.Authenticate(credentials);

            if (authResponse == null)
            {
                return BadRequest(new ErrorResponse("Invalid sign-in credentials!"));
            }

            return Ok(authResponse);
        }

        /// <summary>
        /// Renew JWT
        /// </summary>
        /// <param name="refreshToken">Renewal token</param>
        /// <returns>New JWT and renewal token pair</returns>
        [HttpGet("auth/refresh")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResponse))]
        public ActionResult<AuthResponse> RefreshToken([FromQuery(Name = "refresh-token")] string refreshToken)
        {
            var authResponse = authService.RenewToken(refreshToken);

            if (authResponse == null)
            {
                return BadRequest(new ErrorResponse("Invalid refresh token!"));
            }

            return Ok(authResponse);
        }

        /// <summary>
        /// Invalidates and removes renewal token from the db making the user unathenticated
        /// </summary>
        /// <param name="refreshToken"></param>
        /// <returns></returns>
        [HttpGet("logout")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Logout([FromQuery(Name = "refresh-token")] string refreshToken)
        {
            authService.Logout(refreshToken);
            return Ok();
        }
    }
}
