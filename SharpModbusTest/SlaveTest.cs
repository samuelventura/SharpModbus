
using System;
using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using SharpModbus;

namespace SharpModbusTest
{
    public class SlaveTest
    {
        [Test]
        public void RtuTest()
        {
            var model = new ModbusModel();
            var stream = new StreamModel(model, new ModbusRTUScanner());
            var master = new ModbusMaster(stream, new ModbusRTUProtocol());
            H.SharedSlaveTest(model, master);
        }

        [Test]
        public void TcpTest()
        {
            var model = new ModbusModel();
            var protocol = new ModbusTCPProtocol();
            var stream = new StreamModel(model, new ModbusTCPScanner());
            var master = new ModbusMaster(stream, protocol);
            Assert.AreEqual(0, protocol.TransactionId);
            master.WriteCoil(0, 0, false);
            Assert.AreEqual(1, protocol.TransactionId);
            H.SharedSlaveTest(model, master);
            //ensure TransactionId wraps around 0xFFFF
            protocol.TransactionId = 0xFFFF;
            master.WriteCoil(0, 0, false);
            Assert.AreEqual(0, protocol.TransactionId);
        }

        [Test]
        public void RtuExceptionTest()
        {
            var model = new ModbusModel();
            var stream = new StreamModel(model, new ModbusRTUScanner());
            var master = new ModbusMaster(stream, new ModbusRTUProtocol());
            var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
            Assert.AreEqual("Modbus exception 2", ex.Message);
            Assert.AreEqual(2, ex.Code);
        }

        [Test]
        public void TcpExceptionTest()
        {
            var model = new ModbusModel();
            var stream = new StreamModel(model, new ModbusTCPScanner());
            var master = new ModbusMaster(stream, new ModbusTCPProtocol());
            var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
            Assert.AreEqual("Modbus exception 2", ex.Message);
            Assert.AreEqual(2, ex.Code);
        }

        [Test]
        public void TcpOverSocketTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using (var server = new TcpServerModel(model, scanner))
            using (var master = ModbusMaster.TCP(IPAddress.Loopback.ToString(), server.Port))
            {
                //race condition avoided by access order
                H.SharedSlaveTest(model, master);
            }
        }

        [Test]
        public void RtuOverSocketTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusRTUScanner();
            using (var server = new TcpServerModel(model, scanner))
            using (var client = new TcpClient())
            {
                client.Connect(IPAddress.Loopback, server.Port);
                var stream = new ModbusSocketStream(client, 400);
                var master = new ModbusMaster(stream, new ModbusRTUProtocol());
                //race condition avoided by access order
                H.SharedSlaveTest(model, master);
            }
        }

        [Test]
        public void TcpExceptionOverSocketTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using (var server = new TcpServerModel(model, scanner))
            using (var master = ModbusMaster.TCP(IPAddress.Loopback.ToString(), server.Port))
            {
                var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
                Assert.AreEqual("Modbus exception 2", ex.Message);
                Assert.AreEqual(2, ex.Code);
            }
        }

        [Test]
        public void RtuExceptionOverSocketTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusRTUScanner();
            using (var server = new TcpServerModel(model, scanner))
            using (var client = new TcpClient())
            {
                client.Connect(IPAddress.Loopback, server.Port);
                var stream = new ModbusSocketStream(client, 400);
                var master = new ModbusMaster(stream, new ModbusRTUProtocol());
                var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
                Assert.AreEqual("Modbus exception 2", ex.Message);
                Assert.AreEqual(2, ex.Code);
            }
        }
    }
}
