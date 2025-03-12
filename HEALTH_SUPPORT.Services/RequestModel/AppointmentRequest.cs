using HEALTH_SUPPORT.Repositories.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEALTH_SUPPORT.Services.RequestModel
{
    public class AppointmentRequest
    {
        public class AddAppointmentRequestRequest
        {
            public Guid PsychologistId { get; set; }
        }

        public class EditAppointmentRequestRequest
        {
            public Guid AccountId { get; set; }
            public Guid PsychologistId { get; set; }
        }
    }
}
