﻿using System;
using System.Collections.Generic;
using PlayGen.SUGAR.Client.EvaluationEvents;
using PlayGen.SUGAR.Client.Extensions;
using PlayGen.SUGAR.Client.RequestQueue;
using PlayGen.SUGAR.Contracts;

namespace PlayGen.SUGAR.Client
{
    /// <summary>
    /// Controller that facilitates Match specific operations
    /// </summary>
	public class MatchClient : ClientBase
	{
		private const string ControllerPrefix = "api/match";

		public MatchClient(
			string baseAddress,
			IHttpHandler httpHandler,
			Dictionary<string, string> constantHeaders,
			Dictionary<string, string> sessionHeaders,
			IRequestQueue requestQueue,
			EvaluationNotifications evaluationNotifications)
			: base(baseAddress, httpHandler, constantHeaders, sessionHeaders, requestQueue, evaluationNotifications)
		{
		}

		/// <summary>
		/// Method to create a match for a game a user is currently logged into
		/// </summary>
		/// <returns></returns>
		public MatchResponse Create()
		{
			var query = GetUriBuilder(ControllerPrefix + "/create").ToString();
			return Get<MatchResponse>(query);
		}

        /// <summary>
        /// Method to create a match for a game a user is currently logged into which is also started automatically.
        /// </summary>
        /// <returns></returns>
		public MatchResponse CreateAndStart()
		{
			var query = GetUriBuilder(ControllerPrefix + "/createandstart").ToString();
			return Get<MatchResponse>(query);
		}

        /// <summary>
        /// Start a match for the game the user is currently logged into.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
		public MatchResponse Start(int matchId)
		{
			var query = GetUriBuilder(ControllerPrefix + "/{0}/start").ToString();
			return Get<MatchResponse>(query);
		}

        /// <summary>
        /// Ends a match for the game that the user is currently logged in for.
        /// </summary>
        /// <param name="matchId"></param>
        /// <returns></returns>
		public MatchResponse End(int matchId)
		{
			var query = GetUriBuilder(ControllerPrefix + "/{0}/end", matchId).ToString();
			return Get<MatchResponse>(query);
		}

        /// <summary>
        /// Get a list of matches filtered by a time range.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>A list of <see cref="MatchResponse"/></returns>
        public List<MatchResponse> GetByTime(DateTime? start, DateTime? end)
		{
			var query = GetUriBuilder(ControllerPrefix + "/{0}/{1}", start, end).ToString();
			return Get<List<MatchResponse>>(query);
		}

        /// <summary>
        /// Get a list of matches for a specific game.
        /// </summary>
        /// <param name="gameId"></param>
        /// <returns>A list of <see cref="MatchResponse"/></returns>
		public List<MatchResponse> GetByGame(int gameId)
		{
			var query = GetUriBuilder(ControllerPrefix + "/game/{0}", gameId).ToString();
			return Get<List<MatchResponse>>(query);
		}

        /// <summary>
        /// Get a list of matches for a specific game, filtered by a time range.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>A list of <see cref="MatchResponse"/></returns>
		public List<MatchResponse> GetByGame(int gameId, DateTime? start, DateTime? end)
		{
			var query = GetUriBuilder(ControllerPrefix + "/game/{0}/{1}/{2}", gameId, start, end).ToString();
			return Get<List<MatchResponse>>(query);
		}

        /// <summary>
        /// Get a list of matches that were created by a specific actor.
        /// </summary>
        /// <param name="creatorId"></param>
        /// <returns>A list of <see cref="MatchResponse"/></returns>
		public List<MatchResponse> GetByCreator(int creatorId)
		{
			var query = GetUriBuilder(ControllerPrefix + "/creator/{0}", creatorId).ToString();
			return Get<List<MatchResponse>>(query);
		}

        /// <summary>
        /// Get a list of matches that were created by a specific actor, filtered by a time range.
        /// </summary>
        /// <param name="creatorId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>A list of <see cref="MatchResponse"/></returns>
		public List<MatchResponse> GetByCreator(int creatorId, DateTime? start, DateTime? end)
		{
			var query = GetUriBuilder(ControllerPrefix + "/creator/{0}/{1}/{2}", creatorId, start, end).ToString();
			return Get<List<MatchResponse>>(query);
		}

        /// <summary>
        /// Get a list of matches for a specific game created by a specific actor.
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="creatorId"></param>
        /// <returns>A list of <see cref="MatchResponse"/></returns>
		public List<MatchResponse> GetByGameAndCreator(int gameId, int creatorId)
		{
			var query = GetUriBuilder(ControllerPrefix + "/game/{0}/creator/{1}", gameId, creatorId).ToString();
			return Get<List<MatchResponse>>(query);
		}

        /// <summary>
        /// Get a list of matches for a specific game created by a specific user, filtered by a time range. 
        /// </summary>
        /// <param name="gameId"></param>
        /// <param name="creatorId"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns>A list of <see cref="MatchResponse"/></returns>
		public List<MatchResponse> GetByGameAndCreator(int gameId, int creatorId, DateTime? start, DateTime? end)
		{
			var query = GetUriBuilder(ControllerPrefix + "/game/{0}/creator/{1}/{2}/{3}", gameId, creatorId, start, end).ToString();
			return Get<List<MatchResponse>>(query);
		}

        /// <summary>
        /// Create a new Match Data record.
        /// </summary>
        /// <param name="data"><see cref="EvaluationDataRequest"/> object that holds the details of the new Match Data.</param>
        /// <returns>A <see cref="EvaluationDataResponse"/> containing the new Match Data details.</returns>
        public EvaluationDataResponse AddData(EvaluationDataRequest data)
        {
            var query = GetUriBuilder(ControllerPrefix).ToString();
            return Post<EvaluationDataRequest, EvaluationDataResponse>(query, data);
        }

        /// <summary>
		/// Find a list of all Match Data that match the parameters provided.
		/// </summary>
		/// <param name="matchId">ID of the match.</param>
		/// <param name="keys">Array of Key names.</param>
		/// <returns>A list of <see cref="EvaluationDataResponse"/> which match the search criteria.</returns>
		public IEnumerable<EvaluationDataResponse> GetData(int matchId, string[] keys = null)
        {
            var query = GetUriBuilder(ControllerPrefix)
                .AppendQueryParameter(matchId, "matchId={0}")
                .AppendQueryParameters(keys, "keys={1}")
                .ToString();
            return Get<IEnumerable<EvaluationDataResponse>>(query);
        }
    }
}
