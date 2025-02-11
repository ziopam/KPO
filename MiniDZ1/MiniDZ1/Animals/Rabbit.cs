namespace MiniDZ1.Animals
{
    internal class Rabbit : Herbo
    {
        public Rabbit(String nickname, int food, int health, uint kindness) : base(nickname, food, health, kindness)
        {
            SpecieName = "Кролик";
        }
    }
}
