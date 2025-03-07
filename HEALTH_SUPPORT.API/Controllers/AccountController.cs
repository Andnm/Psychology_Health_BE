using Azure.Core;
using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HEALTH_SUPPORT.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;

        public AccountController(IAccountService accountService, IEmailService emailService, IMemoryCache cache)
        {
            _accountService = accountService;
            _emailService = emailService;
            _cache = cache;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountRequest.CreateAccountModel model)
        {
            await _accountService.AddAccount(model);
            _emailService.GenerateOtp(model.Email);
            return Ok(new { message = "Tạo tài khoản thành công, vui lòng xác thực email bằng OTP!" });
        }

        [HttpPost("otp")]
        public IActionResult VerifyOtp([FromBody] AccountRequest.OtpRequest request)
        {
            var isValid = _emailService.VerifyOtp(request.Email, request.Otp);
            if (!isValid)
            {
                return BadRequest(new { message = "OTP không hợp lệ hoặc đã hết hạn!" });
            }

            return Ok(new { message = "Xác thực OTP thành công!" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] AccountRequest.LoginRequestModel model)
        {
            var loginResult = await _accountService.ValidateLoginAsync(model);
            if (loginResult == null)
            {
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng" });
            }
            if (!_cache.TryGetValue($"OTP_Verified_{model.Email}", out bool isVerified) || !isVerified)
            {
                return BadRequest(new { message = "Bạn cần xác thực OTP trước khi đăng nhập." });
            }
            var token = _accountService.GenerateJwtToken(loginResult);

            return Ok(new { accessToken = token });
        }

        [HttpGet(Name = "GetAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAccounts()
        {
            var result = await _accountService.GetAccounts();
            return Ok(result);
        }

        [HttpGet("{accountId}", Name = "GetAccountById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAccountById(Guid accountId)
        {
            var result = await _accountService.GetAccountById(accountId);
            if(result == null)
            {
                return NotFound(new { message = "Account not found" });
            }
            return Ok(result);
        }

        [HttpPut("{accountId}", Name = "UpdateAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateAccount(Guid accountId, [FromBody] AccountRequest.UpdateAccountModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid update data" });
            }

            var existingAccount = await _accountService.GetAccountById(accountId);
            if (existingAccount == null)
            {
                return NotFound(new { message = "Account not found" });
            }

            await _accountService.UpdateAccount(accountId, model);
            return Ok(new { message = "Account updated successfully" });
        }

        [HttpDelete("{accountId}", Name = "DeleteAccount")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAccount(Guid accountId)
        {
            var existingAccount = await _accountService.GetAccountById(accountId);
            if (existingAccount == null)
            {
                return NotFound(new { message = "Account not found" });
            }
            await _accountService.RemoveAccount(accountId);
            return Ok(new { message = "Account deleted successfully" });
        }
    }
}
