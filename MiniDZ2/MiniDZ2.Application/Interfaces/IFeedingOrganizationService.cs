namespace MiniDZ2.Application.Interfaces
{
    public interface IFeedingOrganizationService
    {
        Task FeedAnimal(Guid animalId, Guid scheduleId);
    }
}
