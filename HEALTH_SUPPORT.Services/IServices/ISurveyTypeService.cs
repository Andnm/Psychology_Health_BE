using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.IServices
{
    public interface ISurveyTypeService
    {
        Task<SurveyTypeResponse.GetSurveyTypeModel?> GetSurveyTypeById(Guid id);
        Task<List<SurveyTypeResponse.GetSurveyTypeModel>> GetSurveyTypes();
        Task AddSurveyType(SurveyTypeRequest.CreateSurveyTypeModel model);
        Task UpdateSurveyType(Guid id, SurveyTypeRequest.UpdateSurveyTypeModel model);
        Task RemoveSurveyType(Guid id);
    }
}