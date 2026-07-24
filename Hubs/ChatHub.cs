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
            Console.WriteLine($"JOIN: user={userId}, conv={conversationId}");

            var belongs = await _chatService.UserBelongToConversation(userId, conversationId);
            Console.WriteLine($"BELONGS: {belongs}");

            if (!belongs) return;

            await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation-{conversationId}");
            Console.WriteLine($"ADDED to conversation-{conversationId}");
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

            Console.WriteLine($"SENDING to conversation-{conversationId}");

            await Clients.Group($"conversation-{conversationId}").SendAsync("ReceiveMessage", new
            {
            senderId = userId,
            content = content,
            createdAt = DateTime.UtcNow.ToString("HH:mm")
     });
        }






    }
    }
