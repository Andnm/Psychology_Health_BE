using HEALTH_SUPPORT.Repositories.Entities;
using HEALTH_SUPPORT.Repositories.Repository;
using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.Implementations
{
    public class SurveyService : ISurveyService
    {
        private readonly IBaseRepository<Survey, Guid> _surveyRepository;
        private readonly ISurveyQuestionService _surveyQuestionService;
        private readonly ISurveyAnswerService _surveyAnswerService;
        private readonly IAccountSurveyService _accountSurveyService;

        public SurveyService(IBaseRepository<Survey, Guid> surveyRepository, ISurveyQuestionService surveyQuestionService, ISurveyAnswerService surveyAnswerService, IAccountSurveyService accountSurveyService)
        {
            _surveyRepository = surveyRepository;
            _surveyQuestionService = surveyQuestionService;
            _surveyAnswerService = surveyAnswerService;
            _accountSurveyService = accountSurveyService;
        }

        public async Task AddSurvey(string userID, SurveyRequest.CreateSurveyRequest model)
        {
            Survey survey = new Survey
            {
                CreateAt = DateTime.Now,
                MaxScore = model.MaxScore,
                SurveyTypeId = model.SurveyTypeId,
            };
            await _surveyRepository.Add(survey);
            await _surveyRepository.SaveChangesAsync();

            //AccountSurvey
            var accountSurvey = new AccountSurveyRequest.CreateAccountSurveyModel
            {
                AccountId = Guid.Parse(userID),
                SurveyId = survey.Id
            };
            await _accountSurveyService.AddAccountSurvey(accountSurvey);

            if (model.QuestionList.Any())
            {
                await _surveyQuestionService.AddSurveyQuestionForSurvey(survey.Id, model.QuestionList);
            }
        }

        public async Task<SurveyResponse.GetSurveyDetailsModel?> GetSurveyById(Guid id)
        {
            var survey = await _surveyRepository.GetById(id);
            if (survey is null)
            {
                throw new Exception("Không tìm thấy bảng khảo sát");
            }

            var questionList = await _surveyQuestionService.GetSurveyQuestionsForSurvey(id);
            return new SurveyResponse.GetSurveyDetailsModel
            {
                Id = id,
                ModifiedAt = survey.ModifiedAt,
                CreateAt = survey.CreateAt,
                IsDeleted = survey.IsDeleted,
                MaxScore = survey.MaxScore,
                QuestionList = questionList
            };
        }

        public Task<List<SurveyResponse.GetSurveyModel>> GetSurveys()
        {
            throw new NotImplementedException();
        }

        public async Task RemoveSurvey(Guid id)
        {
            var survey = await _surveyRepository.GetById(id);
            if (survey is null)
            {
                throw new Exception("Không tìm thấy bảng khảo sát");
            }
            survey.IsDeleted = true;
            survey.ModifiedAt = DateTime.Now;
        }

        public async Task UpdateSurvey(Guid id, SurveyRequest.UpdateSurveyRequest model)
        {
            var survey = await _surveyRepository.GetById(id);
            if (survey is null)
            {
                throw new Exception("Không tìm thấy bảng khảo sát");
            }
            survey.IsDeleted = model.IsDelete.HasValue ? model.IsDelete.Value : survey.IsDeleted;
            survey.MaxScore = model.MaxScore;
            survey.ModifiedAt = DateTime.Now;
            await _surveyRepository.Update(survey);
            await _surveyRepository.SaveChangesAsync();
        }
    }
}
