﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Server.Model;

namespace PlayGen.SUGAR.Server.WebAPI.Extensions
{
	public static class ActorExtensions
	{
		public static IEnumerable<ActorResponse> ToActorContractList(this IEnumerable<Actor> actorModels)
		{
			return actorModels.Select(ToActorContract).ToList();
		}

		public static ActorResponse ToActorContract(this Actor model)
		{
			return new ActorResponse {
				Id = model.Id,

				Name = model.Name
			};
		}
	}
}