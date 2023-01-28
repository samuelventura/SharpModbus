using System;
using NUnit.Framework;
using SharpModbus.Serial;

namespace SharpModbus.Test.Unix
{
    public class SocatSlaveTest
    {
        private SerialSettings ss(string portName)
        {
            return new SerialSettings { PortName = portName };
        }

        [Test]
        public void TcpOverSerialTest()
        {
            var model = new ModbusModel();
            var scanner = new ModbusTCPScanner();
            using(var socat = new Socat())
            using (var server = new SerialModel(Socat.Slave, model, scanner))
            using (var stream = new ModbusSerialStream(ss(Socat.Master), 400))
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
            using(var socat = new Socat())
            using (var server = new SerialModel(Socat.Slave, model, scanner))
            using (var master = ModbusMaster.RTU(ss(Socat.Master)))
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
            using(var socat = new Socat())
            using (var server = new SerialModel(Socat.Slave, model, scanner))
            using (var stream = new ModbusSerialStream(ss(Socat.Master), 400))
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
            using(var socat = new Socat())
            using (var server = new SerialModel(Socat.Slave, model, scanner))
            using (var master = ModbusMaster.RTU(ss(Socat.Master)))
            {
                var ex = Assert.Throws<ModbusException>(() => H.SharedExceptionTest(master));
                Assert.AreEqual("Modbus exception 2", ex.Message);
                Assert.AreEqual(2, ex.Code);
            }
        }
    }
}
