using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.IServices
{
    public interface IAppointmentService
    {
        Task<List<AppointmentResponse.GetAppointmentModel>> GetAppointmentsForAccount(Guid accountId);
        Task<List<AppointmentResponse.GetAppointmentModel>> GetAppointmentsForPsychologist(Guid psychologistId);
        Task<AppointmentResponse.GetAppointmentModel?> GetAppointmentById(Guid id);
        Task AddAppointment(string userID, AppointmentRequest.AddAppointmentRequestRequest model);
        Task UpdateAppointment(Guid id, AppointmentRequest.EditAppointmentRequestRequest model);
        Task RemoveAppointment(Guid id);
    }
}
