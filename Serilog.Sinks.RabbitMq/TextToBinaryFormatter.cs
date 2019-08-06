using System;
using System.IO;
using System.Text;
using Microsoft.Extensions.ObjectPool;
using Serilog.Events;
using Serilog.Formatting;

namespace Serilog.Sinks.RabbitMq.Client
{
    public class TextToBinaryFormatter : IBinaryFormatter
    {
        private readonly ObjectPool<StringBuilder> stringBuildersPool;

        public TextToBinaryFormatter(
            ITextFormatter formatter,
            TextToBinaryFormatterOptions options = null)
        {
            options = options ?? TextToBinaryFormatterOptions.Default;

            Formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            Encoding = options.Encoding ?? Encoding.UTF8;

            this.stringBuildersPool = new DefaultObjectPool<StringBuilder>(
                new StringBuilderPooledObjectPolicy
                {
                    InitialCapacity = options.StringBuilderInitialCapacityBytes,
                    MaximumRetainedCapacity = options.StringBuilderMaximumRetainedCapacityBytes
                }, options.StringBuilderPoolMaxCount);
        }

        public ITextFormatter Formatter { get; }
        public Encoding Encoding { get; }

        public byte[] GetBytes(LogEvent logEvent)
        {
            StringBuilder sb = this.stringBuildersPool.Get();

            try
            {
                using (TextWriter writer = new StringWriter(sb))
                {
                    this.Formatter.Format(logEvent, writer);
                }

                byte[] body = this.Encoding.GetBytes(sb.ToString());

                return body;
            }
            finally 
            {
                this.stringBuildersPool.Return(sb);
            }           
        }
    }
}