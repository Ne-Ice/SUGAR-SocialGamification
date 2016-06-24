﻿using System.Collections.Generic;
using PlayGen.SUGAR.Client.Extensions;
using PlayGen.SUGAR.Contracts;
using PlayGen.SUGAR.Contracts.Controllers;

namespace PlayGen.SUGAR.Client
{
	/// <summary>
	/// Controller that facilitates UserData specific operations.
	/// </summary>
	public class UserDataClient : ClientBase, IUserDataController
	{
		public UserDataClient(string baseAddress) : base(baseAddress)
		{
		}

		/// <summary>
		/// Get a list of all UserData that match the <param name="actorId"/>, <param name="gameId"/> and <param name="keys"/> provided.
		/// </summary>
		/// <param name="actorId">ID of a User.</param>
		/// <param name="gameId">ID of a Game.</param>
		/// <param name="keys">Array of Key names.</param>
		/// <returns>A list of <see cref="SaveDataResponse"/> which match the search criteria.</returns>
		public IEnumerable<SaveDataResponse> Get(int actorId, int gameId, string[] keys)
		{
			var query = GetUriBuilder("api/usersavedata")
				.AppendQueryParameter(actorId, "actorId={0}")
				.AppendQueryParameter(gameId, "gameId={0}")
				.AppendQueryParameters(keys, "key={0}")
				.ToString();
			return Get<IEnumerable<SaveDataResponse>>(query);
		}

		/// <summary>
		/// Create a new UserData record.
		/// </summary>
		/// <param name="data"><see cref="SaveDataRequest"/> object that holds the details of the new UserData.</param>
		/// <returns>A <see cref="SaveDataResponse"/> containing the new UserData details.</returns>
		public SaveDataResponse Add(SaveDataRequest data)
		{
			var query = GetUriBuilder("api/usersavedata").ToString();
			return Post<SaveDataRequest, SaveDataResponse>(query, data);
		}
	}
}
