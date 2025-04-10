namespace MiniDZ2.Application.Interfaces
{
    public interface IFeedingOrganizationService
    {
        void FeedAnimal(Guid animalId, Guid scheduleId);
    }
}
