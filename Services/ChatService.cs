using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace Connect2Deal.Services
{
    public class ChatService
    {

        private readonly AppDbContext mycontext;

        public ChatService(AppDbContext _mycontext)
        {
            mycontext = _mycontext;
        }


        #region Messaging logic

        public async Task<Conversation?> FindConversation(int user1, int user2)
        {
            var first_user = Math.Min(user1, user2);
            var second_user = Math.Max(user1, user2);

            var conversation = await mycontext.Conversations.FirstOrDefaultAsync(c => c.User1Id == first_user && c.User2Id == second_user);

            return conversation;
        }


        public async Task<Conversation> CreateConversation(int user1, int user2)
        {
            var first_user = Math.Min(user1, user2);
            var second_user = Math.Max(user1, user2);

            var newConversation = new Conversation
            {
                User1Id = first_user,
                User2Id = second_user,

            };
            mycontext.Conversations.Add(newConversation);
            await mycontext.SaveChangesAsync();
            return newConversation;
        }

      
        public async Task<Conversation> GetOrCreateConversation(int user1, int user2)
        {
            var conversation = await FindConversation(user1, user2);

            if (conversation != null)
            {
                return conversation;
            }

            var newConversation = await CreateConversation(user1, user2);

            return newConversation;
        }


        public async Task<Message> CreateMessage(int conId, int senderId, string content)
        {
            var newMessage = new Message
            {
                ConversationId = conId,
                SenderId = senderId,
                Content = content
            };
            mycontext.Messages.Add(newMessage);
            await mycontext.SaveChangesAsync();

            var conversation = await mycontext.Conversations.FindAsync(conId);

            if (conversation != null)
            {
                conversation.LastMessageId = newMessage.Id;
                conversation.LastMessageAt = DateTime.UtcNow;
                await mycontext.SaveChangesAsync();
            }
            return newMessage;
        }


        public async Task<List<Conversation>> GetConversations(int userId)
        {
            return await mycontext.Conversations
                .Where(c => c.User1Id == userId || c.User2Id == userId)
                .Include(c => c.User1)
                .Include(c => c.User2)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync();
        }

        public async Task<List<Message>> GetMessagesFromConversation (int conversationId)
        {
            return await mycontext.Messages
                .Where(m => m.ConversationId == conversationId)
                .Include(m => m.Sender)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }


        //public async Task<List<Listing>> GetAllListings()
        //{
        //    return await mycontext.Listings.Where(u => u.Status == "Active").
        //        Include(l => l.Location).
        //        Include(c => c.Category).
        //        Include(u => u.User).
        //        Include(i => i.ListingImages).
        //        OrderByDescending(u => u.CreatedAt).ToListAsync();
        //}


        #endregion





    }
}
