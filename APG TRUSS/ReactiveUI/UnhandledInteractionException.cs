using System;
using System.Runtime.Serialization;

namespace ReactiveUI
{
	[Serializable]
	internal class UnhandledInteractionException : Exception
	{
		public UnhandledInteractionException()
		{
		}

		public UnhandledInteractionException(string? message) : base(message)
		{
		}

		public UnhandledInteractionException(string? message, Exception? innerException) : base(message, innerException)
		{
		}

		protected UnhandledInteractionException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}