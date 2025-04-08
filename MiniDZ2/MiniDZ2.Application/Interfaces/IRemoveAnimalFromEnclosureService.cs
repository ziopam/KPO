namespace MiniDZ2.Application.Interfaces
{
    public interface IRemoveAnimalFromEnclosureService
    {
        Task RemoveAnimalAsync(Guid animalId, Guid enclosureId);
    }
}
