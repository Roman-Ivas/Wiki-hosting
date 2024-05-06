using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using viki_01.Contexts;
using viki_01.Entities;
using viki_01.Extensions;
using viki_01.Models.Dto;
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
        
        [HttpGet("GetMyId")]
        [Authorize]
        public IActionResult GetMyId()
        {
            return Ok(HttpContext.User.GetId());
        }
        
        [HttpGet("profile/{userId:int?}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Profile([FromServices] WikiHostingSqlServerContext context, [FromRoute] int? userId, [FromQuery] int relatedContentLoadLimit = 20)
        {
            if (userId.HasValue)
            {
                var userProfile = await context.Users
                    .Where(u => u.Id == userId)
                    .Select(u =>
                        new UserProfileDto
                        {
                            Id = u.Id,
                            UserName = u.UserName!,
                            AvatarPath = u.AvatarPath,
                            Comments = u.Comments.Take(relatedContentLoadLimit),
                            Contributions = u.Contributions.Take(relatedContentLoadLimit),
                            CreatedPages = u.CreatedPages.Take(relatedContentLoadLimit),
                            CreatedRatings = u.CreatedRatings.Take(relatedContentLoadLimit),
                            CreatedReports = u.CreatedReports.Take(relatedContentLoadLimit),
                            CreatedTemplates = u.CreatedTemplates.Take(relatedContentLoadLimit),
                            CreatedThemes = u.CreatedThemes.Take(relatedContentLoadLimit),
                            Feedbacks = u.Feedbacks.Take(relatedContentLoadLimit),
                            InterestedTopics = u.InterestedTopics.Take(relatedContentLoadLimit),
                            Subscription = u.Subscription.Subscription,
                            LockoutEnabled = u.LockoutEnabled
                        }
                    )
                    .FirstOrDefaultAsync();
                
                if (userProfile is null)
                {
                    return NotFound();
                }
                
                return Ok(userProfile);
            }

            try
            {
                var userOwnProfile = await context.Users
                    .Where(u => u.Id == HttpContext.User.GetId())
                    .Select(u => new
                    {
                        Id = u.Id,
                        UserName = u.UserName!,
                        Email = u.Email!,
                        Password = u.Password,
                        PhoneNumber = u.PhoneNumber,
                        AvatarPath = u.AvatarPath,
                        Preference = u.Preference,
                        Comments = u.Comments.Take(relatedContentLoadLimit),
                        Contributions = u.Contributions.Take(relatedContentLoadLimit),
                        CreatedPages = u.CreatedPages.Take(relatedContentLoadLimit),
                        CreatedRatings = u.CreatedRatings.Take(relatedContentLoadLimit),
                        CreatedReports = u.CreatedReports.Take(relatedContentLoadLimit),
                        CreatedTemplates = u.CreatedTemplates.Take(relatedContentLoadLimit),
                        CreatedThemes = u.CreatedThemes.Take(relatedContentLoadLimit),
                        Feedbacks = u.Feedbacks.Take(relatedContentLoadLimit),
                        InterestedTopics = u.InterestedTopics.Take(relatedContentLoadLimit),
                        Subscription = u.Subscription.Subscription,
                        LockoutEnabled = u.LockoutEnabled
                    })
                    .FirstOrDefaultAsync();
                
                if (userOwnProfile is null)
                {
                    return NotFound();
                }
                
                return Ok(userOwnProfile);
            }
            catch (Exception)
            {
                return Unauthorized();
            }
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
