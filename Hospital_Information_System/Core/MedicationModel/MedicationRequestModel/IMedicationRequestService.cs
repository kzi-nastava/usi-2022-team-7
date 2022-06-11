﻿using HIS.Core.MedicationModel.IngredientModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel.MedicationRequestModel
{
	public interface IMedicationRequestService
	{
		public MedicationRequest Add(MedicationRequest medicationRequest);
		public IEnumerable<MedicationRequest> GetAllThatUse(Ingredient ing);
		public void Remove(MedicationRequest req);
		public IEnumerable<MedicationRequest> GetAllReturnedForRevision();
	}
}