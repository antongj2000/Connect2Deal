using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<Conversation> FindConversation(int user1, int user2)
        {
            var first_user = Math.Min(user1, user2);
            var second_user = Math.Max(user1, user2);

            var conversation = await mycontext.Conversations.FirstOrDefaultAsync(c => c.User1Id == first_user && c.User2Id == second_user);

            if (conversation == null)
            {
                return null;
            }
            return conversation;
        }


        public async Task CreateConversation()
        {
            
        }

        #endregion


    }
}
