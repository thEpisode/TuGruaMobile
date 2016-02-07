using System;

namespace TuGrua.Core.Enums
{
	public enum SocketStatus
	{
		Connecting,
		Connected,
		FailedToConnect,
		ConnectionError,
		ConnectionTimeout,
		Reconnecting,
		ReconnectFailed,
		Disconnected
	}
}