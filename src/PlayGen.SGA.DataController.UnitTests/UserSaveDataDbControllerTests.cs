﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PlayGen.SGA.DataController.Exceptions;
using PlayGen.SGA.DataModel;
using Xunit;

namespace PlayGen.SGA.DataController.UnitTests
{
    public class UserSaveDataDbControllerTests : TestDbController
    {
        #region Configuration
        private readonly UserDbController _userDbController;
        private readonly GameDbController _gameDbController;
        private readonly UserSaveDataDbController _userDataDbController;

        public UserSaveDataDbControllerTests()
        {
            InitializeEnvironment();

            _userDbController = new UserDbController(TestDbController.NameOrConnectionString);
            _gameDbController = new GameDbController(TestDbController.NameOrConnectionString);
            _userDataDbController = new UserSaveDataDbController(NameOrConnectionString);
        }

        private void InitializeEnvironment()
        {
            TestDbController.DeleteDatabase();
        }
        #endregion


        #region Tests
        [Fact]
        public void CreateAndGetUserSaveData()
        {
            string userDataName = "CreateUserData";

            var newSaveData = CreateUserData(userDataName);

            var userDatas = _userDataDbController.Get(newSaveData.GameId, newSaveData.UserId, new string[] { newSaveData.Key });

            int matches = userDatas.Count(g => g.Key == userDataName && g.GameId == newSaveData.GameId && g.UserId == newSaveData.UserId);

            Assert.Equal(matches, 1);
        }

        [Fact]
        public void CreateUserDataWithNonExistingGame()
        {
            string userDataName = "CreateUserDataWithNonExistingGame";

            bool hadException = false;

            try
            {
                CreateUserData(userDataName, -1);
            }
            catch (DuplicateRecordException)
            {
                hadException = true;
            }

            Assert.True(hadException);
        }

        [Fact]
        public void CreateUserDataWithNonExistingUser()
        {
            string userDataName = "CreateUserDataWithNonExistingUser";

            bool hadException = false;

            try
            {
                CreateUserData(userDataName, 0, -1);
            }
            catch (DuplicateRecordException)
            {
                hadException = true;
            }

            Assert.True(hadException);
        }

        [Fact]
        public void GetMultipleUserSaveDatas()
        {
            string[] userDataNames = new[]
            {
                "GetMultipleUserSaveDatas1",
                "GetMultipleUserSaveDatas2",
                "GetMultipleUserSaveDatas3",
                "GetMultipleUserSaveDatas4",
            };

            var doNotFind = CreateUserData("GetMultipleUserSaveDatas_DontGetThis");
            var gameId = doNotFind.GameId;
            var userId = doNotFind.UserId;

            foreach (var userDataName in userDataNames)
            {
                CreateUserData(userDataName, gameId, userId);
            }

            var userDatas = _userDataDbController.Get(gameId, userId, userDataNames);

            var matchingUserSaveDatas = userDatas.Select(g => userDataNames.Contains(g.Key));

            Assert.Equal(matchingUserSaveDatas.Count(), userDataNames.Length);
        }

        [Fact]
        public void GetUserSaveDatasWithNonExistingKey()
        {
            string userDataName = "GetUserSaveDatasWithNonExistingKey";

            var newSaveData = CreateUserData(userDataName);

            var userDatas = _userDataDbController.Get(newSaveData.GameId, newSaveData.UserId, new string[] { "null key" });

            Assert.Empty(userDatas);
        }

        [Fact]
        public void GetUserSaveDatasWithNonExistingGame()
        {
            string userDataName = "GetUserSaveDatasWithNonExistingGame";

            var newSaveData = CreateUserData(userDataName);

            var userDatas = _userDataDbController.Get(-1, newSaveData.UserId, new string[] { userDataName });

            Assert.Empty(userDatas);
        }

        [Fact]
        public void GetUserSaveDatasWithNonExistingUser()
        {
            string userDataName = "GetUserSaveDatasWithNonExistingUser";

            var newSaveData = CreateUserData(userDataName);

            var userDatas = _userDataDbController.Get(newSaveData.GameId, -1, new string[] { userDataName });

            Assert.Empty(userDatas);
        }
        
        [Fact]
        public void SumLongs()
        {
            User user;
            Game game;
            var generatedData = PopulateData("SumLongs", out game, out user);

            var dbResult = _userDataDbController.SumLongs(game.Id, user.Id, "longs");

            var summedValue = generatedData["longs"].Values.Sum(v => Convert.ToInt64(v));

            Assert.Equal(summedValue, dbResult);
        }

        [Fact]
        public void SumFloats()
        {
            User user;
            Game game;
            var generatedData = PopulateData("SumFloats", out game, out user);

            var dbResult = _userDataDbController.SumFloats(game.Id, user.Id, "floats");

            var summedValue = generatedData["floats"].Values.Sum(v => Convert.ToSingle(v));

            Assert.Equal(summedValue, dbResult);
        }

        [Fact]
        public void LatestString()
        {
            User user;
            Game game;
            var generatedData = PopulateData("TryGetLatestString", out game, out user);

            string dbResult;
            bool gotResult = _userDataDbController.TryGetLatestString(game.Id, user.Id, "strings", out dbResult);

            var lastValue = (string)generatedData["strings"].Values[generatedData["strings"].Values.Length - 1];

            Assert.True(gotResult);
            Assert.Equal(lastValue, dbResult);
        }

        [Fact]
        public void LatestBool()
        {
            User user;
            Game game;
            var generatedData = PopulateData("TryGetLatestBool", out game, out user);

            bool dbResult;
            bool gotResult = _userDataDbController.TryGetLatestBool(game.Id, user.Id, "bools", out dbResult);

            var lastValue = (bool)generatedData["bools"].Values[generatedData["bools"].Values.Length - 1];

            Assert.True(gotResult);
            Assert.Equal(lastValue, dbResult);
        }

        [Fact]
        public void SumMissingLongs()
        {
            var dbResult = _userDataDbController.SumLongs(1, 1, "SumMissingLongs");

            Assert.Equal(0, dbResult);
        }

        [Fact]
        public void SumMissingFloats()
        {
            var dbResult = _userDataDbController.SumFloats(1, 1, "SumMissingFloats");

            Assert.Equal(0, dbResult);
        }

        [Fact]
        public void LatestMissingStrings()
        {
            string dbResult;
            bool gotResult = _userDataDbController.TryGetLatestString(1, 1, "LatestMissingStrings", out dbResult);

            Assert.False(gotResult);
        }

        [Fact]
        public void LatestMissingBools()
        {
            bool dbResult;
            bool gotResult = _userDataDbController.TryGetLatestBool(1, 1, "LatestMissingBools", out dbResult);

            Assert.False(gotResult);
        }
        #endregion

        #region Helpers
        private UserData CreateUserData(string key, int gameId = 0, int userId = 0)
        {
            GameDbController gameDbController = new GameDbController(NameOrConnectionString);
            if (gameId == 0)
            {
                Game newgame = new Game
                {
                    Name = key
                };
                gameId = gameDbController.Create(newgame).Id;
            }

            UserDbController userDbController = new UserDbController(NameOrConnectionString);
            if (userId == 0)
            {
                var user = new User
                {
                    Name = key
                };
                userDbController.Create(user);
                userId = user.Id;
            }

            var newUserData = new UserData
            {
                Key = key,
                GameId = gameId,
                UserId = userId,
                Value = key + " value",
                DataType = 0
            };

            return _userDataDbController.Create(newUserData);
        }

        public Dictionary<string, DataParams> PopulateData(string name, out Game game, out User user)
        {
            user = CreateUser(name);
            game = CreateGame(name);

            var dataValues = GenerateDataValues();

            foreach (var kvp in dataValues)
            {
                CreateData(game, user, kvp.Key, kvp.Value.DataType, kvp.Value.Values);
            }

            return dataValues;
        }

        public void CreateData(Game game, User user, string key, DataType type, params object[] values)
        {
            foreach (var value in values)
            {
                var saveData = new UserData
                {
                    UserId = user.Id,
                    User = user,
                    GameId = game.Id,
                    Game = game,

                    Key = key,
                    Value = value.ToString(),
                    DataType = type,
                };

                // Because the tests for these objects rely on their timestamps being different, 
                // their entry into the database needs to be temporally separated.
                // Could rather try sort these values in a linq expression in the test evaluation as the 
                // ticks may vary but this method seems close to what a user would do.
                if (type == DataType.Boolean || type == DataType.String)
                {
                    Thread.Sleep(1000);
                }

                _userDataDbController.Create(saveData);
            }
        }

        public User CreateUser(string name)
        {
            var user = new User
            {
                Name = name,
            };
            _userDbController.Create(user);

            return user;
        }

        public Game CreateGame(string name)
        {
            var game = new Game
            {
                Name = name,
            };
            _gameDbController.Create(game);

            return game;
        }

        public Dictionary<string, DataParams> GenerateDataValues()
        {
            Random random = new Random();

            return new Dictionary<string, DataParams>
        {
            {
                "floats", new DataParams
                {
                    Values = Enumerable
                        .Repeat(0, 10)
                        .Select(i => (random.NextDouble()*1000) - 100)
                        .ToArray().Cast<object>().ToArray(),

                    DataType = DataType.Float,
                }
            },
            {

                "longs", new DataParams
                {
                    Values = Enumerable
                        .Repeat(0, 10)
                        .Select(i => random.Next(-10, 100))
                        .ToArray().Cast<object>().ToArray(),

                    DataType = DataType.Long,
                }
            },
            {
                "strings", new DataParams
                {
                    Values = CultureInfo.GetCultures(CultureTypes.AllCultures)
                        .OrderBy(x => random.Next())
                        .Take(10)
                        .Select(c => c.EnglishName)
                        .ToArray().Cast<object>().ToArray(),

                    DataType = DataType.String,
                }
            },
            {
                "bools", new DataParams
                {
                    Values = Enumerable
                        .Repeat(0, 10)
                        .Select(i => random.Next(0, 2) == 1)
                        .ToArray().Cast<object>().ToArray(),

                    DataType = DataType.Boolean,
                }
            },
        };
        }

        public struct DataParams
        {
            public object[] Values { get; set; }

            public DataType DataType;
        }
        #endregion
    }
}
