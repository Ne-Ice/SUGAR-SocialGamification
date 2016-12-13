﻿// todo remove any references to the contracts project

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NLog;
using PlayGen.SUGAR.Common.Shared;
using PlayGen.SUGAR.Contracts.Shared;
using PlayGen.SUGAR.Core.EvaluationEvents;
using PlayGen.SUGAR.Data.EntityFramework;
using PlayGen.SUGAR.Data.EntityFramework.Exceptions;
using PlayGen.SUGAR.Data.Model;
using PlayGen.SUGAR.Common.Shared.Extensions;

namespace PlayGen.SUGAR.Core.Controllers
{
	public class LeaderboardController : CriteriaEvaluator
	{
        private static Logger Logger = LogManager.GetCurrentClassLogger();

        protected readonly Data.EntityFramework.Controllers.ActorController ActorController;
		protected readonly Data.EntityFramework.Controllers.GroupController GroupController;
		protected readonly Data.EntityFramework.Controllers.UserController UserController;

        // todo replace db controller usage with core controller usage (all cases except for leaderbaordDbController)
		public LeaderboardController(GroupMemberController groupMemberCoreController,
			UserFriendController userFriendCoreController,
            Data.EntityFramework.Controllers.ActorController actorController,
            Data.EntityFramework.Controllers.GroupController groupController,
            Data.EntityFramework.Controllers.UserController userController,
            SUGARContextFactory contextFactory)
			: base(contextFactory, groupMemberCoreController, userFriendCoreController)
		{
			ActorController = actorController;
			GroupController = groupController;
			UserController = userController;
		}

		public List<LeaderboardStandingsResponse> GetStandings(Leaderboard leaderboard, LeaderboardStandingsRequest request)
		{
			if (leaderboard == null)
			{
				throw new MissingRecordException("The provided leaderboard does not exist.");
			}
			if (request.PageLimit <= 0)
			{
				throw new ArgumentException("You must request at least one ranking from the leaderboard.");
			}
			var standings = GatherStandings(leaderboard, request);

            Logger.Info($"{standings?.Count} Standings for Leaderboard: {leaderboard?.Token}");

			return standings;
		}

		protected List<LeaderboardStandingsResponse> GatherStandings(Leaderboard leaderboard, LeaderboardStandingsRequest request)
		{
			var actors = GetActors(request.LeaderboardFilterType, leaderboard.ActorType, request.ActorId);

            var saveDataController = new SaveDataController(ContextFactory, leaderboard.SaveDataCategory);

			List<LeaderboardStandingsResponse> typeResults;

			switch (leaderboard.LeaderboardType)
			{
				case LeaderboardType.Highest:
					typeResults = EvaluateHighest(saveDataController, actors, leaderboard.GameId, leaderboard.SaveDataKey, leaderboard.LeaderboardType, leaderboard.SaveDataType, request);
					break;

				case LeaderboardType.Lowest:
					typeResults = EvaluateLowest(saveDataController, actors, leaderboard.GameId, leaderboard.SaveDataKey, leaderboard.LeaderboardType, leaderboard.SaveDataType, request);
					break;

				case LeaderboardType.Cumulative:
					typeResults = EvaluateCumulative(saveDataController, actors, leaderboard.GameId, leaderboard.SaveDataKey, leaderboard.LeaderboardType, leaderboard.SaveDataType, request);
					break;

				case LeaderboardType.Count:
					typeResults = EvaluateCount(saveDataController, actors, leaderboard.GameId, leaderboard.SaveDataKey, leaderboard.LeaderboardType, leaderboard.SaveDataType, request);
					break;

				case LeaderboardType.Earliest:
					typeResults = EvaluateEarliest(saveDataController, actors, leaderboard.GameId, leaderboard.SaveDataKey, leaderboard.LeaderboardType, leaderboard.SaveDataType, request);
					break;

				case LeaderboardType.Latest:
					typeResults = EvaluateLatest(saveDataController, actors, leaderboard.GameId, leaderboard.SaveDataKey, leaderboard.LeaderboardType, leaderboard.SaveDataType, request);
					break;

				default:
					return null;
			}

			var results = FilterResults(typeResults, request.PageLimit, request.PageOffset, request.LeaderboardFilterType, request.ActorId);

            Logger.Info($"{results?.Count} Standings for Leaderboard: {leaderboard?.Token}");

            return results;
		}

		protected List<ActorResponse> GetActors(LeaderboardFilterType filter, ActorType actorType, int? actorId)
		{
			var actors = Enumerable.Empty<ActorResponse>().ToList();

			switch (filter)
			{
				case LeaderboardFilterType.Top:
					break;
				case LeaderboardFilterType.Near:
					if (!actorId.HasValue)
					{
						throw new ArgumentException("An ActorId has to be passed in order to gather those ranked near them");
					}
					var provided = ActorController.Get(actorId.Value);
					if (provided == null || actorType != ActorType.Undefined && provided.ActorType != actorType)
					{
						throw new ArgumentException("The provided ActorId cannot compete on this leaderboard.");
					}
					break;
				case LeaderboardFilterType.Friends:
					if (!actorId.HasValue)
					{
						throw new ArgumentException("An ActorId has to be passed in order to gather rankings among friends");
					}
					if (actorType == ActorType.Group)
					{
						throw new ArgumentException("This leaderboard cannot filter by friends");
					}
					var user = ActorController.Get(actorId.Value);
					if (user == null || user.ActorType != ActorType.User)
					{
						throw new ArgumentException("The provided ActorId is not an user.");
					}
					break;
				case LeaderboardFilterType.GroupMembers:
					if (!actorId.HasValue)
					{
						throw new ArgumentException("An ActorId has to be passed in order to gather rankings among group members");
					}
					if (actorType == ActorType.Group)
					{
						throw new ArgumentException("This leaderboard cannot filter by group members");
					}

                    // todo this doesn't get the group!
					var group = ActorController.Get(actorId.Value);
					if (group == null || group.ActorType != ActorType.Group)
					{
						throw new ArgumentException("The provided ActorId is not a group.");
					}

                    // todo and then doesn't return the members of that group!
                    // todo what happens in the case where a user is in multiple groups?
					break;
			}

			switch (actorType)
			{
				case ActorType.Undefined:
					actors = ActorController.Get().Select(a => new ActorResponse
					{
						Id = a.Id,
						Name = GetName(a.Id, a.ActorType)
					}).ToList();
					break;

				case ActorType.User:
					actors = UserController.Get().Select(a => new ActorResponse
					{
						Id = a.Id,
						Name = a.Name
					}).ToList();
					break;

				case ActorType.Group:
					actors = GroupController.Get().Select(a => new ActorResponse
					{
						Id = a.Id,
						Name = a.Name
					}).ToList();
					break;
			}
            
            Logger.Debug($"{actors?.Count} Actors for Filter: {filter}, ActorType: {actorType}, ActorId: {actorId}");

            return actors;
		}

		protected string GetName (int id, ActorType actorType)
		{
			switch (actorType)
			{
				case ActorType.User:
					return UserController.Get(id).Name;

				case ActorType.Group:
					return GroupController.Get(id).Name;

				default:
					return "";
			}
		}

		protected List<LeaderboardStandingsResponse> EvaluateHighest(SaveDataController saveDataController, List<ActorResponse> actors, int? gameId, string key, LeaderboardType type, SaveDataType saveDataType, LeaderboardStandingsRequest request)
		{
			List<LeaderboardStandingsResponse> results;
			switch (saveDataType)
			{
				case SaveDataType.Long:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.GetHighestLongs(gameId, r.Id, key, request.DateStart, request.DateEnd).ToString()
					}).ToList();
					break;

				case SaveDataType.Float:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.GetHighestFloats(gameId, r.Id, key, request.DateStart, request.DateEnd).ToString()
					}).ToList();
					break;

				default:
					return null;
			}

			results = results.OrderByDescending(r => float.Parse(r.Value))
						.Where(r => float.Parse(r.Value) > 0).ToList();

            Logger.Debug($"{results?.Count} Actors for GameId: {gameId}, Key: {key}, Leaderboard Type: {type}, Save Data Type: {saveDataType}");

            return results;
		}

		protected List<LeaderboardStandingsResponse> EvaluateLowest(SaveDataController saveDataController, List<ActorResponse> actors, int? gameId, string key, LeaderboardType type, SaveDataType saveDataType, LeaderboardStandingsRequest request)
		{
			List<LeaderboardStandingsResponse> results;
			switch (saveDataType)
			{
				case SaveDataType.Long:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.GetLowestLongs(gameId, r.Id, key, request.DateStart, request.DateEnd).ToString()
					}).ToList();
					break;

				case SaveDataType.Float:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.GetLowestFloats(gameId, r.Id, key, request.DateStart, request.DateEnd).ToString()
					}).ToList();
					break;

				default:
					return null;
			}

			results = results.OrderBy(r => float.Parse(r.Value))
						.Where(r => float.Parse(r.Value) > 0).ToList();

            Logger.Debug($"{results?.Count} Actors for GameId: {gameId}, Key: {key}, Leaderboard Type: {type}, Save Data Type: {saveDataType}");

            return results;
		}

		protected List<LeaderboardStandingsResponse> EvaluateCumulative(SaveDataController saveDataController, List<ActorResponse> actors, int? gameId, string key, LeaderboardType type, SaveDataType saveDataType, LeaderboardStandingsRequest request)
		{
			List<LeaderboardStandingsResponse> results;
			switch (saveDataType)
			{
				case SaveDataType.Long:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.SumLongs(gameId, r.Id, key, request.DateStart, request.DateEnd).ToString()
					}).ToList();
					break;

				case SaveDataType.Float:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.SumFloats(gameId, r.Id, key, request.DateStart, request.DateEnd).ToString()
					}).ToList();
					break;

				default:
					return null;
			}

			results = results.OrderByDescending(r => float.Parse(r.Value))
						.Where(r => float.Parse(r.Value) > 0).ToList();

            Logger.Debug($"{results?.Count} Actors for GameId: {gameId}, Key: {key}, Leaderboard Type: {type}, Save Data Type: {saveDataType}");

            return results;
		}

		protected List<LeaderboardStandingsResponse> EvaluateCount(SaveDataController saveDataController, List<ActorResponse> actors, int? gameId, string key, LeaderboardType type, SaveDataType saveDataType, LeaderboardStandingsRequest request)
		{
			List<LeaderboardStandingsResponse> results;
			switch (saveDataType)
			{
				case SaveDataType.String:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.CountKeys(gameId, r.Id, key, saveDataType, request.DateStart, request.DateEnd).ToString()
					}).ToList();
					break;

				case SaveDataType.Boolean:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.CountKeys(gameId, r.Id, key, saveDataType, request.DateStart, request.DateEnd).ToString()
					}).ToList();
					break;

				default:
					return null;
			}

			results = results.OrderByDescending(r => float.Parse(r.Value))
						.Where(r => float.Parse(r.Value) > 0).ToList();

            Logger.Debug($"{results?.Count} Actors for GameId: {gameId}, Key: {key}, Leaderboard Type: {type}, Save Data Type: {saveDataType}");

            return results;
		}

		protected List<LeaderboardStandingsResponse> EvaluateEarliest(SaveDataController saveDataController, List<ActorResponse> actors, int? gameId, string key, LeaderboardType type, SaveDataType saveDataType, LeaderboardStandingsRequest request)
		{
			List<LeaderboardStandingsResponse> results;
			switch (saveDataType)
			{
				case SaveDataType.String:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.TryGetEarliestKey(gameId, r.Id, key, saveDataType, request.DateStart, request.DateEnd).ToSUGARString()
            }).ToList();
					break;

				case SaveDataType.Boolean:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.TryGetEarliestKey(gameId, r.Id, key, saveDataType, request.DateStart, request.DateEnd).ToSUGARString()
            }).ToList();
					break;

				default:
					return null;
			}

			results = results.OrderBy(r => r.Value)
						.Where(r => DateTime.Parse(r.Value) != default(DateTime)).ToList();

            Logger.Debug($"{results?.Count} Actors for GameId: {gameId}, Key: {key}, Leaderboard Type: {type}, Save Data Type: {saveDataType}");

            return results;
		}

		protected List<LeaderboardStandingsResponse> EvaluateLatest(SaveDataController saveDataController, List<ActorResponse> actors, int? gameId, string key, LeaderboardType type, SaveDataType saveDataType, LeaderboardStandingsRequest request)
		{
			List<LeaderboardStandingsResponse> results;
			switch (saveDataType)
			{
				case SaveDataType.String:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.TryGetLatestKey(gameId, r.Id, key, saveDataType, request.DateStart, request.DateEnd).ToSUGARString()
					}).ToList();
					break;

				case SaveDataType.Boolean:
					results = actors.Select(r => new LeaderboardStandingsResponse
					{
						ActorId = r.Id,
						ActorName = r.Name,
						Value = saveDataController.TryGetLatestKey(gameId, r.Id, key, saveDataType, request.DateStart, request.DateEnd).ToSUGARString()
        }).ToList();
					break;

				default:
					return null;
			}

			results = results.OrderByDescending(r => r.Value)
						.Where(r => DateTime.Parse(r.Value) != default(DateTime)).ToList();

            Logger.Debug($"{results?.Count} Actors for GameId: {gameId}, Key: {key}, Leaderboard Type: {type}, Save Data Type: {saveDataType}");

            return results;
		}

		protected List<LeaderboardStandingsResponse> FilterResults(List<LeaderboardStandingsResponse> typeResults, int limit, int offset, LeaderboardFilterType filter, int? actorId)
		{
			int position;
			switch (filter)
			{
				case LeaderboardFilterType.Top:
					typeResults = typeResults.Skip(offset * limit).Take(limit).ToList();
					position = (offset * limit);
					return typeResults.Select(s => new LeaderboardStandingsResponse
					{
						ActorId = s.ActorId,
						ActorName = s.ActorName,
						Value = s.Value,
						Ranking = ++position
					}).ToList();
				case LeaderboardFilterType.Near:
			        var typeResultList = typeResults as List<LeaderboardStandingsResponse> ?? typeResults.ToList();
			        bool actorCheck = actorId != null && typeResultList.Any(r => r.ActorId == actorId.Value);
					if (actorCheck)
					{
						int actorPosition = typeResultList.TakeWhile(r => r.ActorId != actorId.Value).Count();
						offset += actorPosition / limit;
					}
					typeResults = typeResultList.Skip(offset * limit).Take(limit).ToList();
					position = (offset * limit);
					return typeResults.Select(s => new LeaderboardStandingsResponse
					{
						ActorId = s.ActorId,
						ActorName = s.ActorName,
						Value = s.Value,
						Ranking = ++position
					}).ToList();
				case LeaderboardFilterType.Friends:
					position = (offset * limit);
					var overall = typeResults.Select(s => new LeaderboardStandingsResponse
					{
						ActorId = s.ActorId,
						ActorName = s.ActorName,
						Value = s.Value,
						Ranking = ++position
					});
                    if (actorId != null)
                    {
                        var friends = UserFriendCoreController.GetFriends(actorId.Value).Select(r => r.Id).ToList();
					    friends.Add(actorId.Value);
                        var friendsOnly = overall.Where(r => friends.Contains(r.ActorId));
                        friendsOnly = friendsOnly.Skip(offset * limit).Take(limit);
                        return friendsOnly.ToList();
                    }
                    return Enumerable.Empty<LeaderboardStandingsResponse>().ToList();
				case LeaderboardFilterType.GroupMembers:
					position = (offset * limit);
					var all = typeResults.Select(s => new LeaderboardStandingsResponse
					{
						ActorId = s.ActorId,
						ActorName = s.ActorName,
						Value = s.Value,
						Ranking = ++position
					});
                    if (actorId != null)
                    {
                        var members = GroupMemberCoreController.GetMembers(actorId.Value).Select(r => r.Id);
                        var membersOnly = all.Where(r => members.Contains(r.ActorId));
                        membersOnly = membersOnly.Skip(offset * limit).Take(limit);
                        return membersOnly.ToList();
                    }
                    return Enumerable.Empty<LeaderboardStandingsResponse>().ToList();
                default:
					return null;
			}
        }
	}
}