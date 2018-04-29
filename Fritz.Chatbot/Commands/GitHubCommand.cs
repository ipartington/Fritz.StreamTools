using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fritz.StreamLib.Core;

namespace Fritz.Chatbot.Commands
{
<<<<<<< HEAD
  public class GitHubCommand : ICommand
  {
	public IChatService ChatService { get; set; }

	public string Name => "github";

	public string Description => "Outputs the URL of Jeff's Github Repository";

	public async Task Execute(string userName, string fullCommandText)
	{
	 	  await ChatService.SendMessageAsync("Jeff's Github repository can by found here: https://github.com/csharpfritz/");
	}
  }
=======
	public class GitHubCommand : ICommand
	{
		public IChatService ChatService { get; set; }

		public string Name => "github";

		public string Description => "Outputs the URL of Jeff's Github Repository";

		public async Task Execute(string userName, string fullCommandText)
		{
			await ChatService.SendMessageAsync("Jeff's Github repository can by found here: https://github.com/csharpfritz/");
		}
	}
>>>>>>> 4c1979d8e168fdfe4c5570807a38998d7322ab6b
}
