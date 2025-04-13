namespace MiniDZ2.Application.Interfaces
{
    public interface IDeleteAnimalService
    {
        Task DeleteAnimal(Guid id);
    }
}
