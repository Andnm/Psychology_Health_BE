using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.IServices
{
    public interface ISurveyResultsService
    {
        Task<List<SurveyResultResponse.GetSurveyResultModel>> GetSurveyResults(Guid accountID);
        Task<SurveyResultResponse.GetSurveyResultModel?> GetSurveyResultById(Guid id);
        Task AddSurveyResult(Guid accountID, SurveyResultRequest.AddSurveyResultRequest model);
        Task UpdateSurveyResult(Guid id, SurveyResultRequest.UpdateSurveyResultRequest model);
        Task RemoveSurveyResult(Guid id);
    }
}
