using buddyUp.Models;

namespace buddyUp.Data
{
    public interface IMessageRepository
    {
        Message addMessages(Message message);
        List<Message> getMessagesByChat(int id);
    }
}
