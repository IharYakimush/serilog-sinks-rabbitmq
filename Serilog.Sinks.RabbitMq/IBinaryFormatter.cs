using Serilog.Events;

namespace Serilog.Sinks.RabbitMq.Client
{
    public interface IBinaryFormatter
    {
        byte[] GetBytes(LogEvent logEvent);
    }
}