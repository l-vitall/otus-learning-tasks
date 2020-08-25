using System.Threading.Tasks;

namespace Calories.Common.Services
{
    public interface IKafkaProducerService
    {
        Task SendMessage(string messageTopic, string messageKund, string messageBody);
    }
}