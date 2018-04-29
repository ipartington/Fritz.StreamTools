using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Fritz.StreamLib.Core;

namespace Fritz.Chatbot.Commands
{
	public class UptimeCommand : ICommand
	{
		public IChatService ChatService { get; set; }

		public string Name => "uptime";

		public string Description => "Report how long the stream has been on the air";

		public async Task Execute(string userName, string fullCommandText)
		{


			if (!(ChatService is IStreamService svc))
			{
				return;
			}

<<<<<<< HEAD
			if (svc.Uptime.HasValue)
			{
				await ChatService.SendMessageAsync($"The stream has been up for {svc.Uptime.Value.ToString(@"hh\:mm\:ss")}");
=======
			var uptime = await svc.Uptime();
			if (uptime.HasValue)
			{
				await ChatService.SendMessageAsync($"The stream has been up for {uptime.Value.ToString(@"hh\:mm\:ss")}");
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
			}
			else
			{
				await ChatService.SendMessageAsync("Stream is offline");
			}
		}
	}
}
