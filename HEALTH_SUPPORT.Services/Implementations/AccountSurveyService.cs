using HEALTH_SUPPORT.Repositories.Entities;
using HEALTH_SUPPORT.Repositories.Repository;
using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.Implementations
{
    public class AccountSurveyService : IAccountSurveyService
    {
        private readonly IBaseRepository<Survey, Guid> _surveyRepository;
        private readonly IBaseRepository<AccountSurvey, Guid> _accountSurveyRepository;
        private readonly IBaseRepository<SurveyResults, Guid> _surveyResultRepository;

        public AccountSurveyService(IBaseRepository<Survey, Guid> surveyRepository, IBaseRepository<AccountSurvey, Guid> accountSurveyRepository, IBaseRepository<SurveyResults, Guid> surveyResultRepository)
        {
            _surveyRepository = surveyRepository;
            _accountSurveyRepository = accountSurveyRepository;
            _surveyResultRepository = surveyResultRepository;
        }

        public async Task AddAccountSurvey(AccountSurveyRequest.CreateAccountSurveyModel model)
        {
            var accountSurvey = new AccountSurvey
            {
                AccountId = model.AccountId,
                SurveyId = model.SurveyId,
                CreateAt = DateTime.Now,
            };
            await _accountSurveyRepository.Add(accountSurvey);
            await _surveyRepository.SaveChangesAsync();
        }

        public async Task<AccountSurveyResponse.GetAccountSurveysModel?> GetAccountSurveyById(Guid id)
        {
            var accountSurvey = await _accountSurveyRepository.GetById(id);
            if (accountSurvey is null)
            {
                throw new Exception("Không tìm thấy khảo sát.");
            }
            var surveyResult = _surveyResultRepository.GetAll().Where(s => s.AccountSurveyId == accountSurvey.Id).Select(s => new SurveyResultResponse.GetSurveyResultModel
            {
                SurveyId = s.SurveyId,
                AccountSurveyId = s.AccountSurveyId,
                CreateAt = s.CreateAt,
                ResultDescription = s.ResultDescription,
                ModifiedAt = s.ModifiedAt,
                Score = s.Score,
                Id = s.Id,
                IsDelete = s.IsDeleted
            }).ToList();
            return new AccountSurveyResponse.GetAccountSurveysModel
            {
                CreateAt = accountSurvey.CreateAt,
                AccountId = accountSurvey.AccountId,
                ModifiedAt = accountSurvey.ModifiedAt,
                SurveyId = accountSurvey.SurveyId,
                Id = id,
                IsDelete = accountSurvey.IsDeleted,
                ResultList = surveyResult != null ? surveyResult : null
            };
        }

        public async Task<List<AccountSurveyResponse.GetAccountSurveysModel>> GetAccountSurveys(Guid userId)
        {
            var accountSurveys = _accountSurveyRepository.GetAll().Where(s => s.AccountId == userId)
                .Select(s => new AccountSurveyResponse.GetAccountSurveysModel
                {
                    Id = s.Id,
                    AccountId = s.AccountId,
                    CreateAt = s.CreateAt,
                    ModifiedAt = s.ModifiedAt,
                    SurveyId = s.SurveyId,
                    IsDelete = s.IsDeleted
                }).ToList();
            return accountSurveys;
        }

        public async Task RemoveAccountSurvey(Guid id)
        {
            var accountSurvey = await _accountSurveyRepository.GetById(id);
            if (accountSurvey is null)
            {
                throw new Exception("Không tìm thấy khảo sát.");
            }
            accountSurvey.IsDeleted = true;
            accountSurvey.ModifiedAt = DateTime.Now;
        }
    }
}
