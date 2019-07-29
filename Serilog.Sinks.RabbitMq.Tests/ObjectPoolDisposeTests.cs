using System;
using System.Collections.Generic;
using Microsoft.Extensions.ObjectPool;
using Xunit;

namespace Serilog.Sinks.RabbitMq.Tests
{
    public class MyClass : IDisposable
    {
        public MyClass()
        {
            Console.WriteLine("Created");
        }
        public void Dispose()
        {
            Console.WriteLine("Disposed");
        }
    }

    public class ObjectPoolDisposeTests
    {
        [Fact]
        public void DoTest()
        {
            ObjectPool<MyClass> pool =
                new DefaultObjectPoolProvider {MaximumRetained = 2}.Create(new DefaultPooledObjectPolicy<MyClass>());

            //ObjectPool<MyClass> pool = new LeakTrackingObjectPool<MyClass>(new DefaultObjectPool<MyClass>(new DefaultPooledObjectPolicy<MyClass>(), 2));
            var o1 = pool.Get();
            var o2 = pool.Get();
            var o3 = pool.Get();
            var o4 = pool.Get();

            pool.Return(o1);
            pool.Return(o2);
            pool.Return(o3);
            var o5 = pool.Get();

            Assert.Equal(o1, o5);
        }        
    }
}