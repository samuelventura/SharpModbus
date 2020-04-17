using System;
using NUnit.Framework;

namespace SharpModbus.Test.Special
{
    public class Com0ComSlaveTest
    {
        public const string SlaveCOM = "COM99";
        public const string MasterCOM = "COM98";

        [Test]
        public void TcpOverSerialTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using (var server = new SerialModel(SlaveCOM, model, scanner))
            using (var stream = new ModbusSerialStream(new SerialSettings(MasterCOM), 400))
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
            using (var stream = new ModbusIsolatedStream(new SerialSettings(MasterCOM), 400))
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
            using (var master = ModbusMaster.RTU(new SerialSettings(MasterCOM)))
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
            using (var master = ModbusMaster.IsolatedRTU(new SerialSettings(MasterCOM)))
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
            using (var stream = new ModbusSerialStream(new SerialSettings(MasterCOM), 400))
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
            using (var stream = new ModbusIsolatedStream(new SerialSettings(MasterCOM), 400))
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
            using (var master = ModbusMaster.RTU(new SerialSettings(MasterCOM)))
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
            using (var master = ModbusMaster.IsolatedRTU(new SerialSettings(MasterCOM)))
            {
                var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
                Assert.AreEqual("Modbus exception 2", ex.Message);
                Assert.AreEqual(2, ex.Code);
            }
        }
    }
}
