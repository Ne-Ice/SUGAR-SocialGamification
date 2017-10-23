﻿using System;
using System.Net;
using PlayGen.SUGAR.Common.Shared.Exceptions;

namespace PlayGen.SUGAR.Client.Exceptions
{
    public class ClientHttpException : SUGARException
    {
		public HttpStatusCode StatusCode { get; }

	    public ClientHttpException(HttpStatusCode statusCode)
	    {
		    StatusCode = statusCode;
	    }

	    public ClientHttpException(HttpStatusCode statusCode, string message) : base(message)
	    {
			StatusCode = statusCode;
		}

		public ClientHttpException(HttpStatusCode statusCode, string message, Exception innerException) : base(message, innerException)
	    {
			StatusCode = statusCode;
		}
	}
}
