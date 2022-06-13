﻿using HIS.Core.PersonModel.DoctorModel.DoctorComparers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.PersonModel.DoctorModel
{
	public class DoctorService : IDoctorService
	{
		private readonly IDoctorRepository _repo;

		public DoctorService(IDoctorRepository repo)
		{
			_repo = repo;
		}

		public IEnumerable<Doctor> GetAll()
		{
			return _repo.GetAll();
		}

        public Doctor GetDoctorFromPerson(Person person)
		{
            return _repo.GetAll().First(d => d.Person == person);
		}

        public IEnumerable<Doctor> MatchByFirstName(string query, DoctorComparer comparer)
        {
            return _repo.MatchByFirstName(query, comparer);
        }

        public IEnumerable<Doctor> MatchByLastName(string query, DoctorComparer comparer)
        {
            return _repo.MatchByLastName(query, comparer);
        }

        public IEnumerable<Doctor> MatchBySpecialty(string query, DoctorComparer comparer)
        {
            return _repo.MatchBySpecialty(query, comparer);
        }

        public IEnumerable<Doctor> MatchByString(string query, DoctorComparer comparer, Func<Doctor, string> toStr)
        {
            return _repo.MatchByString(query, comparer, toStr);
        }
    }
}
