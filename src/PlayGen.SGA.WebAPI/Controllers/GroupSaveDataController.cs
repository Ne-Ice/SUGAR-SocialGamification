﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using PlayGen.SGA.DataController;
using PlayGen.SGA.Contracts.Controllers;
using PlayGen.SGA.WebAPI.ExtensionMethods;
using PlayGen.SGA.Contracts;

namespace PlayGen.SGA.WebAPI.Controllers
{
    [Route("api/[controller]")]
    public class GroupSaveDataController : Controller, IGroupSaveDataController
    {
        private GroupSaveDataDbController _groupSaveDataDbController;

        public GroupSaveDataController(GroupSaveDataDbController groupSaveDataDbController)
        {
            _groupSaveDataDbController = groupSaveDataDbController;
        }

        // GET api/groupsavedata?actorId=1&gameId=1&key=key1&key=key2...
        [HttpGet]
        public IEnumerable<SaveDataResponse> Get(int actorId, int gameId, string[] key)
        {
            var data = _groupSaveDataDbController.Get(actorId, gameId, key);
            return data.ToContract();
        }

        // POST api/groupsavedata
        [HttpPost]
        public SaveDataResponse Add([FromBody]SaveDataRequest newData)
        {
            var data = _groupSaveDataDbController.Create(newData.ToGroupModel());
            return data.ToContract();
        }
    }
}
