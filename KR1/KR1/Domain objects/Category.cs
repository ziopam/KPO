namespace KR1.DomainObjects
{

    internal class Category(int id, string name, bool isIncome)
    {
        public int Id { get; private set; } = id;
        public bool IsIncome { get; set; } = isIncome;
        public string Name { get; set; } = name;
    }
}
