using HEALTH_SUPPORT.Repositories.Entities;
using HEALTH_SUPPORT.Repositories.Repository;
using HEALTH_SUPPORT.Services.IServices;
using HEALTH_SUPPORT.Services.RequestModel;
using HEALTH_SUPPORT.Services.ResponseModel;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.Implementations
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IBaseRepository<Appointment, Guid> _appointmentRepository;
        private readonly IBaseRepository<Psychologist, Guid> _psychologistRepository;
        private readonly IBaseRepository<Account, Guid> _accountRepository;

        public AppointmentService(IBaseRepository<Appointment, Guid> appointmentRepository, IBaseRepository<Psychologist, Guid> psychologistRepository, IBaseRepository<Account, Guid> accountRepository)
        {
            _appointmentRepository = appointmentRepository;
            _psychologistRepository = psychologistRepository;
            _accountRepository = accountRepository;
        }

        public async Task AddAppointment(string userID, AppointmentRequest.AddAppointmentRequestRequest model)
        {
            var psychologist = await _psychologistRepository.GetById(model.PsychologistId);
            if (psychologist is null)
            {
                throw new Exception("Không tìm thấy bác sĩ tâm lý.");
            }
            Appointment appointment = new Appointment
            {
                AccountId = Guid.Parse(userID),
                CreateAt = DateTime.Now,
                PsychologistId = model.PsychologistId
            };
            await _appointmentRepository.Add(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }

        public async Task<AppointmentResponse.GetAppointmentModel?> GetAppointmentById(Guid id)
        {
            var appointment = await _appointmentRepository.GetById(id);
            if (appointment is null)
            {
                throw new Exception("Không tìm thấy lịch hẹn.");
            }
            var psychologist = await _psychologistRepository.GetById(appointment.PsychologistId);
            if (psychologist is null)
            {
                throw new Exception("Không tìm thấy bác sĩ tâm lý.");
            }
            var account = await _accountRepository.GetById(appointment.AccountId);
            if (account is null)
            {
                throw new Exception("Không tìm thấy người dùng.");
            }
            return new AppointmentResponse.GetAppointmentModel
            {
                Id = id,
                AccountId = appointment.AccountId,
                Account = new AppointmentResponse.GetAccountsForAppointmentModel
                {
                    Id = appointment.AccountId,
                    Address = account.Address,
                    Email = account.Email,
                    Fullname = account.Fullname,
                    Phone = account.Phone,
                    UserName = account.UseName
                },
                CreateAt = appointment.CreateAt,
                ModifiedAt = appointment.ModifiedAt,
                IsDelete = appointment.IsDeleted,
                PsychologistId = appointment.PsychologistId,
                Psychologist = new PsychologistResponse.GetPsychologistModel
                {
                    IsDelete = psychologist.IsDeleted,
                    Email = psychologist.Email,
                    Id = psychologist.Id,
                    Name = psychologist.Name,
                    PhoneNumber = psychologist.PhoneNumber,
                    Specialization = psychologist.Specialization
                }
            };
        }

        public async Task<List<AppointmentResponse.GetAppointmentModel>> GetAppointmentsForAccount(Guid accountId)
        {
            var appointment = _appointmentRepository.GetAll().Where(s => s.AccountId == accountId).Select(s => new AppointmentResponse.GetAppointmentModel
            {
                Id = s.Id,
                AccountId = s.AccountId,
                CreateAt = s.CreateAt,
                ModifiedAt = s.ModifiedAt,
                IsDelete = s.IsDeleted,
                PsychologistId = s.PsychologistId
            }).ToList();
            if (!appointment.Any())
            {
                throw new Exception("Không tìm thấy lịch hẹn.");
            }
            var psychologistIdList = appointment.Select(s => s.Id).ToList();
            var psychologist = _psychologistRepository.GetAll().Where(s => psychologistIdList.Contains(s.Id)).Distinct().Select(s => new PsychologistResponse.GetPsychologistModel
            {
                IsDelete = s.IsDeleted,
                Email = s.Email,
                Id = s.Id,
                Name = s.Name,
                PhoneNumber = s.PhoneNumber,
                Specialization = s.Specialization
            }).ToList();
            if (!psychologist.Any())
            {
                throw new Exception("Không tìm thấy bác sĩ tâm lý.");
            }
            foreach(var item in appointment)
            {
                item.Psychologist = psychologist.FirstOrDefault(s => s.Id == item.PsychologistId);
            }
            return appointment;
        }

        public async Task<List<AppointmentResponse.GetAppointmentModel>> GetAppointmentsForPsychologist(Guid psychologistId)
        {
            var appointment = _appointmentRepository.GetAll().Where(s => s.PsychologistId == psychologistId).Select(s => new AppointmentResponse.GetAppointmentModel
            {
                Id = s.Id,
                AccountId = s.AccountId,
                CreateAt = s.CreateAt,
                ModifiedAt = s.ModifiedAt,
                IsDelete = s.IsDeleted,
                PsychologistId = s.PsychologistId
            }).ToList();
            if (!appointment.Any())
            {
                throw new Exception("Không tìm thấy lịch hẹn.");
            }
            var accountIdList = appointment.Select(s => s.Id).ToList();
            var account = _accountRepository.GetAll().Where(s => accountIdList.Contains(s.Id)).Distinct().Select(s => new AppointmentResponse.GetAccountsForAppointmentModel
            {
                Id = s.Id,
                Address = s.Address,
                Email = s.Email,
                Fullname = s.Fullname,
                Phone = s.Phone,
                UserName = s.UseName
            }).ToList();
            if (!account.Any())
            {
                throw new Exception("Không tìm thấy bác sĩ tâm lý.");
            }
            foreach (var item in appointment)
            {
                item.Account = account.FirstOrDefault(s => s.Id == item.AccountId);
            }
            return appointment;
        }

        public async Task RemoveAppointment(Guid id)
        {
            var appointment = await _appointmentRepository.GetById(id);
            if (appointment is null)
            {
                throw new Exception("Không tìm thấy lịch hẹn.");
            }
            appointment.IsDeleted = true;
            appointment.ModifiedAt = DateTime.Now;
            await _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }

        public async Task UpdateAppointment(Guid id, AppointmentRequest.EditAppointmentRequestRequest model)
        {
            var appointment = await _appointmentRepository.GetById(id);
            if (appointment is null)
            {
                throw new Exception("Không tìm thấy lịch hẹn.");
            }
            appointment.AccountId = model.AccountId;
            appointment.PsychologistId = model.PsychologistId;
            appointment.ModifiedAt = DateTime.Now;
            await _appointmentRepository.Update(appointment);
            await _appointmentRepository.SaveChangesAsync();
        }
    }
}
