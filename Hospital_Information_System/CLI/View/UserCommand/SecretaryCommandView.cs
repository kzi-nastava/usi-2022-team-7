using System;
using System.Collections.Generic;

namespace HIS.CLI.View.UserCommand
{
    internal class SecretaryCommandView : UserCommandView
    {
        public SecretaryCommandView(UserAccountView userAccountView, AppointmentView appointmentView, MedicalRecordView medicalRecordView)
        {
            AddCommands(new Dictionary<string, Action>
            {
                {"patient-account-create", () => userAccountView.CmdCreate()},
                {"patient-account-view", () => userAccountView.CmdRead()},
                {"patient-account-update", () => userAccountView.CmdUpdate()},
                {"patient-account-delete", () => userAccountView.CmdDelete()},
                {"block-patient-account", () => userAccountView.CmdBlock()},
                {"unblock-patient-account", () => userAccountView.CmdUnblock()},
                {"view-patient-requests", () => throw new NotImplementedException()},
                {"handle-patient-requests", () => throw new NotImplementedException()},
                {"handle-referrals", () => throw new NotImplementedException()}, 
                {"create-urgent-appointment", () => throw new NotImplementedException()}, 
                {"request-new-equipment", () => throw new NotImplementedException()},
                {"move-dynamic-equipment", () => throw new NotImplementedException()}
            });
        }
    }
}