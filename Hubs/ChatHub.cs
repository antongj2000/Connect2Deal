using Connect2Deal.Models;
using Connect2Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.VisualBasic;
using System.Security.Claims;


namespace Connect2Deal.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {

        private readonly ChatService _chatService;

        public ChatHub(ChatService chatService)
        {
            _chatService = chatService;
        }

        public async Task JoinConversation(int conversationId)
        {
            int userId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var belongs = await _chatService.UserBelongToConversation(userId, conversationId);

            if (!belongs) return;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");
        }


        public async Task SendMessage(int conversationId, string content)
        {
            int userId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var belongs = await _chatService.UserBelongToConversation(userId, conversationId);

            if (!belongs)
            {
                return;
            }

            await _chatService.CreateMessage(conversationId, userId, content);

            await Clients.Group($"conversation-{conversationId}").SendAsync("ReceiveMessage", new
            {
                senderId = userId,
                content = content,
                createdAt = DateTime.UtcNow.ToString("HH:mm")
            });

            var otherUserId = await _chatService.GetOtherUserId(conversationId, userId);

            if (otherUserId == null)
            {
                return;
            }

            await Clients.Group($"user-{otherUserId}").SendAsync("InboxUpdate", new { conversationId });

        }

        public async Task MarkRead(int conversationId)
        {
            int userId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));

            var belongs = await _chatService.UserBelongToConversation(userId, conversationId);
            if (!belongs) return;

            await _chatService.MarkAsRead(conversationId, userId);
        }



        public async Task JoinUserChannel()
        {
            int userId = int.Parse(Context.User.FindFirstValue(ClaimTypes.NameIdentifier));
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
        }



    }
}
