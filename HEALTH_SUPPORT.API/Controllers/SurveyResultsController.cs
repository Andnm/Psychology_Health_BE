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
    public class SurveyResultsController : ControllerBase
    {
        private readonly ISurveyResultsService _surveyResultsService;

        public SurveyResultsController(ISurveyResultsService SurveyResultsService)
        {
            _surveyResultsService = SurveyResultsService;
        }

        [HttpGet("{accountId}", Name = "GetSurveyResultss")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetSurveyResults(Guid accountId)
        {
            var result = await _surveyResultsService.GetSurveyResults(accountId);
            return Ok(result);
        }

        [HttpGet("{surveyID}/SurveyResults", Name = "GetSurveyResultsById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSurveyResultsById(Guid surveyID)
        {
            var result = await _surveyResultsService.GetSurveyResultById(surveyID);
            if (result == null)
            {
                return NotFound(new { message = "SurveyResults Not Found" });
            }
            return Ok(result);
        }

        [Authorize]
        [HttpPost(Name = "SubmitSurvey")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateSurveyResults([FromBody] SurveyResultRequest.AddSurveyResultRequest model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null)
            {
                throw new Exception("Không tìm thấy người dùng.");
            }
            await _surveyResultsService.AddSurveyResult(Guid.Parse(userId), model);
            return Ok(new { message = "Create SurveyResults Successfully" });
        }

        [HttpPut("{SurveyResultsId}", Name = "UpdateSurveyResults")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSurveyResults(Guid SurveyResultsId, [FromBody] SurveyResultRequest.UpdateSurveyResultRequest model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid update data" });
            }
            await _surveyResultsService.UpdateSurveyResult(SurveyResultsId, model);
            return Ok(new { message = "Update SurveyResults Successfully" });
        }

        [HttpDelete("{SurveyResultsId}", Name = "DeleteSurveyResults")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSurveyResults(Guid SurveyResultsId)
        {
            var exstingSurveyResults = await _surveyResultsService.GetSurveyResultById(SurveyResultsId);
            if (exstingSurveyResults == null)
            {
                return NotFound(new { message = "SurveyResults Not Found" });
            }
            await _surveyResultsService.RemoveSurveyResult(SurveyResultsId);
            return Ok(new { message = "Delete SurveyResults Successfully" });
        }
    }
}
