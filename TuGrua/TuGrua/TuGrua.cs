using System;
using System.IO;
using System.Json;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TuGrua.Core.Entities;
using TuGrua.Core.Enums;
using TuGrua.Extensions;
using Xamarin.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TuGrua.Core.Backend.Socket;
using Quobject.SocketIoClientDotNet.Client;

namespace TuGrua
{
    /// <summary>
	/// A delegate type for hooking up change notifications.
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	public delegate void ChangedEventHandler(object sender, JToken e);

    public class App : Application
	{
        private Core.Backend.Socket.IO socketManager;
        public static Socket io;
        public static TuGrua.Core.Enums.SocketStatus SocketStatus;

        public static event ChangedEventHandler OnSocketEvent;

        // Invoke the Changed event; called whenever list changes
        protected void OnEvent(JToken e)
        {
            if (OnSocketEvent != null)
                OnSocketEvent(this, e);
        }

        public App ()
        {
            InitializeSocket();

            // The root page of your application
            TuGrua.MainPage mainPage = new TuGrua.MainPage();
            NavigationPage.SetHasNavigationBar(mainPage, false);
            MainPage = new NavigationPage(mainPage);
        }

        public void InitializeSocket()
        {
            socketManager = new Core.Backend.Socket.IO("http://tugruacobackend.cloudapp.net:3000/");
            io = socketManager.GetSocket();

            io.On(Socket.EVENT_CONNECT, (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "EVENT_CONNECT", Data = "Conectado" }));
                SocketStatus = TuGrua.Core.Enums.SocketStatus.Connected;
            });

            io.On(Socket.EVENT_DISCONNECT, (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "EVENT_DISCONNECT", Data = "Desconectado" }));
                SocketStatus = TuGrua.Core.Enums.SocketStatus.Disconnected;
            });

            io.On(Socket.EVENT_RECONNECTING, (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "EVENT_RECONNECTING", Data = "Reconectando..." }));
                SocketStatus = TuGrua.Core.Enums.SocketStatus.Reconnecting;
            });

            io.On(Socket.EVENT_RECONNECT_FAILED, (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "EVENT_RECONNECT_FAILED", Data = "Reconexión fallida" }));
                SocketStatus = TuGrua.Core.Enums.SocketStatus.ReconnectFailed;
            });

            io.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "EVENT_CONNECT_ERROR", Data = "Error en la conexión" }));
                SocketStatus = TuGrua.Core.Enums.SocketStatus.ConnectionError;
            });

            io.On(Socket.EVENT_CONNECT_TIMEOUT, (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "EVENT_CONNECT_TIMEOUT", Data = "El servidor no responde" }));
                SocketStatus = TuGrua.Core.Enums.SocketStatus.ConnectionTimeout;
            });

            io.On(Socket.EVENT_RECONNECT_ERROR, (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "EVENT_RECONNECT_ERROR", Data = "Error de reconexión" }));
            });

            io.On(Socket.EVENT_RECONNECT_ATTEMPT, (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "EVENT_RECONNECT_ATTEMPT", Data = "Intento de reconexión" }));
            });

            io.On("Welcome", (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "Welcome", Data = data }));
            });

            io.On("Backend.Message", (data) =>
            {
                OnEvent(JToken.FromObject(new { Command = "Backend.Message", Values = JObject.Parse(data.ToString()) }));
            });
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}

