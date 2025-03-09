using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HEALTH_SUPPORT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountSurveyController : ControllerBase
    {
        private readonly IAccountSurveyService _accountSurveyService;

        public AccountSurveyController(IAccountSurveyService accountSurveyService)
        {
            _accountSurveyService = accountSurveyService;
        }

        [HttpGet("{accountId}", Name = "GetAccountSurveys")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetAccountSurveys(Guid accountId)
        {
            var result = await _accountSurveyService.GetAccountSurveys(accountId);
            return Ok(result);
        }

        [HttpGet("{surveyID}/accountSurvey", Name = "GetAccountSurveyById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetAccountSurveyById(Guid surveyID)
        {
            var result = await _accountSurveyService.GetAccountSurveyById(surveyID);
            if (result == null)
            {
                return NotFound(new { message = "AccountSurvey Not Found" });
            }
            return Ok(result);
        }

        [HttpPost(Name = "CreateAccountSurvey")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateAccountSurvey([FromBody] AccountSurveyRequest.CreateAccountSurveyModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                throw new Exception("Không tìm thấy người dùng.");
            }
            await _accountSurveyService.AddAccountSurvey(model);
            return CreatedAtRoute("GetAccountSurveyById", new { AccountSurveyId = /* newly created id */ Guid.NewGuid() }, new { message = "AccountSurvey created successfully" });
        }
        //Update AccountSurvey Type
        //[HttpPut("{AccountSurveyId}", Name = "UpdateAccountSurvey")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult> UpdateAccountSurvey(Guid AccountSurveyId, [FromBody] AccountSurveyRequest.UpdateAccountSurveyRequest model)
        //{
        //    if (model == null)
        //    {
        //        return BadRequest(new { message = "Invalid update data" });
        //    }
        //    await _accountSurveyService.UpdateAccountSurvey(AccountSurveyId, model);
        //    return Ok(new { message = "Create AccountSurvey Successfully" });
        //}

        [HttpDelete("{accountSurveyId}", Name = "DeleteAccountSurvey")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteAccountSurvey(Guid accountSurveyId)
        {
            var exstingAccountSurvey = await _accountSurveyService.GetAccountSurveyById(accountSurveyId);
            if (exstingAccountSurvey == null)
            {
                return NotFound(new { message = "AccountSurvey Not Found" });
            }
            await _accountSurveyService.RemoveAccountSurvey(accountSurveyId);
            return Ok(new { message = "Delete AccountSurvey Successfully" });
        }
    }
}
