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
        public IActionResult Inbox(int userId)
        {
            //var chat = await _chatService.GetConversations(userId);

            return View();
        }







    }
}
