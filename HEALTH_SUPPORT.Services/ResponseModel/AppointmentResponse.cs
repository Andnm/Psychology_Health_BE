using HEALTH_SUPPORT.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.ResponseModel
{
    public class AppointmentResponse
    {
        public class GetAppointmentModel
        {
            public Guid Id { get; set; }
            public DateTimeOffset CreateAt { get; set; }
            public DateTimeOffset? ModifiedAt { get; set; }
            public Guid AccountId { get; set; }
            [ForeignKey("AccountId")]
            public GetAccountsForAppointmentModel Account { get; set; }
            public Guid PsychologistId { get; set; }
            [ForeignKey("PsychologistId")]
            public PsychologistResponse.GetPsychologistModel Psychologist { get; set; }
            public bool IsDelete { get; set; }
        }

        public class GetAccountsForAppointmentModel
        {
            public Guid Id { get; set; }
            public string UserName { get; set; }
            public string Fullname { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string Address { get; set; }
        }
    }
}
