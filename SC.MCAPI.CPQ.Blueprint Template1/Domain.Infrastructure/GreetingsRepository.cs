using Domain.Infrastructure.Interfaces;
using Domain.Model;
using System;

namespace Domain.Infrastructure
{
    public class GreetingsRepository : IGreetingsRepository
    {
        public GreetingsModel GetGreetings(GreetingsModel greetings)
        {
            greetings.Greetings = $"Hello World ! Here is your Id :{greetings.Id}";
            return greetings;
        }
    }
}
