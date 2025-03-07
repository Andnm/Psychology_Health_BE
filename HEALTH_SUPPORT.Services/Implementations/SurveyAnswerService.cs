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
    public class SurveyAnswerService : ISurveyAnswerService
    {
        private readonly IBaseRepository<SurveyAnswer, Guid> _surveyAnswerRepository;
        private readonly IBaseRepository<SurveyQuestion, Guid> _surveyQuestionRepository;

        public SurveyAnswerService(IBaseRepository<SurveyAnswer, Guid> surveyAnswerRepository, IBaseRepository<SurveyQuestion, Guid> surveyQuestionRepository)
        {
            _surveyAnswerRepository = surveyAnswerRepository;
            _surveyQuestionRepository = surveyQuestionRepository;
        }

        public async Task AddSurveyAnswerForSurveyQuestion(List<SurveyAnswerRequest.CreateSurveyAnswerRequest> model)
        {
            foreach (var item in model)
            {
                var answer = new SurveyAnswer
                {
                    Content = item.Content,
                    CreateAt = DateTime.Now,
                    Point = item.Point,
                    QuestionId = item.QuestionId
                };
                await _surveyAnswerRepository.Add(answer);
            }
            await _surveyQuestionRepository.SaveChangesAsync();
        }

        public async Task<SurveyAnswerResponse.GetSurveyAnswerModel?> GetSurveyAnswerById(Guid id)
        {
            var surveyAnswer = await _surveyAnswerRepository.GetById(id);
            if(surveyAnswer is null)
            {
                throw new Exception("Không tìm thấy câu trả lời.");
            }
            return new SurveyAnswerResponse.GetSurveyAnswerModel
            {
                Id = id,
                Content = surveyAnswer.Content,
                IsDelete = surveyAnswer.IsDeleted,
                Point = surveyAnswer.Point
            };
        }

        public async Task<List<SurveyAnswerResponse.GetSurveyAnswerModel?>> GetSurveyAnswerForQuestion(List<Guid> questionIds)
        {
            var answerList = await _surveyAnswerRepository.GetAll().Where(s => questionIds.Contains(s.QuestionId)).Select(s => new SurveyAnswerResponse.GetSurveyAnswerModel
            {
                QuestionId = s.QuestionId,
                Content = s.Content,
                Id = s.Id,
                IsDelete= s.IsDeleted,
                Point = s.Point
            }).ToListAsync();
            return answerList;
        }

        public Task<List<SurveyAnswerResponse.GetSurveyAnswerModel>> GetSurveyAnswers()
        {
            throw new NotImplementedException();
        }

        public async Task RemoveSurveyAnswer(Guid id)
        {
            var surveyAnswer = await _surveyAnswerRepository.GetById(id);
            if (surveyAnswer is null)
            {
                throw new Exception("Không tìm thấy câu trả lời.");
            }
            surveyAnswer.ModifiedAt = DateTime.Now;
            surveyAnswer.IsDeleted = true;
            await _surveyAnswerRepository.Update(surveyAnswer);
            await _surveyAnswerRepository.SaveChangesAsync();
        }

        public async Task UpdateSurveyAnswer(Guid id, SurveyAnswerRequest.UpdateSurveyAnswerRequest model)
        {
            var surveyAnswer = await _surveyAnswerRepository.GetById(id);
            if (surveyAnswer is null)
            {
                throw new Exception("Không tìm thấy câu trả lời.");
            }
            surveyAnswer.Content = model.Content;
            surveyAnswer.IsDeleted = model.IsDelete.HasValue ? model.IsDelete.Value : surveyAnswer.IsDeleted;
            surveyAnswer.Point = model.Point;
            surveyAnswer.ModifiedAt = DateTime.Now;
            await _surveyAnswerRepository.Update(surveyAnswer);
            await _surveyAnswerRepository.SaveChangesAsync();
        }
    }
}
