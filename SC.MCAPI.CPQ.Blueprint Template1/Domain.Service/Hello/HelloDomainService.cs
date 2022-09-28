using Domain.Infrastructure.Interfaces;
using Domain.Model;
using Domain.Service.Interface;

namespace Domain.Service.Hello
{
    internal class HelloDomainService : IHelloDomainService
    {
        private readonly IGreetingsRepository _greetingsRepository;


        public HelloDomainService(IGreetingsRepository greetingsRepository)
        {
            _greetingsRepository = greetingsRepository;
        }

        public GreetingsModel GetGreetings(GreetingsModel greetings)
        {
            var greetingsModel = _greetingsRepository.GetGreetings(greetings);
            return greetingsModel;
        }

        public GreetingsModel GetNewFeatureGreetings(GreetingsModel greetings)
        {
            var greetingsModel = _greetingsRepository.GetGreetings(greetings);
            greetingsModel.Greetings = $"(Feature Enabled) Hello  {greetingsModel.Name}!";

            return greetingsModel;
        }
    }
}
