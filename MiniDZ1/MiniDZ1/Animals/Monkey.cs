namespace MiniDZ1.Animals
{
    internal class Monkey : Herbo
    {
        public Monkey(String nickname, int food, int health, uint kindness) : base(nickname, food, health, kindness)
        {
            SpecieName = "Обезьяна";
        }
    }
}
