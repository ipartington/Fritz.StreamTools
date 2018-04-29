using Fritz.StreamLib.Core;
using Fritz.Twitch;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TwitchLib;
using TwitchLib.Events.Client;
using TwitchLib.Extensions.Client;
using TwitchLib.Models.API.v5.Streams;
using TwitchLib.Models.Client;
using TwitchLib.Services;

namespace Fritz.StreamTools.Services
{


	public class TwitchService : IHostedService, IStreamService, IChatService
	{

		private IConfiguration Configuration { get; }
		public ILogger Logger { get; }

		private readonly Proxy Proxy;

		private ChatClient _ChatClient;

		public event EventHandler<ServiceUpdatedEventArgs> Updated;
		public event EventHandler<ChatMessageEventArgs> ChatMessage;
		public event EventHandler<ChatUserInfoEventArgs> UserJoined;
		public event EventHandler<ChatUserInfoEventArgs> UserLeft;

		public TwitchService(IConfiguration config, ILoggerFactory loggerFactory, Fritz.Twitch.Proxy proxy, Fritz.Twitch.ChatClient chatClient)
		{
			this.Configuration = config;
			this.Logger = loggerFactory.CreateLogger("StreamServices");
			this.Proxy = proxy;
			this._ChatClient = chatClient;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			await StartTwitchMonitoring();
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return StopTwitchMonitoring();
		}

		public static int _CurrentFollowerCount;
		public int CurrentFollowerCount
		{
			get { return _CurrentFollowerCount; }
			internal set { _CurrentFollowerCount = value; }
		}

		public static int _CurrentViewerCount;
<<<<<<< HEAD
		private Timer _Timer;
		private TwitchClient _TwitchClient;
		private Streams.V5 _TwitchStream;

		internal Streams.V5 TwitchStream
		{
			get
			{
				if (_TwitchStream == null)
				{
					_TwitchStream = CreateTwitchStream();
				}
				return _TwitchStream;
			}
		}

		public int CurrentViewerCount { get { return _CurrentViewerCount; } }

		private string ClientId { get { return Configuration["StreamServices:Twitch:ClientId"]; } }

		private string Channel { get { return Configuration["StreamServices:Twitch:Channel"]; } }

		private string ChannelId { get { return Configuration["StreamServices:Twitch:UserId"]; } }

		private string ChatToken { get { return Configuration["StreamServices:Twitch:ChatToken"]; } }

		private string ChatBotName { get { return Configuration["StreamServices:Twitch:ChatBotName"]; } }

		public string Name { get { return "Twitch"; } }

		public TimeSpan? Uptime
		{
			get
			{

				var sw = Stopwatch.StartNew();
				var uptime = TwitchProxy.GetUptimeForStream(Channel, ClientId);
				Logger.LogInformation($"Twitch Uptime took: {sw.ElapsedMilliseconds}ms");
				return uptime;

			}
		}

		public bool IsAuthenticated => ChatToken != null;
=======

		public int CurrentViewerCount { get { return _CurrentViewerCount; } }

		public string Name { get { return "Twitch"; } }

		public ValueTask<TimeSpan?> Uptime() => Proxy.Uptime();

		public bool IsAuthenticated => true;
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b

		private Task StartTwitchMonitoring()
		{
<<<<<<< HEAD
			var api = new TwitchLib.TwitchAPI(clientId: ClientId, accessToken: ChatToken);
			Service = new FollowerService(api);
			Service.SetChannelByName(Channel);
			var svcTask = Task.Run( async () => {

				await Service.StartService();
				Logger.LogTrace("Connected to Twitch Service");
				Service.OnNewFollowersDetected += Service_OnNewFollowersDetected;

			});

			var viewerTask = Task.Run(() =>
			{
				TwitchStream.GetStreamByUserAsync(ChannelId).ContinueWith((s) =>
				{
					var myStream = s.Result;
					_CurrentViewerCount = myStream.Stream?.Viewers ?? 0;
					Logger.LogTrace($"Identified {_CurrentViewerCount} viewers on Twitch");
				});

			});

			var followerTask = GetFollowerCount();

			var chatTask = Task.Run(() =>
			{

				if (ChatToken != null)
				{
					var creds = new ConnectionCredentials(ChatBotName, ChatToken);
					_TwitchClient = new TwitchClient(creds, Channel);
					_TwitchClient.OnUserJoined += _TwitchClient_OnUserJoined;
					_TwitchClient.OnUserLeft += _TwitchClient_OnUserLeft;
					_TwitchClient.OnMessageReceived += _TwitchClient_OnMessageReceived;
					_TwitchClient.OnWhisperReceived += _TwitchClient_OnWhisperReceived;
					_TwitchClient.OnConnected += _TwitchClient_OnConnected;
					_TwitchClient.Connect();
				}

			});

			Task.WaitAll(svcTask, viewerTask, followerTask, chatTask);

			_CurrentFollowerCount = followerTask.Result;

			Logger.LogInformation($"Now monitoring Twitch with {_CurrentFollowerCount} followers and {_CurrentViewerCount} Viewers");

			_Timer = new Timer(CheckViews, null, 0, 5000);

			return Task.CompletedTask;
=======

			_ChatClient.Connected += (c, args) => Logger.LogInformation("Now connected to Twitch Chat");
			_ChatClient.NewMessage += _ChatClient_NewMessage;
			_ChatClient.UserJoined += _ChatClient_UserJoined;
			_ChatClient.Init();

			_CurrentFollowerCount = await Proxy.GetFollowerCountAsync();
			Proxy.NewFollowers += Proxy_NewFollowers;
			Proxy.WatchFollowers(10000);

			_CurrentViewerCount = await Proxy.GetViewerCountAsync();
			Proxy.NewViewers += Proxy_NewViewers;
			Proxy.WatchViewers();

			Logger.LogInformation($"Now monitoring Twitch with {_CurrentFollowerCount} followers and {_CurrentViewerCount} Viewers");

		}
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b

		private void _ChatClient_UserJoined(object sender, ChatUserJoinedEventArgs e)
		{
			UserJoined?.Invoke(this, new ChatUserInfoEventArgs
			{
				ServiceName = "Twitch",
				UserName = e.UserName
			});
		}

<<<<<<< HEAD
		private async Task<int> GetFollowerCount()
		{

			var api = new TwitchLib.TwitchAPI(clientId: ClientId, accessToken: ChatToken);
			var v5 = new TwitchLib.Channels.V5(api);

			var follows = await v5.GetChannelFollowersAsync(ChannelId, limit: 1);

			Logger.LogTrace($"Identified {follows.Total} followers on Twitch");
			return follows.Total;
		}

		private void _TwitchClient_OnConnected(object sender, OnConnectedArgs e)
		{
			Logger.LogInformation("Now connected to Twitch Chat Room");
		}

		private void _TwitchClient_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
		{
			ChatMessage?.Invoke(this, new ChatMessageEventArgs
			{
				IsModerator = false,
				IsOwner = false,
				IsWhisper = true,
				Message = e.WhisperMessage.Message,
				ServiceName = "Twitch",
				UserName = e.WhisperMessage.Username
			});
		}

		private void _TwitchClient_OnUserLeft(object sender, OnUserLeftArgs e)
		{

			UserLeft?.Invoke(this, new ChatUserInfoEventArgs
			{
				ChannelId = 0,
				ServiceName = "Twitch",
				UserId = 0,
				UserName = e.Username
			});

		}

		private void _TwitchClient_OnUserJoined(object sender, OnUserJoinedArgs e)
=======
		private void _ChatClient_NewMessage(object sender, NewMessageEventArgs e)
		{

			ChatMessage?.Invoke(this, new ChatMessageEventArgs
			{
				IsModerator = false,
				IsOwner = (_ChatClient.ChannelName == e.UserName),
				IsWhisper = e.IsWhisper,
				Message = e.Message,
				ServiceName = "Twitch",
				UserName = e.UserName
			});
		}

		private void Proxy_NewViewers(object sender, NewViewersEventArgs e)
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
		{
			Interlocked.Exchange(ref _CurrentViewerCount, e.ViewerCount);
			Logger.LogInformation($"New Viewers on Twitch, new total: {_CurrentViewerCount}");

<<<<<<< HEAD
			UserJoined?.Invoke(this, new ChatUserInfoEventArgs
			{
				ServiceName = "Twitch",
				UserName = e.Username
			});

		}

		private void _TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{

			ChatMessage?.Invoke(this, new ChatMessageEventArgs
			{
				IsModerator = e.ChatMessage.IsModerator,
				IsOwner = e.ChatMessage.IsBroadcaster,
				IsWhisper = false,
				Message = e.ChatMessage.Message,
				ServiceName = "Twitch",
				UserName = e.ChatMessage.Username
			});

		}

		private async void CheckViews(object state)
		{
=======
			Updated?.Invoke(this, new ServiceUpdatedEventArgs
			{
				ServiceName = Name,
				NewViewers = _CurrentViewerCount
			});
		}
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b

		private void Proxy_NewFollowers(object sender, NewFollowersEventArgs e)
		{
			Interlocked.Exchange(ref _CurrentFollowerCount, e.FollowerCount);
			Logger.LogInformation($"New Followers on Twitch, new total: {_CurrentFollowerCount}");

			Updated?.Invoke(this, new ServiceUpdatedEventArgs
			{
				ServiceName = Name,
				NewFollowers = _CurrentFollowerCount
			});
		}

<<<<<<< HEAD
				myStream = await TwitchStream.GetStreamByUserAsync(ChannelId);
=======
		private void _TwitchClient_OnConnected(object sender, OnConnectedArgs e)
		{
			Logger.LogInformation("Now connected to Twitch Chat Room");
		}
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b

		private void _TwitchClient_OnWhisperReceived(object sender, OnWhisperReceivedArgs e)
		{
			ChatMessage?.Invoke(this, new ChatMessageEventArgs
			{
				IsModerator = false,
				IsOwner = false,
				IsWhisper = true,
				Message = e.WhisperMessage.Message,
				ServiceName = "Twitch",
				UserName = e.WhisperMessage.Username
			});
		}

		private void _TwitchClient_OnUserLeft(object sender, OnUserLeftArgs e)
		{

			UserLeft?.Invoke(this, new ChatUserInfoEventArgs
			{
				ChannelId = 0,
				ServiceName = "Twitch",
				UserId = 0,
				UserName = e.Username
			});

		}

		private void _TwitchClient_OnUserJoined(object sender, OnUserJoinedArgs e)
		{

			UserJoined?.Invoke(this, new ChatUserInfoEventArgs
			{
				ServiceName = "Twitch",
				UserName = e.Username
			});

		}

<<<<<<< HEAD
		private TwitchLib.Streams.V5 CreateTwitchStream(TwitchLib.TwitchAPI api = null)
		{

			if (api == null)
			{
				api = new TwitchLib.TwitchAPI(clientId: ClientId, accessToken: ChatToken);
			}
			TwitchLib.Streams.V5 v5Stream = null;

			while (v5Stream == null || TwitchService.ErrorsReadingViewers > 0)
			{
				try
				{
					v5Stream = new TwitchLib.Streams.V5(api);
					TwitchService.ErrorsReadingViewers = 0;
				}
				catch (Exception ex)
				{
					TwitchService.ErrorsReadingViewers++;
					Logger.LogError(ex, $"Error reading viewers.. {TwitchService.ErrorsReadingViewers} consecutive errors.  Delaying 2 seconds");
					Task.Delay(2000);
				}
			}

			return v5Stream;
=======
		private void _TwitchClient_OnMessageReceived(object sender, OnMessageReceivedArgs e)
		{

			ChatMessage?.Invoke(this, new ChatMessageEventArgs
			{
				IsModerator = e.ChatMessage.IsModerator,
				IsOwner = e.ChatMessage.IsBroadcaster,
				IsWhisper = false,
				Message = e.ChatMessage.Message,
				ServiceName = "Twitch",
				UserName = e.ChatMessage.Username
			});
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b

		}

		internal void Service_OnNewFollowersDetected(object sender,
		TwitchLib.Events.Services.FollowerService.OnNewFollowersDetectedArgs e)
		{
			Interlocked.Exchange(ref _CurrentFollowerCount, _CurrentFollowerCount + e.NewFollowers.Count);
			Logger.LogInformation($"New Followers on Twitch, new total: {_CurrentFollowerCount}");

			Updated?.Invoke(this, new ServiceUpdatedEventArgs
			{
				ServiceName = Name,
				NewFollowers = _CurrentFollowerCount
			});
		}

		private Task StopTwitchMonitoring()
		{

			Proxy.Dispose();

			return Task.CompletedTask;
		}

		public Task<bool> SendMessageAsync(string message)
		{
<<<<<<< HEAD
			_TwitchClient.SendMessage(message);
=======
			_ChatClient.PostMessage(message);
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
			return Task.FromResult(true);
		}

		public Task<bool> SendWhisperAsync(string userName, string message)
		{

<<<<<<< HEAD
			_TwitchClient.SendWhisper(userName, message);
=======
			_ChatClient.WhisperMessage(message, userName);
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
			return Task.FromResult(true);

		}

		public Task<bool> TimeoutUserAsync(string userName, TimeSpan time)
		{

<<<<<<< HEAD
			_TwitchClient.TimeoutUser(userName, time);
			return Task.FromResult(true);
=======
			//_TwitchClient.TimeoutUser(userName, time);
			//return Task.FromResult(true);
			return Task.FromResult(false);
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b


		}

		public Task<bool> BanUserAsync(string userName)
		{
<<<<<<< HEAD
			_TwitchClient.BanUser(userName);
			return Task.FromResult(true);
=======
			//_TwitchClient.BanUser(userName);
			//return Task.FromResult(true);
			return Task.FromResult(false);

>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
		}

		public Task<bool> UnbanUserAsync(string userName)
		{
<<<<<<< HEAD
			_TwitchClient.UnbanUser(userName);
			return Task.FromResult(true);
=======
			//_TwitchClient.UnbanUser(userName);
			//return Task.FromResult(true);
			return Task.FromResult(false);
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
		}

		internal void MessageReceived(bool isModerator, bool isBroadcaster, string message, string userName)
		{
			ChatMessage?.Invoke(this, new ChatMessageEventArgs
			{
				IsModerator = isModerator,
				IsOwner = isBroadcaster,
				IsWhisper = false,
				Message = message,
				ServiceName = "Twitch",
				UserName = userName
			});

		}

<<<<<<< HEAD
	}

	internal class TwitchProxy
	{

		static DateTime? _CreatedAtUtc = null;
		static DateTime _LastCheckUtc = DateTime.MinValue;


		public static TimeSpan? GetUptimeForStream(string channelName, string clientId)
		{

			if (_LastCheckUtc > DateTime.UtcNow.AddMinutes(-5)) {

				if (!_CreatedAtUtc.HasValue) return null;
				return DateTime.UtcNow.Subtract(_CreatedAtUtc.Value);

			}

			using (var client = new HttpClient())
			{

				var resultString = client.GetStringAsync($"https://api.twitch.tv/kraken/streams/{channelName}?client_id={clientId}")
					.GetAwaiter().GetResult();

				_CreatedAtUtc = ParseStreamForCreatedAt(resultString);

				if (_CreatedAtUtc == null) return null;

				_LastCheckUtc = DateTime.UtcNow;

				return DateTime.UtcNow.Subtract(_CreatedAtUtc.Value);

			}

		}

		internal static DateTime? ParseStreamForCreatedAt(string jsonDoc)
		{

			var jObj = JsonConvert.DeserializeObject<JObject>(jsonDoc);

			var streamData = jObj.GetValue("stream");
			if (streamData == null || !streamData.HasValues)
			{
				return null;
			}

			return streamData["created_at"].Value<DateTime>();

		}

=======
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
	}

}
