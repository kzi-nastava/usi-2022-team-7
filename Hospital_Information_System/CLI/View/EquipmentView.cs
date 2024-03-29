﻿using HIS.Core.EquipmentModel;
using HIS.Core.PersonModel.UserAccountModel;
using HIS.Core.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using HIS.Core.EquipmentModel.EquipmentRequestModel;
using HIS.Core.RoomModel;

namespace HIS.CLI.View
{
	internal class EquipmentView : AbstractView
	{
		private static readonly string hintEnterQuery = "Enter search query";
		private static readonly string hintSearchSelectEquipment = "Select equipment";
		private static readonly string hintInputAmountOfEquipment = "Input amount of equipment";
		private static readonly string errNoEquipmentNeeded = "All equipment is in stock";
		private static readonly string errNotZero = "Input amount must be greater than 0!";
		private const string errNotEnoughEquipment = "This room does not have that much equipment";
		private const string showCurrentState = "Current state in the room [Equipment: Quantity]:";
		private const string errNoEquipmentInRoom = "There's no equipment in this room";
		private const string errPositiveNumber = "You must input a positive number (or 0)";
		private const string hintInputUsedEquipment = "For each equipment, input how many you have used: ";

		private readonly IEquipmentService _service;
		private readonly IList<EquipmentProperty> _searchableProperties;
		private readonly IEnumerable<EquipmentType> _equipmentTypes;
		private readonly IEnumerable<EquipmentUse> _equipmentUses;
		private readonly IEquipmentRequestService _equipmentRequestService;

		internal EquipmentView(IEquipmentService service, IEquipmentRequestService equipmentRequestService)
		{
			_service = service;
			_searchableProperties = Utility.GetEnumValues<EquipmentProperty>().ToList();
			_equipmentTypes = Utility.GetEnumValues<EquipmentType>();
			_equipmentUses = Utility.GetEnumValues<EquipmentUse>();
			_equipmentRequestService = equipmentRequestService;
		}

		internal void CmdSearch()
		{
			var propertiesToSearchOn = EasyInput<EquipmentProperty>.SelectMultiple(_searchableProperties, _cancel);

			Hint(hintEnterQuery);
			var searchQuery = EasyInput<string>.Get(_cancel);
			var searchResult = _service.Search(searchQuery, propertiesToSearchOn);

			foreach (var equipment in searchResult)
			{
				Print(equipment.ToString());
			}
		}

		internal void CmdFilter()
		{
			var filterMapping = new Dictionary<string, Func<IEnumerable<Equipment>>>
			{
				["Filter by type"] = () => _service.FilterByType(SelectType()),
				["Filter by use"] = () => _service.FilterByUse(SelectUse()),
				["Filter by out of stock"] = () => _service.FilterByAmount(num => num == 0),
				["Filter by less than 10"] = () => _service.FilterByAmount(num => num >= 0 && num < 10),
				["Filter by more than 10"] = () => _service.FilterByAmount(num => num >= 10),
			};

			var filterQuery = EasyInput<string>.Select(filterMapping.Keys, _cancel);
			var filterResult = filterMapping[filterQuery]();

			foreach (var equipment in filterResult)
			{
				Print(equipment.ToString() + $" ({_service.GetTotalSupply(equipment)})");
			}
		}

		internal void CmdRequestNew()
		{
			try
			{
				IEnumerable<Equipment> equipmentNotInStock = _service.GetDynamicEquipmentNotInStock();

				var selectedEquipment = SelectEquipment(equipmentNotInStock);

				var newRequest = CreateRequestEquipment(selectedEquipment);

				newRequest.OrderTime = DateTime.Now;
				_equipmentRequestService.Add(newRequest);
			}
			catch (NothingToSelectException)
			{
				Print(errNoEquipmentNeeded);
			}
		}
		
		private EquipmentRequest CreateRequestEquipment(List<Equipment> equipments)
		{
			EquipmentRequest request = new EquipmentRequest();
			
			foreach (var equipment in equipments)
			{
				int amountOfEquipment = InputAmountOfEquipment(equipment, _cancel);
				request.Equipment.Add(equipment, amountOfEquipment);
			}
			return request;
		}
		
		private int InputAmountOfEquipment(Equipment equipment, string inputCancelString)
		{
			Print(equipment.ToString());
			Print(hintInputAmountOfEquipment);
			return EasyInput<int>.Get(new List<Func<int, bool>> {s => s > 0}, new[]
				{errNotZero}, inputCancelString);
		}

		private List<Equipment> SelectEquipment(IEnumerable<Equipment> equipmentNotInStock)
		{
			Print(hintSearchSelectEquipment);
			return EasyInput<Equipment>.SelectMultiple(equipmentNotInStock.ToList(), _cancel)
				.ToList();
		}

		private EquipmentType SelectType()
		{
			return EasyInput<EquipmentType>.Select(_equipmentTypes, _cancel);
		}

		private EquipmentUse SelectUse()
		{
			return EasyInput<EquipmentUse>.Select(_equipmentUses, _cancel);
		}
		
		public void DeleteEquipmentAfterAppointment(Room room)
		{
			Dictionary<Equipment, int> currentDynamicEquipmentQuantity = _service.GetDynamicEquipment(room);
			if (currentDynamicEquipmentQuantity.Count == 0)
			{
				Error(errNoEquipmentInRoom);
				return;
			}
			Print(currentDynamicEquipmentQuantity);
			var newDynamicEquipment = GetNewEquipmentQuantity(currentDynamicEquipmentQuantity);
			var nonDynamicEquipment = _service.GetNonDynamicEquipment(room);
			var equipment = _service.GetEquipmentAfterDeletion(newDynamicEquipment, nonDynamicEquipment);
			room.Equipment = equipment;
		}
		
		private void Print(Dictionary<Equipment, int> currentEquipmentQuantity)
		{
			Hint(showCurrentState);
			foreach (KeyValuePair<Equipment, int> entry in currentEquipmentQuantity)
			{
				Print(entry.Key + ": " + entry.Value);
			}
			
		}
		
		private Dictionary<Equipment, int> GetNewEquipmentQuantity(
			Dictionary<Equipment, int> oldEquipmentQuantity)
		{
			Dictionary<Equipment, int> newEquipmentQuantity = new Dictionary<Equipment, int>();
			Hint(hintInputUsedEquipment);
			foreach (KeyValuePair<Equipment, int> entry in oldEquipmentQuantity)
			{
				var equipment = entry.Key;
				var currentQuantity = entry.Value;
				Console.Write(equipment + ": ");
				int usedQuantity = GetUsedEquipmentQuantity(currentQuantity); 
				int newQuantity = currentQuantity - usedQuantity;
				if (newQuantity != 0)
				{
					newEquipmentQuantity[equipment] = newQuantity;
				}
			}

			return newEquipmentQuantity;
		}
		
		private int GetUsedEquipmentQuantity(int currentQuantity)
		{
			return EasyInput<int>.Get(
				new List<Func<int, bool>>
				{
					s => s <= currentQuantity,
					s => s >= 0,
				},
				new[]
				{
					errNotEnoughEquipment,
					errPositiveNumber,
				},
				_cancel
			);
		}
	}
}
