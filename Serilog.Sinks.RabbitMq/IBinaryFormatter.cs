using Serilog.Events;

namespace Serilog.Sinks.RabbitMq
{
    public interface IBinaryFormatter
    {
        byte[] GetBytes(LogEvent logEvent);
    }
}