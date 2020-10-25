using System;
using System.Text;
using NUnit.Framework;

namespace SharpModbus.Test.Special
{
    //dotnet test SharpModbus.Test.Special --filter FullyQualifiedName~TryoutTest
    //to confirm scanner in SharpModbus v1.0.7 included in SharpChannel works across transaction id 0xff
    public class TryoutTest
    {
        [Test]
        public void DefaultTest()
        {
            var socket = Tools.ConnectWithTimeout("127.0.0.1", 9005, 400);
            var stream = new ModbusSocketStream(socket, 400, StreamLog);
            var protocol = new ModbusTCPProtocol();
            protocol.TransactionId = 0xFA;
            using (var master = new ModbusMaster(stream, protocol))
            {
                var count = 10;
                while (--count > 0)
                {
                    master.ReadCoil(1, 4096);
                }
            }
        }

        private void StreamLog(char prefix, byte[] bytes, int count)
        {
            var sb = new StringBuilder();
            sb.Append(prefix);
            for (var i = 0; i < count; i++)
            {
                var b = bytes[i];
                if (i > 0)
                    sb.Append(" ");
                sb.Append(b.ToString("X2"));
            }
            Console.Error.WriteLine("{0} {1}", prefix.ToString(), sb.ToString());
        }
    }
}
