namespace SecondKPOProject
{
    internal static class EngineFactory
    {
        /// <summary>
        /// Creates an engine of the specified type.
        /// </summary>
        /// <param name="engineType">Illustrates the type of engine to create. </param>
        /// <returns>IEngine object</returns>
        /// <exception cref="ArgumentException">Thrown if unkwown engine type is given.</exception>
        public static IEngine CreateEngine(EngineType engineType)
        {
            return engineType switch
            {
                EngineType.HandControl => new HandsControlEngine(),
                EngineType.LegsControl => new LegsControlEngine { PedalSize = new Random().Next(1, 6) },
                _ => throw new ArgumentException("Unknown engine type"),
            };
        }
    }
}
