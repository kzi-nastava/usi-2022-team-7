﻿using System.Collections.Generic;
using HIS.Core.PersonModel.PatientModel;
using Newtonsoft.Json;

namespace HIS.Core.PollModel.HospitalPollModel
{
    public class HospitalPoll : Poll
    {
        [JsonConverter(typeof(PatientJSONReferenceConverter))]
        public Patient Pollee { get; set; }

        public HospitalPoll()
        {

        }

        public HospitalPoll(Dictionary<string, int> questionnaire, string comment, Patient pollee) : base(questionnaire, comment)
        {
            Pollee = pollee;
        }

		public override string ToString()
		{
            return $"HospitalPoll{{Pollee={Pollee}, {base.ToString()}}}";
		}
	}
}
