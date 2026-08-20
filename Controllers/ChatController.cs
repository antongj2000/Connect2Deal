using Connect2Deal.Constants;
using Connect2Deal.Hubs;
using Connect2Deal.Models;
using Connect2Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;


namespace Connect2Deal.Controllers
{

    [Authorize]
    public class ChatController : Controller
    {

        private readonly ChatService _chatService;
        private readonly ListingService _listingService;
        private readonly IHubContext<ChatHub> _hub;

        public ChatController(ChatService chatService, ListingService listingService, IHubContext<ChatHub> hub)
        {
            _chatService = chatService;
            _listingService = listingService;
            _hub = hub;
        }

        [HttpGet]
        public async Task<IActionResult> Inbox()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var conversations = await _chatService.GetConversations(userId);
            var unread = new Dictionary<int, int>();
            foreach (var conversation in conversations)
            { 
                unread[conversation.Id] = await _chatService.CountUnreadMessages(conversation.Id, userId);
            }
            ViewData["Unread"] = unread;
            return View(conversations);
        }

        [HttpGet]
        public async Task<IActionResult> Conversation(int id)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var conversation = await _chatService.GetConversationById(id);
            if (conversation == null)
            {
                return NotFound();
            }

            if (conversation.User1Id != userId && conversation.User2Id != userId)
            {
                return Forbid();
            }

            var otherUser = conversation.User1Id == userId ? conversation.User2 : conversation.User1;

            var messages = await _chatService.GetMessagesFromConversation(id);
            await _chatService.MarkAsRead(id, userId);

            ViewData["ConversationId"] = id;
            ViewData["OtherName"] = otherUser?.Username ?? "Conversation";
            ViewData["OtherImage"] = otherUser?.ProfileImage;

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


        [HttpGet]
        public async Task<IActionResult> MessageSeller(int listingId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var listing = await _listingService.GetListingById(listingId);
            if (listing == null)
            {
                return NotFound();
            }

            if (listing.UserId == userId)
            {
                return Forbid();
            }

            return PartialView("_MessageSeller", listing);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendListingInquiry(int listingId, string content)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (string.IsNullOrWhiteSpace(content))
            {
                return BadRequest();
            }

            var listing = await _listingService.GetListingById(listingId);
            if (listing == null)
            {
                return NotFound();
            }

            if (listing.UserId == userId)
            {
                return Forbid();
            }

            var conversation = await _chatService.GetOrCreateConversation(userId, listing.UserId);

            await _chatService.CreateMessage(conversation.Id, userId, content.Trim(),
                                             listingId, MessageTypes.ListingInquiry);

            var cover = listing.ListingImages?.FirstOrDefault(i => i.IsPrimary)
                        ?? listing.ListingImages?.FirstOrDefault();

            await _hub.Clients.Group($"conversation-{conversation.Id}").SendAsync("ReceiveMessage", new
            {
                senderId = userId,
                content = content.Trim(),
                createdAt = DateTime.UtcNow.ToString("HH:mm"),
                listingId = listing.Id,
                listingTitle = listing.Title,
                listingPrice = (listing.Price?.ToString("N0") ?? "0") + " €",
                listingImage = cover?.ImagePath
            });

            await _hub.Clients.Group($"user-{listing.UserId}")
                .SendAsync("InboxUpdate", new { conversationId = conversation.Id });

            return Json(new { conversationId = conversation.Id });
        }



    }

    }
