﻿using HIS.Core.Foundation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HIS.Core.PollModel
{
    public abstract class Poll : Entity
    {
        public static readonly Predicate<int> IsValidRating = rating => rating >= 1 && rating <= 5;
        public const string ErrInvalidRating = "Rating must be between 1 and 5";

        [JsonProperty]
        private readonly Dictionary<string, int> _questionnaire;

        public string Comment { get; set; }

        public Poll()
        {

        }

        public Poll(Dictionary<string, int> questionnaire, string comment)
        {
            ValidateQuestionnaire(questionnaire);
            _questionnaire = questionnaire;
            Comment = comment;
        }

        public void AddQuestion(string question, int rating)
        {
            ValidateQuestion(question, rating);
            _questionnaire[question] = rating;
        }

        public void RemoveQuestion(string question)
        {
            _questionnaire.Remove(question);
        }

        public int GetRating(string question)
        {
            return _questionnaire[question];
        }

        public IList<string> GetQuestions()
		{
            return _questionnaire.Keys.ToList();
		}

        private void ValidateQuestionnaire(Dictionary<string, int> questionnaire)
        {
            foreach (KeyValuePair<string, int> pair in questionnaire)
            {
                ValidateQuestion(pair.Key, pair.Value);
            }
        }

        private static void ValidateQuestion(string question, int rating)
        {
            if (!IsValidRating(rating))
            {
                throw new ArgumentException($"{question}: {ErrInvalidRating}");
            }
        }

		public override string ToString()
		{
            return $"Poll{{Questionnaire={{{_questionnaire.Select(kv => $"{kv.Key}: {kv.Value}").Aggregate((s1, s2) => $"{s1}, {s2}")}}}, Comment={Comment}}}";
		}
	}
}
