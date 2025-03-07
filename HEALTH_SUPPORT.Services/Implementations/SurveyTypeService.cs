using HEALTH_SUPPORT.Repositories.Entities;
using HEALTH_SUPPORT.Repositories.Repository;
using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.Implementations
{
    public class SurveyTypeService : ISurveyTypeService
    {
        private readonly IBaseRepository<SurveyType, Guid> _surveyTypeRepository;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _environment;

        public SurveyTypeService(IBaseRepository<SurveyType, Guid> surveyTypeRepository, IConfiguration configuration, IHostEnvironment environment)
        {
            _surveyTypeRepository = surveyTypeRepository;
            _configuration = configuration;
            _environment = environment;
        }

        public async Task AddSurveyType(SurveyTypeRequest.CreateSurveyTypeModel model)
        {
            SurveyType survey = new SurveyType
            {
                SurveyName = model.SurveyName
            };
            await _surveyTypeRepository.Add(survey);
            await _surveyTypeRepository.SaveChangesAsync();
        }

        public async Task<SurveyTypeResponse.GetSurveyTypeModel?> GetSurveyTypeById(Guid id)
        {
            var surveyType = await _surveyTypeRepository.GetById(id);
            if (surveyType is null)
            {
                throw new Exception("Not exist survey type!");
            }
            return new SurveyTypeResponse.GetSurveyTypeModel
            {
                Id = surveyType.Id,
                SurveyName = surveyType.SurveyName
            };
        }

        public async Task<List<SurveyTypeResponse.GetSurveyTypeModel>> GetSurveyTypes()
        {
            return await _surveyTypeRepository.GetAll()
                .Where(s => !s.IsDeleted)
                .AsNoTracking()
                .Select(s => new SurveyTypeResponse.GetSurveyTypeModel
                {
                    Id = s.Id,
                    SurveyName = s.SurveyName
                })
                .ToListAsync();
                          
        }

        public async Task RemoveSurveyType(Guid id)
        {
            var surveyType = await _surveyTypeRepository.GetById(id);
            if (surveyType is null)
            {
                throw new Exception("Not exist survey type!");
            }
            surveyType.IsDeleted = true;
            await _surveyTypeRepository.Update(surveyType);
            await _surveyTypeRepository.SaveChangesAsync();
        }

        public async Task UpdateSurveyType(Guid id, SurveyTypeRequest.UpdateSurveyTypeModel model)
        {
            var surveyType = await _surveyTypeRepository.GetById(id);
            if (surveyType is null)
            {
                throw new Exception("Not exist survey type!");
            }
            surveyType.SurveyName = model.SurveyName;
            await _surveyTypeRepository.Update(surveyType);
            await _surveyTypeRepository.SaveChangesAsync();
        }
    }
}
