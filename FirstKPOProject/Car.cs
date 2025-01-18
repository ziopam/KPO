
namespace FirstKPOProject;

internal class Car
{
    private static readonly Random _random = new();

    public required int Number { get; set; }
    public Engine Engine { get; init; }

    public Car()
    {
        Engine = new Engine { Size = _random.Next(1, 10) };
    }

    public override string ToString()
    {
        return $"Car with number = {Number}";
    }
}

