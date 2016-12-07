﻿using System;
using System.Collections.Generic;
using PlayGen.SUGAR.Client.AsyncRequestQueue;
using PlayGen.SUGAR.Client.EvaluationEvents;
using PlayGen.SUGAR.Client.Extensions;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;

namespace PlayGen.SUGAR.Client
{
	/// <summary>
	/// Controller that facilitates GameData specific operations.
	/// </summary>
	public class GameDataClient : ClientBase
	{
		private const string ControllerPrefix = "api/gamedata";

		public GameDataClient(string baseAddress, IHttpHandler httpHandler, AsyncRequestController asyncRequestController, EvaluationNotifications evaluationNotifications)
			: base(baseAddress, httpHandler, asyncRequestController, evaluationNotifications)
		{
		}

		/// <summary>
		/// Find a list of all GameData that match the <param name="actorId"/>, <param name="gameId"/> and <param name="key"/> provided.
		/// </summary>
		/// <param name="actorId">ID of a User/Group.</param>
		/// <param name="gameId">ID of a Game.</param>
		/// <param name="key">Array of Key names.</param>
		/// <returns>A list of <see cref="SaveDataResponse"/> which match the search criteria.</returns>
		public IEnumerable<SaveDataResponse> Get(int? actorId, int? gameId, string[] key)
		{
			var query = GetUriBuilder(ControllerPrefix)
				.AppendQueryParameter(actorId, "actorId={0}")
				.AppendQueryParameter(gameId, "gameId={0}")
				.AppendQueryParameters(key, "key={0}")
				.ToString();
			return Get<IEnumerable<SaveDataResponse>>(query);
		}

        /// <summary>
        /// Finds a list of GameData with the highest <param name="dataType"/> for each <param name="key"/> provided that matches the <param name="actorId"/> and <param name="gameId"/>.
        /// </summary>
        /// <param name="actorId">ID of a User/Group.</param>
		/// <param name="gameId">ID of a Game.</param>
		/// <param name="key">Array of Key names.</param>
        /// <param name="dataType">Data type of value</param>
        /// <returns>A list of <see cref="SaveDataResponse"/> which match the search criteria.</returns>
	    public IEnumerable<SaveDataResponse> GetHighest(int? actorId, int? gameId, string[] key, SaveDataType dataType)
	    {
            var query = GetUriBuilder(ControllerPrefix + "/highest")
                .AppendQueryParameter(actorId, "actorId={0}")
                .AppendQueryParameter(gameId, "gameId={0}")
                .AppendQueryParameters(key, "key={0}")
                .AppendQueryParameter(dataType, "dataType={0}")
                .ToString();
	        return Get<IEnumerable<SaveDataResponse>>(query);
	    }

		/// <summary>
		/// Create a new GameData record.
		/// </summary>
		/// <param name="data"><see cref="SaveDataRequest"/> object that holds the details of the new GameData.</param>
		/// <returns>A <see cref="SaveDataResponse"/> containing the new GameData details.</returns>
		public SaveDataResponse Add(SaveDataRequest data)
		{
			var query = GetUriBuilder(ControllerPrefix).ToString();
			return Post<SaveDataRequest, SaveDataResponse>(query, data);
		}

		public void AddAsync(SaveDataRequest data, Action<SaveDataResponse> onSuccess, Action<Exception> onError)
		{
            AsyncRequestController.EnqueueRequest(() => Add(data), 
                onSuccess, 
                onError);
		}
	}
}