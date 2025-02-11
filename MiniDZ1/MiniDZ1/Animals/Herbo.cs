namespace MiniDZ1.Animals
{
    internal abstract class Herbo : Animal
    {
        public uint Kindness { get; }

        protected Herbo(String nickname, int food, int health, uint kindness) : base(nickname, food, health)
        {
            if (kindness > 10)
            {
                throw new System.ArgumentException("Kindness can be only in range [0; 10]", nameof(kindness));
            }

            Kindness = kindness;
        }

        public bool IsInterecative()
        {
            return Kindness > 5;
        }

        public override string ToString()
        {
            return $"{SpecieName} {Nickname} (#{Number}) с добротой {Kindness} потребляет {Food} кг в сутки";
        }
    }
}
