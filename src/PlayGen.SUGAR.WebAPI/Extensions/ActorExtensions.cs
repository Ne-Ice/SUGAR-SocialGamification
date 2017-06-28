﻿using System.Collections.Generic;
using System.Linq;
using PlayGen.SUGAR.Common;
using PlayGen.SUGAR.Contracts;

namespace PlayGen.SUGAR.WebAPI.Extensions
{
	public static class ActorExtensions
	{
		public static ActorsResponse ToCollectionContract(this IEnumerable<Actor> models)
		{
			return new ActorsResponse() {
				Items = models.Select(ToContract).ToArray(),
			};

		}

		public static ActorResponse ToContract(this Actor model)
		{
			return new ActorResponse
			{
				Id = model.Id,

				Name = model.Name
			};
		}
	}
}