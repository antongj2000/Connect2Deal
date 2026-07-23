using Connect2Deal.Models;
using Connect2Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace Connect2Deal.Controllers
{

    [Authorize]
    public class ChatController : Controller
    {

        private readonly ChatService _chatService;

        public ChatController(ChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpGet]
        public async Task<IActionResult> Inbox()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var conversations = await _chatService.GetConversations(userId);
            return View(conversations);
        }


        //SELECT * FROM conversations 
        //WHERE user1_id = 5 OR user2_id = 5
        //ORDER BY last_message_at DESC

        [HttpGet]
        public async Task<IActionResult> Conversation(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var belongs = await _chatService.UserBelongToConversation(userId, id);
            if (!belongs)
            {
                return Forbid();
            }

            var messages = await _chatService.GetMessagesFromConversation(id);
            return View(messages);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Send(int conversationId, string content)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var belongs = await _chatService.UserBelongToConversation(userId, conversationId);
            if (!belongs)
            {
                return Forbid();
            }

            await _chatService.CreateMessage(conversationId, userId, content);
            return RedirectToAction("Conversation", new { id = conversationId });
        }


        public async Task<IActionResult> StartConversation(int sellerId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _chatService.GetOrCreateConversation(userId, sellerId);

            return RedirectToAction("Conversation", new { id = conversation.Id });

        }
    }





    }
