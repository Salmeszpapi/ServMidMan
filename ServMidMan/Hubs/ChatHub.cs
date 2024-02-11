using Microsoft.AspNetCore.SignalR;

namespace ServMidMan.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message)
        {
            Clients.All.SendAsync("ReceiveMessage",user, message);
            //Clients.User("test").SendAsync("ReceiveMessage", user, message);
        }
		public async Task NewProductUpdated(int productId)
		{
			Clients.All.SendAsync("NewProductUpdated", productId);
			//Clients.User("test").SendAsync("ReceiveMessage", user, message);
		}
	}
}
