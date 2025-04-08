namespace MiniDZ2.Application.Interfaces
{
    public interface IAnimalTransferService
    {
        public Task MoveAnimalAsync(Guid animalId, Guid toEnclosureId);
    }
}
