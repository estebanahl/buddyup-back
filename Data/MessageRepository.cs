using buddyUp.Models;
using Microsoft.EntityFrameworkCore;

namespace buddyUp.Data
{
    public class MessageRepository : IMessageRepository
    {
        private readonly ApplicationDbContext _context;

        public MessageRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Message addMessages(Message? message)
        {      
            _context.Message.Add(message);
            _context.SaveChanges();
            return message;
        }

        public List<Message> getMessagesByChat(int id)
        {
            return _context.Message.Where(m => m.chatId == id).ToList();
        }
    }
}
