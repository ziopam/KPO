namespace SecondKPOProject
{
    /// <summary>
    /// Enumerates the types of engines.
    /// </summary>
    internal enum EngineType
    {
        HandControl,
        LegsControl,
    }

    /// <summary>
    /// Represents an engine.
    /// </summary>
    internal interface IEngine
    {
        EngineType EngineType { get; }

        /// <summary>
        /// Tells if a person can use the engine.
        /// </summary>
        /// <param name="param">Parameter that helps identify if person can use the engine.</param>
        /// <returns></returns>
        public bool CanPersonUseIt(int param);
    }
}
