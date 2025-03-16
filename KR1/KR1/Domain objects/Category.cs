namespace KR1.DomainObjects
{

    internal class Category(int id, string name, bool isIncome)
    {
        public int Id { get; private set; } = id;
        public bool IsIncome { get; private set; } = isIncome;
        public string Name { get; private set; } = name;
    }
}
