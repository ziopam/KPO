namespace MiniDZ2.Domain.ValueObjects
{
    internal record Gender
    {
        public static readonly Gender Male = new("Самец");
        public static readonly Gender Female = new("Самка");
        public static readonly Gender Unknown = new("Другое");

        public string Value { get; }

        private Gender(string value) => Value = value;
    }
}
