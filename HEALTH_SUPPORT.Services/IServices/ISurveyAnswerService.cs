using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.IServices
{
    public interface ISurveyAnswerService
    {
        Task<List<SurveyAnswerResponse.GetSurveyAnswerModel>> GetSurveyAnswers();
        Task<SurveyAnswerResponse.GetSurveyAnswerModel?> GetSurveyAnswerById(Guid id);
        Task<List<SurveyAnswerResponse.GetSurveyAnswerModel?>> GetSurveyAnswerForQuestion(List<Guid> questionIds);
        Task AddSurveyAnswerForSurveyQuestion(List<SurveyAnswerRequest.CreateSurveyAnswerRequest> model);
        Task UpdateSurveyAnswer(Guid id, SurveyAnswerRequest.UpdateSurveyAnswerRequest model);
        Task RemoveSurveyAnswer(Guid id);
    }
}
