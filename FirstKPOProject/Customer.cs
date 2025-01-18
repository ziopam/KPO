
namespace FirstKPOProject;

internal class Customer
{
    public required string Name { get; set; }

    public Car? Car { get; set; }


    public override string ToString()
    {
        return $"Customer with Name = {Name}" + (Car is null ? "" : " has " + Car.ToString());
    }

}
