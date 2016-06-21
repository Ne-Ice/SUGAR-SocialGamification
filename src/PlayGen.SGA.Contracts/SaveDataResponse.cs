﻿namespace PlayGen.SGA.Contracts
{
    /// <summary>
    /// Encapsulates savedata details from the server.
    /// </summary>
    public class SaveDataResponse
    {
        public int ActorId { get; set; }

        public int GameId { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }

        public DataType DataType { get; set; }
    }
}
