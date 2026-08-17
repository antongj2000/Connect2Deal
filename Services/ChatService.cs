using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;
using System.Security.Claims;

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

        public async Task<bool> UserBelongToConversation (int userId, int conveId)
        {

            var check = await mycontext.Conversations.FirstOrDefaultAsync
                (x => x.Id == conveId && (x.User1Id == userId || x.User2Id == userId));

            if (check == null)
            {
                return false;
            }

            return true;
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


        public async Task MarkAsRead (int convId, int userId)
        {

            var messages = await mycontext.Messages.Where(x=>x.ConversationId == convId
                                                    && x.ReadAt == null && x.SenderId != userId).ToListAsync();

            foreach (var mssg in messages)
            {
                mssg.ReadAt = DateTime.UtcNow;
            }

            await mycontext.SaveChangesAsync();
        }


        public async Task<int> CountUnreadMessages(int convId, int userId)
        {
            return await mycontext.Messages.CountAsync(x => x.SenderId != userId && x.ConversationId == convId && x.ReadAt == null);
        }

        #endregion


        #region Inbox live chat logig

        public async Task<int?> GetOtherUserId(int conversationId, int myUserId)
        {
            var conversation = await mycontext.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId);

            if (conversation == null)
            {
                return null;
            }

            if (conversation.User1Id == myUserId)
            {
                return conversation.User2Id;
            }
            else
            {
                return conversation.User1Id;
            }
        }


        #endregion


        #region Count all unreaded messages

        public async Task<int> CountAllUnreadMessages (int userId)
        {
            int allMessages = await mycontext.Messages.CountAsync(x=>x.SenderId != userId && x.ReadAt == null
            && (x.Conversation.User1Id == userId || x.Conversation.User2Id == userId));
            return allMessages;
        }


        #endregion



    }
}
