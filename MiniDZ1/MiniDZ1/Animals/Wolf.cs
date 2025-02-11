namespace MiniDZ1.Animals
{
    internal class Wolf : Predator
    {
        public Wolf(String nickname, int food, int health, uint cruelty) : base(nickname, food, health, cruelty)
        {
            SpecieName = "Волк";
        }
    }
}
