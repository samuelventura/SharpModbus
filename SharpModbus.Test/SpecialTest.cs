using System;
using System.Net;
using NUnit.Framework;

namespace SharpModbus.Test
{
    public class SpecialTest
    {
        [Test]
        public void TransactionIdWrapTest()
        {
            var model = new ModbusModel();
            var protocol = new ModbusTCPProtocol();
            var stream = new StreamModel(model, new ModbusTCPScanner());
            var master = new ModbusMaster(stream, protocol);
            Assert.AreEqual(0, protocol.TransactionId);
            master.WriteCoil(0, 0, false);
            Assert.AreEqual(1, protocol.TransactionId);
            //ensure TransactionId wraps around 0xFFFF
            protocol.TransactionId = 0xFFFF;
            master.WriteCoil(0, 0, false);
            Assert.AreEqual(0, protocol.TransactionId);
            master.WriteCoil(0, 0, false);
            Assert.AreEqual(1, protocol.TransactionId);
        }

        [Test]
        public void TcpRetryAfterTimeoutTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using (var server = new TcpServerModel(model, scanner))
            using (var master = ModbusMaster.TCP(IPAddress.Loopback.ToString(), server.Port, 50))
            {
                server.delay = 100;
                Assert.Throws(typeof(Exception), () => master.WriteCoil(0, 0, false));
                //client side delay needed to eat all server side delay otherwise
                //the added socket stream discard on write will still throw tid mismatch
                System.Threading.Thread.Sleep(100);
                server.delay = 0;
                master.WriteCoil(0, 0, false);
            }
        }
    }
}
