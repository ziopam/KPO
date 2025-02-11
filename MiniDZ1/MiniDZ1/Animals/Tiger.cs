namespace MiniDZ1.Animals
{
    internal class Tiger : Predator
    {
        public Tiger(String nickname, int food, int health, uint cruelty) : base(nickname, food, health, cruelty)
        {
            SpecieName = "Тигр";
        }
    }
}
