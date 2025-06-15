namespace KR3.Shared.Models
{
    public class MessageBusSettings
    {
        public string Host { get; set; } = "rabbitmq";
        public int Port { get; set; } = 5672;
        public string UserName { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string OrderPaymentRequestQueue { get; set; } = "order_payment_requests";
        public string OrderPaymentResponseQueue { get; set; } = "order_payment_responses";

        public TimeSpan RequestedConnectionTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan SocketReadTimeout { get; set; } = TimeSpan.FromSeconds(30);
        public TimeSpan SocketWriteTimeout { get; set; } = TimeSpan.FromSeconds(30);
    }
}
