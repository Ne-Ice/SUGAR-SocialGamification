﻿using PlayGen.SUGAR.Client.AsyncRequestQueue;
using PlayGen.SUGAR.Client.EvaluationEvents;

namespace PlayGen.SUGAR.Client
{
	public class SUGARClient
	{
		private readonly IHttpHandler _httpHandler;

		private readonly string _baseAddress;
        private readonly EvaluationNotifications _evaluationNotifications = new EvaluationNotifications();
        private readonly AsyncRequestController _asyncRequestController = new AsyncRequestController();

		private AccountClient _accountClient;
	    private SessionClient _sessionClient;
		private AchievementClient _achievementClient;
		private GameClient _gameClient;
		private GameDataClient _gameDataClient;
		private GroupClient _groupClient;
		private GroupMemberClient _groupMemberClient;
		private UserClient _userClient;
		private UserFriendClient _userFriendClient;
		private ResourceClient _resourceClient;
		private LeaderboardClient _leaderboardClient;
		private SkillClient _skillClient;

		public AccountClient Account			=> _accountClient ?? (_accountClient = new AccountClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
        public SessionClient Session            => _sessionClient ?? (_sessionClient = new SessionClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public AchievementClient Achievement	=> _achievementClient ?? (_achievementClient = new AchievementClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public GameClient Game					=> _gameClient ?? (_gameClient = new GameClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public GameDataClient GameData			=> _gameDataClient ?? (_gameDataClient = new GameDataClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public GroupClient Group				=> _groupClient ?? (_groupClient = new GroupClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public GroupMemberClient GroupMember	=> _groupMemberClient ?? (_groupMemberClient = new GroupMemberClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public UserClient User					=> _userClient ?? (_userClient = new UserClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public UserFriendClient UserFriend		=> _userFriendClient ?? (_userFriendClient = new UserFriendClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public ResourceClient Resource			=> _resourceClient ?? (_resourceClient = new ResourceClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public LeaderboardClient Leaderboard	=> _leaderboardClient ?? (_leaderboardClient = new LeaderboardClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
		public SkillClient Skill				=> _skillClient ?? (_skillClient = new SkillClient(_baseAddress, _httpHandler, _asyncRequestController, _evaluationNotifications));
	    

        // todo possibly pass update event so async requests are either read in and handled there or this creates another thread to poll the async queue
        public SUGARClient(string baseAddress, IHttpHandler httpHandler = null)
		{
			_baseAddress = baseAddress;
			_httpHandler = httpHandler ?? new DefaultHttpHandler();
		}
	}
}
