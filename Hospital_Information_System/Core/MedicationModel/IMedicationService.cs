﻿using HIS.Core.MedicationModel.IngredientModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace HIS.Core.MedicationModel
{
	public interface IMedicationService
	{
		public Medication Add(Medication obj);
		public Medication Get(int id);
		public IEnumerable<Medication> GetAll();
		public IEnumerable<Medication> GetByName(string name);
		public void Remove(Medication obj);
		public IEnumerable<Medication> GetAllThatUse(Ingredient ingredient);
		public void Copy(Medication src, Medication dest, IEnumerable<MedicationProperty> properties);

		public bool IsMedicationSafe(List<Ingredient> medicationIngredients, List<Ingredient> patientsAllergies);
	}
}
