namespace MiniDZ2.Application.DTOs
{
    public class CreateAnimalDto
    {
        public required string Name { get; set; }
        public required string Species { get; set; }
        public DateTime BirthDate { get; set; }
        public required string Gender { get; set; }
        public required string FavoriteFood { get; set; }
        public required bool IsDangerous { get; set; }
    }
}
