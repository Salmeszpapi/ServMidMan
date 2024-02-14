using Microsoft.AspNetCore.SignalR;

namespace ServMidMan.Hubs
{
    public class ChatHub : Hub
    {
		public async Task SendMessage(string user, string message)
		{
			await Clients.All.SendAsync("ReceiveMessage", user, message);
		}

        public async Task NewProductUpdated(string user, string message)
        {
            await Clients.All.SendAsync("ChangeColor", "red"); // Or any other color you want to broadcast
        }
    }
}
