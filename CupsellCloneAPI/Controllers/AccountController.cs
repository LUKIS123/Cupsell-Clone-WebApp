using CupsellCloneAPI.Authentication.Models;
using CupsellCloneAPI.Authentication.Services;
using CupsellCloneAPI.Core.Models.Dtos.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CupsellCloneAPI.Controllers
{
    [ApiController]
    [Route("cupsellclone/account")]
    public class AccountController : ControllerBase
    {
        // TODO: zrobic akcje dla modyfikacji danych konta oraz odzyskiwanie hasła

        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public async Task<ActionResult> Register([FromBody] RegisterUserDto dto)
        {
            await _accountService.RegisterUser(dto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticatedUserResponseDto>> Login([FromBody] LoginUserDto dto)
        {
            var response = await _accountService.LoginUser(dto);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("details")]
        public async Task<ActionResult<UserDto>> GetUserDetails()
        {
            var response = await _accountService.GetUserDetails();
            return Ok(response);
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<AuthenticatedUserResponseDto>> Refresh([FromBody] string refreshToken)
        {
            var response = await _accountService.RefreshUserJwt(refreshToken);
            return Ok(response);
        }

        [Authorize]
        [HttpDelete("logout")]
        public async Task<ActionResult> Logout()
        {
            await _accountService.DeleteUserRefreshTokens();
            return NoContent();
        }

        [Authorize]
        [HttpPost("resendVerificationEmail")]
        public async Task<ActionResult> ResendVerificationEmail()
        {
            await _accountService.ResendUserVerificationEmail();
            return Ok();
        }

        [HttpGet("verify")]
        public async Task<ActionResult> VerifyUserEmail([FromQuery] string encryptedToken)
        {
            await _accountService.VerifyUser(encryptedToken);
            return Ok("Success! You can now login to your account!");
        }
    }
}