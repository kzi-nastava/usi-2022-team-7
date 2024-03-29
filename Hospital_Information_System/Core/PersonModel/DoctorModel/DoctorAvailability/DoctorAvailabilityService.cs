﻿using HIS.Core.AppointmentModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.Core.PersonModel.DoctorModel.DoctorAvailability
{
    public class DoctorAvailabilityService : IDoctorAvailabilityService
    {
        private IDoctorService _doctorService;
        private IAppointmentService _appointmentService;

        public DoctorAvailabilityService()
        {
        }

        public DoctorAvailabilityService(IDoctorService doctorService, IAppointmentService appointmentService)
        {
            _doctorService = doctorService;
            _appointmentService = appointmentService;
        }

        public Doctor FindFirstAvailableDoctor(DateTime scheduledFor)
        {
            return _doctorService.GetAll().First(d => IsAvailable(d, scheduledFor));
        }

        public bool IsAvailable(Doctor doctor, DateTime newSchedule, Appointment refAppointment = null)
        {
            foreach (Appointment appointment in _appointmentService.GetAll())
            {
                if ((doctor == appointment.Doctor) && (appointment != refAppointment))
                {
                    if (_appointmentService.AreColliding(appointment.ScheduledFor, newSchedule))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public IEnumerable<Doctor> GetAvailable(Appointment refAppointment)
        {
            if (refAppointment == null)
            {
                return _doctorService.GetAll();
            }

            return _doctorService.GetAll().Where(d => IsAvailable(d, refAppointment.ScheduledFor, refAppointment));
        }

        public Doctor FindFirstAvailableDoctorOfSpecialty(DateTime scheduledFor, Doctor.MedicineSpeciality speciality)
        {
            return _doctorService.GetAll().First(d => IsAvailable(d, scheduledFor) && d.Specialty == speciality);
        }
    }
}
