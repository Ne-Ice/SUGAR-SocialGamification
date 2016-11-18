﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.SUGAR.WebAPI.Extensions
{
	public static class EvaluationCriteriaExtensions
    {
		public static EvaluationCriteriaResponse ToContract(this Data.Model.EvaluationCriteria completionCriteria)
		{
			if (completionCriteria == null)
			{
				return null;
			}
			return new EvaluationCriteriaResponse
			{
                Id = completionCriteria.Id,
                Key = completionCriteria.Key,
				DataType = completionCriteria.DataType,
				CriteriaQueryType = completionCriteria.CriteriaQueryType,
				ComparisonType = completionCriteria.ComparisonType,
				Scope = completionCriteria.Scope,
				Value = completionCriteria.Value,
			};
		}

		public static List<EvaluationCriteriaResponse> ToContractList(this List<Data.Model.EvaluationCriteria> completionCriterias)
		{
			return completionCriterias.Select(ToContract).ToList();
		}

        public static Data.Model.EvaluationCriteria ToModel(this EvaluationCriteriaCreateRequest completionCriteria)
        {
            if (completionCriteria == null)
            {
                return null;
            }
            return new Data.Model.EvaluationCriteria
            {
                Key = completionCriteria.Key,
                DataType = completionCriteria.DataType,
                CriteriaQueryType = completionCriteria.CriteriaQueryType,
                ComparisonType = completionCriteria.ComparisonType,
                Scope = completionCriteria.Scope,
                Value = completionCriteria.Value,
            };
        }

        public static Data.Model.EvaluationCriteria ToModel(this EvaluationCriteriaUpdateRequest completionCriteria)
        {
            if (completionCriteria == null)
            {
                return null;
            }
            return new Data.Model.EvaluationCriteria
            {
                Id = completionCriteria.Id,
                Key = completionCriteria.Key,
                DataType = completionCriteria.DataType,
                CriteriaQueryType = completionCriteria.CriteriaQueryType,
                ComparisonType = completionCriteria.ComparisonType,
                Scope = completionCriteria.Scope,
                Value = completionCriteria.Value,
            };
        }

        public static List<Data.Model.EvaluationCriteria> ToModelList(this List<EvaluationCriteriaCreateRequest> completionCriterias)
        {
            return completionCriterias.Select(ToModel).ToList();
        }

        public static List<Data.Model.EvaluationCriteria> ToModelList(this List<EvaluationCriteriaUpdateRequest> completionCriterias)
        {
            return completionCriterias.Select(ToModel).ToList();
        }
    }
}
