﻿using System.Collections.Generic;
using PlayGen.SUGAR.Contracts.Shared;
using System.Linq;

namespace PlayGen.SUGAR.WebAPI.Extensions
{
	public static class EvaluationDataExtensions
	{
        public static List<EvaluationDataResponse> ToContractList(this List<Data.Model.EvaluationData> models)
        {
            return models.Select(ToContract).ToList();
        }

        public static EvaluationDataResponse ToContract(this Data.Model.EvaluationData evaluationData)
		{
			if (evaluationData == null)
			{
				return null;
			}

			return new EvaluationDataResponse
			{
                GameId = evaluationData.GameId,
                RelatedEntityId = evaluationData.RelatedEntityId,
                CreatingActorId = evaluationData.CreatingActorId,
				Key = evaluationData.Key,
				Value = evaluationData.Value,
				EvaluationDataType = evaluationData.EvaluationDataType
			};
		}
	}
}