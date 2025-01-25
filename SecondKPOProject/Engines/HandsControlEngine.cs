
namespace SecondKPOProject
{
    /// <summary>
    /// Represents a hands controlled engine.
    /// </summary>
    internal class HandsControlEngine : IEngine
    {
        public EngineType EngineType => EngineType.HandControl;

        public bool CanPersonUseIt(int handsPower) => handsPower > 5;
    }
}
