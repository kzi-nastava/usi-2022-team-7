﻿using Newtonsoft.Json;

namespace HospitalIS.Backend
{
    public class DeleteRequest : Request
    {
        [JsonConverter(typeof(Repository.AppointmentRepository.AppointmentReferenceConverter))]
        public Appointment Appointment { get; set; }

        public DeleteRequest(UserAccount requestee, Appointment appointment) : base(requestee)
        {
            Appointment = appointment;
        }
        public override string ToString()
        {
            return $"DeleteRequest{{Id = {Id}, AppointmentId = {Appointment.Id}, PatientId = {Requestee.Id}}}";
        }
    }
}
