﻿using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Data.Model;

namespace PlayGen.SUGAR.WebAPI.Extensions
{
	public static class ActorExtensions
	{
		public static ActorResponse ToContract(this Actor actorModel)
		{
			var actorContract = new ActorResponse
			{
				Id = actorModel.Id,
			};

			return actorContract;
		}
	}
}