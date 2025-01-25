namespace SecondKPOProject;

/// <summary>
/// Represents a legs controlled engine.
/// </summary>
internal class LegsControlEngine : IEngine
{
    public EngineType EngineType => EngineType.LegsControl;
    public bool CanPersonUseIt(int legsPower) => legsPower > 5;
    public required int PedalSize { get; init; }
}
