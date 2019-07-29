using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace Serilog.Sinks.RabbitMq
{
    public class TextToBinaryFormatterOptions
    {
        public static TextToBinaryFormatterOptions Default { get; } = new TextToBinaryFormatterOptions();

        public Encoding Encoding { get; set; } = Encoding.UTF8;
        public int StringBuilderPoolMaxCount { get; set; } = 10;
        public int StringBuilderInitialCapacityBytes { get; set; } = 1024;
        public int StringBuilderMaximumRetainedCapacityBytes { get; set; } = 5 * 1024;
    }
}