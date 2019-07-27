using System;
using System.IO;
using System.Text;
using Xunit;

namespace Serilog.Sinks.RabbitMq.Tests
{
    public class MemoryStreamReuseTests
    {
        [Fact]
        public void Test1()
        {
            MemoryStream ms = new MemoryStream();

            StreamWriter streamWriter = new StreamWriter(ms, Encoding.UTF8, 1024, true);

            streamWriter.WriteLine("1");
            streamWriter.Flush();

            byte[] array = ms.ToArray();

            ArraySegment<byte> segment = new ArraySegment<byte>(ms.GetBuffer(), 0, (int)ms.Length);

            
        }
    }
}
