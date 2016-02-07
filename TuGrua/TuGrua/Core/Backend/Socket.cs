using System;
using Quobject.SocketIoClientDotNet.Client;

namespace TuGrua.Core.Backend.Socket
{
	public class IO
    {
        private Quobject.SocketIoClientDotNet.Client.Socket socket { get; set; }
        public IO(string url)
        {
            socket = Quobject.SocketIoClientDotNet.Client.IO.Socket(url);
        }

        public Quobject.SocketIoClientDotNet.Client.Socket GetSocket()
        {
            return socket;
        }

        public void Connect()
        {
            socket.Connect();
        }
    }
}

