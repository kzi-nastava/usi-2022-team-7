﻿using HIS.Core.MedicationModel;
using HIS.Core.MedicationModel.IngredientModel;
using HIS.Core.MedicationModel.MedicationRequestModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace HIS.CLI.View
{
	internal class IngredientView : AbstractView
	{
		private static readonly string errNameTaken = "Name already taken";
		private static readonly string hintName = "Enter name";
		private static readonly string warnDependentMedications = "The following medications will also be removed. Proceed?";

		private IIngredientService _service;
		private IMedicationService _medicationService;
		private IMedicationRequestService _medicationRequestService;
		private IEnumerable<IngredientProperty> _properties;

		public IngredientView(IIngredientService service, IMedicationService medicationService, IMedicationRequestService medicationRequestService)
		{
			_service = service;
			_medicationService = medicationService;
			_medicationRequestService = medicationRequestService;
			_properties = Utility.GetEnumValues<IngredientProperty>();
		}

		internal void CmdCreate()
		{
			var newIngredient = Input(_properties);
			_service.Add(newIngredient);
		}

		internal void CmdRead()
		{
			var selected = EasyInput<Ingredient>.Select(_service.GetAll(), _cancel);
			Print(selected.ToString());
		}

		internal void CmdUpdate()
		{
			var ingredientToChange = EasyInput<Ingredient>.Select(_service.GetAll(), _cancel);
			var selectedProperties = EasyInput<IngredientProperty>.SelectMultiple(_properties.ToList(), _cancel);
			var newIngredient = Input(selectedProperties);
			_service.Copy(newIngredient, ingredientToChange, selectedProperties);
		}

		internal void CmdDelete()
		{
			var selected = EasyInput<Ingredient>.SelectMultiple(_service.GetAll().ToList(), _cancel);
			var dependentMedications = selected.SelectMany(ing => _medicationService.GetAllThatUse(ing)).Distinct();
			var dependentMedicationRequests = selected.SelectMany(ing => _medicationRequestService.GetAllThatUse(ing)).Distinct();

			if (dependentMedications.Count() + dependentMedicationRequests.Count() != 0)
			{
				Hint(warnDependentMedications);
				Print(dependentMedications.Select(med => med.Name).Aggregate((s1, s2) => s1 + ", " + s2));
				Print(dependentMedicationRequests.Select(req => req.Medication.Name).Aggregate((s1, s2) => s1 + ", " + s2));
				if (!EasyInput<bool>.YesNo(_cancel))
				{
					return;
				}
			}

			// todo @magley : is this the right place to do this or should IngredientService be in charge of this?
			foreach (var med in dependentMedications)
			{
				_medicationService.Remove(med);
			}
			foreach (var req in dependentMedicationRequests)
			{
				_medicationRequestService.Remove(req);
			}

			foreach (var s in selected)
			{
				_service.Remove(s);
			}
		}

		private Ingredient Input(IEnumerable<IngredientProperty> whichProperties)
		{
			Ingredient result = new Ingredient();

			if (whichProperties.Contains(IngredientProperty.NAME))
			{
				Hint(hintName);
				result.Name = InputName();
			}

			return result;
		}

		private string InputName()
		{
			return EasyInput<string>.Get(
				new List<Func<string, bool>>() { s => _service.GetByName(s).Count() == 0 },
				new[] { errNameTaken },
				_cancel
			);
		}
	}
}
