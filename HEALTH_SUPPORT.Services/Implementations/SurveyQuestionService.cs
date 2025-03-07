using HEALTH_SUPPORT.Repositories.Entities;
using HEALTH_SUPPORT.Repositories.Repository;
using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.Implementations
{
    public class SurveyQuestionService : ISurveyQuestionService
    {
        private readonly IBaseRepository<Survey, Guid> _surveyRepository;
        private readonly IBaseRepository<SurveyQuestion, Guid> _surveyQuestionRepository;
        private readonly ISurveyAnswerService _surveyAnswerService;
        public SurveyQuestionService(IBaseRepository<Survey, Guid> surveyRepository, IBaseRepository<SurveyQuestion, Guid> surveyQuestionRepository, ISurveyAnswerService surveyAnswerService)
        {
            _surveyRepository = surveyRepository;
            _surveyQuestionRepository = surveyQuestionRepository;
            _surveyAnswerService = surveyAnswerService;
        }

        public async Task AddSurveyQuestionForSurvey(Guid surveyID, List<SurveyQuestionRequest.CreateSurveyQuestionRequest> model)
        {
            var survey = await _surveyRepository.GetById(surveyID);
            if (survey is null)
            {
                throw new Exception("Không tìm thấy bảng khảo sát.");
            }
            foreach (var surveyQuestion in model)
            {
                var question = new SurveyQuestion
                {
                    CreateAt = DateTime.Now,
                    Options = surveyQuestion.Options,
                    SurveyId = surveyID,
                    SurveyTypeId = survey.SurveyTypeId,
                    Validity = surveyQuestion.Validity,
                    ContentQ = surveyQuestion.ContentQ
                };
                await _surveyQuestionRepository.Add(question);
                if(surveyQuestion.AnswersList.Any())
                {
                    surveyQuestion.AnswersList.ForEach(answer => answer.QuestionId = question.Id);
                }
            }
            await _surveyQuestionRepository.SaveChangesAsync();
            var answerList = model
                .SelectMany(surveyQuestion => surveyQuestion.AnswersList)
                .ToList();

            if (answerList.Any())
            {
                await _surveyAnswerService.AddSurveyAnswerForSurveyQuestion(answerList);
            }
        }

        public async Task<SurveyQuestionResponse.GetSurveyQuestionModel?> GetSurveyQuestionById(Guid id)
        {
            var question = await _surveyQuestionRepository.GetById(id);
            if (question is null)
            {
                throw new Exception("Không tìm thấy câu hỏi.");
            }
            return new SurveyQuestionResponse.GetSurveyQuestionModel
            {
                Id = id,
                ContentQ = question.ContentQ,
                CreateAt = question.CreateAt,
                ModifiedAt = question.ModifiedAt,
                Options = question.Options,
                Validity = question.Validity,
                IsDelete = question.IsDeleted
            };
        }

        public Task<List<SurveyQuestionResponse.GetSurveyQuestionModel>> GetSurveyQuestions()
        {
            throw new NotImplementedException();
        }

        public async Task<List<SurveyQuestionResponse.GetSurveyQuestionModel>> GetSurveyQuestionsForSurvey(Guid surveyId)
        {
            var questionList = await _surveyQuestionRepository.GetAll().Where(s => s.SurveyId == surveyId).Select(s => new SurveyQuestionResponse.GetSurveyQuestionModel
            {
                Id= s.Id,
                SurveyId = s.SurveyId,
                ContentQ = s.ContentQ,
                CreateAt = s.CreateAt,
                Options= s.Options,
                ModifiedAt= s.ModifiedAt,
                Validity= s.Validity,
                IsDelete= s.IsDeleted
            }).ToListAsync();

            var answerList = await _surveyAnswerService.GetSurveyAnswerForQuestion(questionList.Select(s => s.Id).ToList());

            foreach (var item in questionList)
            {
                var answer = answerList.Where(s => s.QuestionId == item.Id).ToList();
                if (answer.Any())
                {
                    item.AnswerList.AddRange(answer);
                }
            }

            return questionList;
        }

        public async Task RemoveSurveyQuestion(Guid id)
        {
            var question = await _surveyQuestionRepository.GetById(id);
            if (question is null)
            {
                throw new Exception("Không tìm thấy câu hỏi.");
            }
            question.IsDeleted = true;
            question.ModifiedAt = DateTime.Now;
            await _surveyQuestionRepository.Update(question);
            await _surveyQuestionRepository.SaveChangesAsync();
        }

        public async Task UpdateSurveyQuestion(Guid id, SurveyQuestionRequest.UpdateSurveyQuestionRequest model)
        {
            var question = await _surveyQuestionRepository.GetById(id);
            if (question is null)
            {
                throw new Exception("Không tìm thấy câu hỏi.");
            }
            question.ContentQ = model.ContentQ;
            question.IsDeleted = model.IsDelete.HasValue ? model.IsDelete.Value : question.IsDeleted;
            question.ModifiedAt = DateTime.Now;
            question.Options = model.Options;
            question.Validity = model.Validity;
            await _surveyQuestionRepository.Update(question);
            await _surveyQuestionRepository.SaveChangesAsync();
        }
    }
}
