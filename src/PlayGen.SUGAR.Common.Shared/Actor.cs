﻿namespace PlayGen.SUGAR.Common
{
	public abstract class Actor
	{
		public int Id { get; set; }

		public string Name { get; set; }

		public abstract Common.ActorType ActorType { get; }
	}
}
