namespace MiniDZ1.Animals
{
    internal abstract class Animal(String nickName, int food, int health) : Interfaces.IAlive, Interfaces.IInventory
    {
        public String? SpecieName { get; protected set; }
        public String Nickname { get; } = nickName;
        public int Food { get; } = food;
        public int Health { get; } = health;
        public int Number { get; set; } = -1;
    }
}
