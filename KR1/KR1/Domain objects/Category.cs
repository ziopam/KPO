namespace KR1.DomainObjects
{

    internal class Category(Int64 id, string name, bool isIncome)
    {
        public Int64 Id { get; private set; } = id;
        public bool IsIncome { get; set; } = isIncome;
        public string Name { get; set; } = name;
    }
}
