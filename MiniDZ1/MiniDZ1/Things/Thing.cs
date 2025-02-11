namespace MiniDZ1.Things
{
    internal abstract class Thing : Interfaces.IInventory
    {
        public int Number { get; set; } = -1;
        public String? ThingName { get; protected set; }

        public override String ToString()
        {
            return $"{ThingName} #{Number}";
        }
    }
}
