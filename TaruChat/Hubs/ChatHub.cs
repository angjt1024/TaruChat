using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace SignalRChat.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(string user, string message, string profile, string sentDate)
        {
            string chatId = Context.GetHttpContext().Request.Query["chatId"];

            await Clients.Caller.SendAsync("ReceiveMessage", user, message, sentDate, "caller");
            await Clients.OthersInGroup(chatId).SendAsync("ReceiveMessage", user, message, sentDate, "others", profile);
            await Clients.All.SendAsync("UpdateStatus", chatId, user, message);
            await Clients.Others.SendAsync("UpdateNotify", chatId);

        }

        public async Task SendImage(string user, string url, string profile, string sentDate)
        {
            string chatId = Context.GetHttpContext().Request.Query["chatId"];

            await Clients.Caller.SendAsync("ReceiveImage", user, url, sentDate, "caller");
            await Clients.OthersInGroup(chatId).SendAsync("ReceiveImage", user, url, sentDate, "others", profile);
            await Clients.All.SendAsync("UpdateStatus", chatId, user, "Sent an image");
            await Clients.Others.SendAsync("UpdateNotify", chatId);
        }
        public async Task SendDocument(string user, string document, string profile, string sentDate)
        {
            string chatId = Context.GetHttpContext().Request.Query["chatId"];

            await Clients.Caller.SendAsync("ReceiveDocument", user, document, sentDate, "caller");
            await Clients.OthersInGroup(chatId).SendAsync("ReceiveDocument", user, document, sentDate, "others", profile);
            await Clients.All.SendAsync("UpdateStatus", chatId, user, "Sent a document");
            await Clients.Others.SendAsync("UpdateNotify", chatId);
        }

        public async Task SendVideo(string user, string id, string profile, string sentDate)
        {
            string chatId = Context.GetHttpContext().Request.Query["chatId"];

            await Clients.Caller.SendAsync("ReceiveVideo", user, id, sentDate, "caller");
            await Clients.OthersInGroup(chatId).SendAsync("ReceiveVideo", user, id, sentDate, "others", profile);
            await Clients.All.SendAsync("UpdateStatus", chatId, user, "Sent a Youtube Video");
            await Clients.Others.SendAsync("UpdateNotify", chatId);
        }

        public async Task SendAudio(string user, string audio, string profile, string sentDate)
        {
            string chatId = Context.GetHttpContext().Request.Query["chatId"];

            await Clients.Caller.SendAsync("ReceiveAudio", user, audio, sentDate, "caller");
            await Clients.OthersInGroup(chatId).SendAsync("ReceiveAudio", user, audio, sentDate, "others", profile);
            await Clients.All.SendAsync("UpdateStatus", chatId, user, "Sent an audio");
            await Clients.Others.SendAsync("UpdateNotify", chatId);
        }


        public async Task SendCamVideo(string user, string video, string profile, string sentDate)
        {
            string chatId = Context.GetHttpContext().Request.Query["chatId"];

            await Clients.Caller.SendAsync("ReceiveCamVideo", user, video, sentDate, "caller");
            await Clients.OthersInGroup(chatId).SendAsync("ReceiveCamVideo", user, video, sentDate, "others", profile);
            await Clients.All.SendAsync("UpdateStatus", chatId, user, "Sent a video");
            await Clients.Others.SendAsync("UpdateNotify", chatId);
        }
        // ----------------------------------------------------------------------------------------
        // Connected
        // ----------------------------------------------------------------------------------------

        public override async Task OnConnectedAsync()
        {
            string id = Context.ConnectionId;

            string chatId = Context.GetHttpContext().Request.Query["chatId"];

            await Groups.AddToGroupAsync(id, chatId);

            //await ChatConnected();
            await base.OnConnectedAsync();
        }
        private async Task ChatConnected()
        {
            string id = Context.ConnectionId;
            string chatId = Context.GetHttpContext().Request.Query["chatId"];

            await Groups.AddToGroupAsync(id, chatId);
            await Clients.Group(chatId).SendAsync("UpdateStatus", $"<b>{id}</b> joined");

        }

        // ----------------------------------------------------------------------------------------
        // Disconnected
        // ----------------------------------------------------------------------------------------


        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string page = Context.GetHttpContext().Request.Query["page"];

            await base.OnDisconnectedAsync(exception);
        }




    }
}