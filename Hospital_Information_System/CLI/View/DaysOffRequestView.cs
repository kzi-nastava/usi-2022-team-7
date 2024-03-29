﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using HIS.Core.PersonModel.DoctorModel;
using HIS.Core.PersonModel.DoctorModel.DaysOffRequestModel;
using HIS.Core.PersonModel.UserAccountModel;

namespace HIS.CLI.View
{
    internal class DaysOffRequestView : AbstractView
    {
        private readonly IDaysOffRequestService _service;
        private readonly IDoctorService _doctorService;
        private readonly AppointmentView _appointmentView;
        
        
        private const string errTooLateToSchedule =
            "You can send this request 2 or more days before the first day off, now it is too late";

        private const string hintInputStartDay = "Input date you want the break to start";
        private const string hintInputEndDay = "Input date you want the break to end";
        private const string hintInputReason = "Input reason for requesting days off";
        private const string hintSelectAction = "Select action over requests";
        private const string hintApproved = "Request approved";
        private const string hintDenied = "Request denied";

        private const string hintIsRequestUrgent = "Do you want to make an urgent request?";
        private const string errEndBeforeStart = "The last day off comes before or at the same day as the first day, input last day again";
        private const string errUnableToSchedule = "You have appointment(s) or days off scheduled during the requested break";
        private const string errNoReason = "You have to input reason";
        private const string errBreakTooLong = "You can schedule break that lasts up to 5 days";
        private const string errUnableToApprove = "Doctor has appointments scheduled for that time period";

        public DaysOffRequestView(IDaysOffRequestService service, IDoctorService doctorService, AppointmentView appointmentView)
        {
            _service = service;
            _doctorService = doctorService;
            _appointmentView = appointmentView;
        }

        internal void CmdRead()
        {
            var requests = _service.Get(User);
            PrintAll(requests);
        }
        
        private void PrintAll(List<DaysOffRequest> requests)
        {
            foreach (var request in requests)
            {
                Print(request.ToString());
            }
        }
        
        internal void CmdCreateDaysOffRequest()
        {
            Doctor doctor = _doctorService.GetDoctorFromPerson(User.Person);
            DaysOffRequest daysOffRequest; 
            Hint(hintIsRequestUrgent);
            if (EasyInput<bool>.YesNo(_cancel)) //request is urgent
            {
                daysOffRequest = CreateUrgentRequest(doctor);
            }
            else //request is not urgent
            {
                daysOffRequest = CreateUnurgentRequest(doctor);
            }
            
            _service.Add(daysOffRequest);
        }

        internal void NotifyDoctor()
        {
            var requests = _service.GetChanged(User);
            PrintAll(requests);
            foreach (var request in requests)
            {
                request.Deleted = true;
            }
        }
        
        internal void CmdHandle()
        {
            try
            {
                CmdRead();
            
                var actions = new Dictionary<string, Action>
                {
                    ["Approve days off request"] = () => Approve(),
                    ["Deny days off request"] = () => Deny()
                };
                Print(hintSelectAction);
                var actionChoice = EasyInput<string>.Select(actions.Keys.ToList(), _cancel);

                actions[actionChoice]();
            }
            catch (NothingToSelectException e)
            {
                Error(e.Message);
            }
        }

        private void Approve()
        {
            var request = Select();
            
            if (HasProblematicAppointments(request))
                Hint(errUnableToApprove);
            else
            {
                request.State = DaysOffRequest.DaysOffRequestState.APPROVED;
                Hint(hintApproved);
            }
                
        }

        private void Deny()
        {
            var request = Select();
            request.RejectionExplanation = InputExplanation();
            request.State = DaysOffRequest.DaysOffRequestState.REJECTED;
            Hint(hintDenied);
        }

        private DaysOffRequest Select()
        {
            return EasyInput<DaysOffRequest>.Select(_service.Get(User), _cancel);
        }

        private bool HasProblematicAppointments(DaysOffRequest request)
        {
            return _service.FindProblematicAppointments(request.Requester, request.Start, request.End).Count() != 0;
        }
        private DaysOffRequest CreateUnurgentRequest(Doctor doctor)
        {
            DateTime start;
            DateTime end;
            while (true)
            {
                start = InputStartDay();
                end = InputEndDay(start);
                if (_service.IsRangeCorrect(doctor, start, end)) break;
                
                Error(errUnableToSchedule);
                _appointmentView.Print(_service.FindProblematicAppointments(doctor, start, end));
                Print(_service.FindProblematicDaysOff(doctor, start, end));
            }
            var reason = InputReason();
            var state = DaysOffRequest.DaysOffRequestState.SENT;
            return new DaysOffRequest(doctor, start, end, reason, state);
        }

        private DaysOffRequest CreateUrgentRequest(Doctor doctor)
        {
            DateTime start;
            DateTime end;
            while (true)
            {
                start = InputStartDay();
                end = InputEndDay(start);
                if (_service.IsEndDateCorrect(start, end)) break;
                
                Error(errBreakTooLong);
            }

            var reason = InputReason();
            var state = DaysOffRequest.DaysOffRequestState.APPROVED;
            _service.DeleteProblematicAppointments(doctor, start, end);
            return new DaysOffRequest(doctor, start, end, reason, state);
        }

        private DateTime InputStartDay()
        {
            Hint(hintInputStartDay);
            DateTime lastDayForScheduling = DateTime.Now.AddDays(2);
            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>
                {
                    m => DateTime.Compare(lastDayForScheduling, m) <= 0,
                },
                new string[]
                {
                    errTooLateToSchedule,
                },
                _cancel);
        }
        
        private DateTime InputEndDay(DateTime startDay)
        {
            Hint(hintInputEndDay);
            return EasyInput<DateTime>.Get(
                new List<Func<DateTime, bool>>
                {
                    m => DateTime.Compare(startDay.AddDays(1), m) <= 0,
                },
                new string[]
                {
                    errEndBeforeStart,
                },
                _cancel);
        }

        private string InputReason()
        {
            Hint(hintInputReason);
            return EasyInput<string>.Get(
                new List<Func<string, bool>>
                {
                    s => s.Trim().Length > 0,
                },
                new string[]
                {
                    errNoReason,
                },
                _cancel);
        }
        
        private string InputExplanation()
        {
            Hint(hintInputReason);
            return EasyInput<string>.Get(
                new List<Func<string, bool>>
                {
                    s => s.Trim().Length > 0,
                },
                new string[]
                {
                    errNoReason,
                },
                _cancel);
        }

        private void Print(List<DaysOffRequest> requests)
        {
            foreach (var request in requests)
            {
               Print(request.ToString());
            }
        }
    }
}