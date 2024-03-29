﻿using HIS.Core.AppointmentModel;
using HIS.Core.PersonModel.PatientModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.PollModel.AppointmentPollModel;
using System.Collections.Generic;

namespace HIS.CLI.View
{
	internal class AppointmentPollView : AbstractView
	{
		private IAppointmentPollService _service;
		private IPatientService _patientService;
		private IAppointmentService _appointmentService;
		private PollView _pollView;

		private const string hintSelectAppointment = "Select appointment";
		private const string hintComment = "Input comment";

		public AppointmentPollView(IAppointmentPollService service, IPatientService patientService, IAppointmentService appointmentService, PollView pollView)
		{
			_service = service;
			_patientService = patientService;
			_appointmentService = appointmentService;
			_pollView = pollView;
		}

		internal void CmdCreate()
		{
			try
			{
				if (User.Type != UserAccount.AccountType.PATIENT)
				{
					return;
				}

				Patient patient = _patientService.GetPatientFromPerson(User.Person);

				Hint(hintSelectAppointment);
				Appointment appointment = EasyInput<Appointment>.Select(_appointmentService.GetPollable(patient), _cancel);

				Dictionary<string, int> questionnaire = _pollView.GenerateQuestionnaire(AppointmentPollHelpers.Questions);

				Hint(hintComment);
				string comment = EasyInput<string>.Get(_cancel);

				var poll = new AppointmentPoll(questionnaire, comment, appointment);

				_service.Add(poll);
			}
			catch (NothingToSelectException e)
			{
				Error(e.Message);
			}
		}
	}
}
