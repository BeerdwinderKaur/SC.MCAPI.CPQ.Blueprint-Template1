using Domain.Model;

namespace Domain.Infrastructure.Interfaces
{
    public interface IGreetingsRepository
    {
        GreetingsModel GetGreetings(GreetingsModel greetings);
    }
}
