namespace MiniDZ1.Animals
{
    abstract class Predator : Animal
    {
        public uint Aggressiveness { get; }
        protected Predator(string nickname, int food, int health, uint aggressiveness) : base(nickname, food, health)
        {
            if (aggressiveness > 10)
            {
                throw new System.ArgumentException("Aggressiveness can be only in range [0; 10]", nameof(aggressiveness));
            }

            Aggressiveness = aggressiveness;
        }

        public override string ToString()
        {
            return $"{SpecieName} {Nickname} (#{Number}) с агрессивностью {Aggressiveness} потребляет {Food} кг в сутки";
        }
    }
}
