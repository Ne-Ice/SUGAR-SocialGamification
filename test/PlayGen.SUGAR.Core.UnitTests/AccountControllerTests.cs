﻿using PlayGen.SUGAR.Core.Controllers;
using PlayGen.SUGAR.Data.EntityFramework;
using PlayGen.SUGAR.Data.Model;
using Xunit;

using DbControllerLocator = PlayGen.SUGAR.Data.EntityFramework.UnitTests.ControllerLocator;


namespace PlayGen.SUGAR.Core.UnitTests
{
    [Collection("Project Fixture Collection")]
    public class AccountControllerTests
    {
        #region Configuration
        private readonly AccountController _accountController = ControllerLocator.AccountController;
        private readonly AccountSourceController _accountSourceController = ControllerLocator.AccountSourceController;
        //private readonly Data.EntityFramework.Controllers.UserController _userController = DbControllerLocator.UserController;
        //private readonly Data.EntityFramework.Controllers.GameController _gameController = DbControllerLocator.GameController;
        #endregion

        #region Tests

        [Fact]
        public void CanAutoRegisterOnNonExistentUserLogin()
        {
            var source = _accountSourceController.Create(new AccountSource()
            {
                RequiresPassword = false,
                Token = "testSource",
                ApiSecret = "",
                UsernamePattern = "",
                Description = "",
                AutoRegister = true
            }); 




            var toVerify = new Account()
            {
                 Name = "testUser",
                 Password = "TestPass"
            };

            var response = _accountController.Authenticate(toVerify, source.Token);

            Assert.Equal(toVerify.Name, response.Name);
            Assert.True(response.Id > 0);
        }

        #endregion
    }
}
