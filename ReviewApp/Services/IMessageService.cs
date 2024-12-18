using System.Collections.Generic;
using System.Threading.Tasks;
using CommonModel;
using ReviewApp.Repository;

namespace ReviewApp.Services
{
    public interface IMessageService
    {
        Task<List<Message>> GetMessagesAsync(int senderId, int receiverId);
        Task<List<Message>> GetMessagesAsync(int senderId);
        Task<Message> GetMessageAsync(int messageId);
        Task<int> AddMessageAsync(Message message);
        Task UpdateMessageAsync(Message message);
        Task DeleteMessageAsync(int messageId);
    }

    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;

        public MessageService()
        {
            _messageRepository = new MessageApiClient();
        }

        public async Task<List<Message>> GetMessagesAsync(int senderId)
        {
            return await _messageRepository.GetMessagesByUserIdAsync(senderId);
        }
        public async Task<List<Message>> GetMessagesAsync(int senderId, int receiverId)
        {
            return await _messageRepository.GetConverstationAsync(senderId, receiverId);
        }

        public async Task<Message> GetMessageAsync(int messageId)
        {
            return await _messageRepository.GetMessageByIdAsync(messageId);
        }

        public async Task<int> AddMessageAsync(Message message)
        {
            return await _messageRepository.AddMessageAsync(message);
        }

        public async Task UpdateMessageAsync(Message message)
        {
            await _messageRepository.UpdateMessageAsync(message);
        }

        public async Task DeleteMessageAsync(int messageId)
        {
            await _messageRepository.DeleteMessageAsync(messageId);
        }
    }
}
