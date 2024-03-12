using Microsoft.AspNetCore.SignalR;
using MySqlX.XDevAPI;
using ServMidMan.Helper;
using System.Collections.Generic;

namespace ServMidMan.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string myClientId;
        public ChatHub(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public ChatHub()
        {
        }
        public async Task SendMessage(string userid, string SenderId)
		{
            userid = 35.ToString();
            string signalrUserId;
            SiteGuardian.ClientWithSignalRKey.TryGetValue(userid, out signalrUserId);
            if(signalrUserId != null)
            {
                await Clients.Client(signalrUserId).SendAsync("ReceiveMessage", SenderId);
            }
        }

        public async Task NewProductUpdated(string newProductID, string message)
        {
            await Clients.All.SendAsync("ChangeColor", newProductID); // Or any other color you want to broadcast
        }
        public async Task NewChatIncomed(string userId)
        {
            await Clients.All.SendAsync("ChangeColor"); // Or any other color you want to broadcast
        }

        public override async Task OnConnectedAsync()
        {
            // Get the connection ID of the newly connected client
            string connectionId = Context.ConnectionId;
            // You can store this connectionId in your application for later use if needed

            await base.OnConnectedAsync();
        }
        public async Task GetClientSignalRId()
        {
            string clientId = Context.ConnectionId;
            //_clientDictionaryService.AddClientId(clientId, "AssociatedData");
            myClientId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            if(myClientId is null)
            {
                return;
            }
            if (SiteGuardian.ClientWithSignalRKey.ContainsKey(myClientId))
            {
                // Update the value for an existing key
                SiteGuardian.ClientWithSignalRKey[myClientId] = clientId;
            }
            else
            {
                SiteGuardian.ClientWithSignalRKey.TryAdd(myClientId, clientId);
            }


        }

    }
}
