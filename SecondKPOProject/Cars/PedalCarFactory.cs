namespace SecondKPOProject
{
    /// <summary>
    /// Factory for creating pedal cars. Sends the created car to the receiver.
    /// </summary>
    /// <param name="receiver"></param>
    internal class PedalCarFactory(ICarReceiver receiver)
    {
        private readonly ICarReceiver _receiver = receiver;

        /// <summary>
        /// Creates a car for the receiver.
        /// </summary>
        public void CreateCarForReceiver()
        {
            _receiver.ReceiveCar(new Car(EngineType.LegsControl));
        }
    }
}
