using Domain.Model;

namespace Domain.Service.Interface
{
    public interface IHelloDomainService
    {
        GreetingsModel GetGreetings(GreetingsModel hello);
        GreetingsModel GetNewFeatureGreetings(GreetingsModel hello);
    }
}
