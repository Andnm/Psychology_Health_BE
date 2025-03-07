using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using Microsoft.AspNetCore.Mvc;

namespace HEALTH_SUPPORT.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SurveyTypeController : ControllerBase
    {
        private readonly ISurveyTypeService _surveyTypeService;

        public SurveyTypeController(ISurveyTypeService surveyTypeService)
        {
            _surveyTypeService = surveyTypeService;
        }

        [HttpGet(Name = "GetSurveyTypes")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> GetSurveyTypes()
        {
            var result = await _surveyTypeService.GetSurveyTypes();
            return Ok(result);
        }

        [HttpGet("{surveyTypeId}", Name = "GetSurveyTypeById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> GetSurveyTypeById (Guid surveyTypeId)
        {
            var result = await _surveyTypeService.GetSurveyTypeById(surveyTypeId);
            if (result == null)
            {
                return NotFound(new { message = "Survey Type not found" });
            }
            return Ok(result);
        }

        [HttpPost(Name = "CreateSurveyType")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult> CreateSurveyType([FromBody] SurveyTypeRequest.CreateSurveyTypeModel model)
        {
            await _surveyTypeService.AddSurveyType(model);
            return CreatedAtRoute("GetSurveyTypeById", new { surveyTypeId = /* newly created id */ Guid.NewGuid() }, new { message = "Survey Type created successfully" });
        }
        //Update Survey Type
        [HttpPut("{surveyTypeId}", Name = "UpdateSurveyType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateSurveyType(Guid surveyTypeId, [FromBody] SurveyTypeRequest.UpdateSurveyTypeModel model)
        {
            if (model == null)
            {
                return BadRequest(new { message = "Invalid update data" });
            }
            // check survey type exist
            var exstingSurveyType=await _surveyTypeService.GetSurveyTypeById(surveyTypeId);
            if (exstingSurveyType == null)
            {
                return NotFound(new { message = "Survey Type not found" });
            }

            await _surveyTypeService.UpdateSurveyType(surveyTypeId, model);
            return Ok(new { message = "Survey Type successfully" });
        }

        [HttpDelete("{surveyTypeId}", Name = "DeleteSurveyType")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteSurveyType(Guid surveyTypeId)
        {
            var exstingSurveyType = await _surveyTypeService.GetSurveyTypeById(surveyTypeId);
            if (exstingSurveyType == null)
            {
                return NotFound(new { message = "Survey Type not found" });
            }
            await _surveyTypeService.RemoveSurveyType(surveyTypeId);
            return Ok(new { message = "Survey Type successfully" });
        }   
    }
}
