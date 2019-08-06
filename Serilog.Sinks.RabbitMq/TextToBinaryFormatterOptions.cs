using System;
using System.Text;

namespace Serilog.Sinks.RabbitMq.Client
{
    public class TextToBinaryFormatterOptions
    {
        private Encoding _encoding = Encoding.UTF8;
        private int _stringBuilderPoolMaxCount = 10;
        private int _stringBuilderInitialCapacityBytes = 1024;
        private int _stringBuilderMaximumRetainedCapacityBytes = 5 * 1024;

        public static TextToBinaryFormatterOptions Default { get; } = new TextToBinaryFormatterOptions();

        public Encoding Encoding
        {
            get => _encoding;
            set => _encoding = value ?? throw new ArgumentNullException();
        }

        public int StringBuilderPoolMaxCount
        {
            get => _stringBuilderPoolMaxCount;
            set => _stringBuilderPoolMaxCount = value >= 0 ? value : throw new ArgumentException();
        }

        public int StringBuilderInitialCapacityBytes
        {
            get => _stringBuilderInitialCapacityBytes;
            set => _stringBuilderInitialCapacityBytes = value > 0 ? value : throw new ArgumentException();
        }

        public int StringBuilderMaximumRetainedCapacityBytes
        {
            get => _stringBuilderMaximumRetainedCapacityBytes;
            set => _stringBuilderMaximumRetainedCapacityBytes = value > 0 ? value : throw new ArgumentException();
        }
    }
}