using System;
using System.Collections.Generic;
<<<<<<< HEAD
using System.Linq;
=======
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
using System.Threading.Tasks;
using Fritz.StreamTools.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
<<<<<<< HEAD
=======
using Microsoft.Extensions.Logging;
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b

namespace Fritz.StreamTools.Pages
{
	public class TestTwitchbotModel : PageModel
	{

		public TwitchService Service { get; }
<<<<<<< HEAD

		public TestTwitchbotModel(Services.TwitchService service)
		{
			Service = service;
=======
		public ILogger Logger { get; }
		public Twitch.Proxy TwitchProxy { get; }

		public TestTwitchbotModel(Services.TwitchService service, ILoggerFactory loggerFactory, Twitch.Proxy proxy)
		{
			Service = service;
			this.Logger = loggerFactory.CreateLogger("FritzBot");
			this.TwitchProxy = proxy;
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
		}

		[BindProperty]
		public string Message { get; set; }

<<<<<<< HEAD
		[BindProperty]
		public string UserName { get; set; }

		public void OnGet()
		{

=======

		[BindProperty]
		public string UserName { get; set; }

		public TimeSpan? Uptime { get; set; }

		public async Task OnGet()
		{

			var sw = Stopwatch.StartNew();
			Uptime = await TwitchProxy.Uptime();
			this.Logger.LogInformation($"Get uptime took {sw.ElapsedMilliseconds}ms");

			sw.Restart();
			var api = new TwitchLib.TwitchAPI(clientId: "t7y5txan5q662t7zj7p3l4wlth8zhv");
			var v5Stream = new TwitchLib.Streams.V5(api);
			var myStream = await v5Stream.GetStreamByUserAsync("96909659");
			var createdAt = myStream.Stream?.CreatedAt;
			this.Logger.LogInformation($"Get uptime took {sw.ElapsedMilliseconds}ms");


>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
		}

		public void OnPost()
		{

			Service.MessageReceived(false, false, Message, UserName);

		}

	}
}
