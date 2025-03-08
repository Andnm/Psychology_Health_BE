using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HEALTH_SUPPORT.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SurveyQuestionController : ControllerBase
    {
        private readonly ISurveyQuestionService _SurveyQuestionService;

        public SurveyQuestionController(ISurveyQuestionService SurveyQuestionService)
        {
            _SurveyQuestionService = SurveyQuestionService;
        }

        [HttpGet(Name = "GetSurveyQuestions")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetSurveyQuestions()
        {
            var result = await _SurveyQuestionService.GetSurveyQuestions();
            return Ok(result);
        }

        [HttpGet("{SurveyQuestionId}", Name = "GetSurveyQuestionById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSurveyQuestionById(Guid SurveyQuestionId)
        {
            var result = await _SurveyQuestionService.GetSurveyQuestionById(SurveyQuestionId);
            if (result == null)
            {
                return NotFound(new { message = "SurveyQuestion Not Found" });
            }
            return Ok(result);
        }
        [HttpPost("{surveyId}", Name = "CreateSurveyQuestion")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateSurveyQuestion(Guid surveyId, [FromBody] List<SurveyQuestionRequest.CreateSurveyQuestionRequest> model)
        {
            await _SurveyQuestionService.AddSurveyQuestionForSurvey(surveyId, model);
            return CreatedAtRoute("GetSurveyQuestionById", new { SurveyQuestionId = /* newly created id */ Guid.NewGuid() }, new { message = "SurveyQuestion Type created successfully" });
        }
        //Update SurveyQuestion Type
        [HttpPut("{SurveyQuestionId}", Name = "UpdateSurveyQuestion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSurveyQuestion(Guid SurveyQuestionId, [FromBody] SurveyQuestionRequest.UpdateSurveyQuestionRequest model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid update data" });
            }
            await _SurveyQuestionService.UpdateSurveyQuestion(SurveyQuestionId, model);
            return Ok(new { message = "Create SurveyQuestion Successfully" });
        }

        [HttpDelete("{SurveyQuestionId}", Name = "DeleteSurveyQuestion")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSurveyQuestion(Guid SurveyQuestionId)
        {
            await _SurveyQuestionService.RemoveSurveyQuestion(SurveyQuestionId);
            return Ok(new { message = "Remove SurveyQuestion Successfully" });
        }
    }
}
