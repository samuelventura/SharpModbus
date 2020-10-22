using System;
using NUnit.Framework;

namespace SharpModbus.Test.Special
{
    public class Com0ComSlaveTest
    {
        public const string SlaveCOM = "COM99";
        public const string MasterCOM = "COM98";

        private SerialSettings ss(string portName)
        {
            return new SerialSettings { PortName = portName };
        }

        [Test]
        public void TcpOverSerialTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var stream = new ModbusSerialStream(ss(MasterCOM), 400))
            {
                var master = new ModbusMaster(stream, new ModbusTCPProtocol());
                //race condition avoided by access order
                H.SharedSlaveTest(model, master);
            }
        }

        [Test]
        public void TcpOverIsolatedTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var stream = new ModbusIsolatedStream(ss(MasterCOM), 400))
            {
                var master = new ModbusMaster(stream, new ModbusTCPProtocol());
                //race condition avoided by access order
                H.SharedSlaveTest(model, master);
            }
        }

        [Test]
        public void RtuOverSerialTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusRTUScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var master = ModbusMaster.RTU(ss(MasterCOM)))
            {
                //race condition avoided by access order
                H.SharedSlaveTest(model, master);
            }
        }

        [Test]
        public void RtuOverIsolatedTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusRTUScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var master = ModbusMaster.IsolatedRTU(ss(MasterCOM)))
            {
                //race condition avoided by access order
                H.SharedSlaveTest(model, master);
            }
        }

        [Test]
        public void TcpExceptionOverSerialTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var stream = new ModbusSerialStream(ss(MasterCOM), 400))
            {
                var master = new ModbusMaster(stream, new ModbusTCPProtocol());
                var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
                Assert.AreEqual("Modbus exception 2", ex.Message);
                Assert.AreEqual(2, ex.Code);
            }
        }

        [Test]
        public void TcpExceptionOverIsolatedTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var stream = new ModbusIsolatedStream(ss(MasterCOM), 400))
            {
                var master = new ModbusMaster(stream, new ModbusTCPProtocol());
                var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
                Assert.AreEqual("Modbus exception 2", ex.Message);
                Assert.AreEqual(2, ex.Code);
            }
        }

        [Test]
        public void RtuExceptionOverSerialTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusRTUScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var master = ModbusMaster.RTU(ss(MasterCOM)))
            {
                var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
                Assert.AreEqual("Modbus exception 2", ex.Message);
                Assert.AreEqual(2, ex.Code);
            }
        }

        [Test]
        public void RtuExceptionOverIsolatedTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusRTUScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var master = ModbusMaster.IsolatedRTU(ss(MasterCOM)))
            {
                var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
                Assert.AreEqual("Modbus exception 2", ex.Message);
                Assert.AreEqual(2, ex.Code);
            }
        }
    }
}
