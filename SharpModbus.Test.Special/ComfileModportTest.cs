using System;
using System.Threading;
using NUnit.Framework;

namespace SharpModbus.Test.Special
{
    public class ComfileModportTest
    {
        [Test]
        public void ModportSweepTest()
        {
            //http://comfiletech.com/etc/field-i-o/modport-i-o-module/
            //m0 - MD-DIDC8 8 digital input
            //m1 - MD-DOSO8 8 digital output
            //all outputs wired to corresponding inputs
            var settings = new SerialSettings()
            {
                PortName = "COM3",
                BaudRate = 57600,
            };
            using (var master = ModbusMaster.RTU(settings))
            {
                testMaster(master);
            }
            Thread.Sleep(200);
            using (var master = ModbusMaster.IsolatedRTU(settings))
            {
                testMaster(master);
            }
        }

        private void testMaster(ModbusMaster master)
        {
            for (var i = 0; i < 4; i++)
            {
                master.WriteCoil(1, us(3000 + i), false);
                Thread.Sleep(50);
                Assert.AreEqual(false, master.ReadCoil(1, us(3000 + i)));
                master.WriteCoil(1, us(3000 + i), true);
                Thread.Sleep(50);
                Assert.AreEqual(true, master.ReadCoil(1, us(3000 + i)));
            }
            var s0 = bs(false, false, false, false);
            var s1 = bs(false, true, true, false);
            var s2 = bs(true, false, false, true);
            master.WriteCoils(1, 3000, s1);
            Thread.Sleep(50);
            Assert.AreEqual(s1, master.ReadCoils(1, 3000, 4));
            Thread.Sleep(50);
            master.WriteCoils(1, 3000, s2);
            Thread.Sleep(50);
            Assert.AreEqual(s2, master.ReadCoils(1, 3000, 4));
            Thread.Sleep(50);
            master.WriteCoils(1, 3000, s0);
            Thread.Sleep(50);
            Assert.AreEqual(s0, master.ReadCoils(1, 3000, 4));
        }

        private ushort us(int v)
        {
            return (ushort)v;
        }

        private bool[] bs(params bool[] args)
        {
            return args;
        }

        private void checkSampleCompiles()
        {
            var settings = new SerialSettings()
            {
                PortName = "COM3",
                BaudRate = 57600,
            };
            //Modbus RTU over serial
            using (var master = ModbusMaster.RTU(settings))
            {
                master.WriteCoil(1, 3000, false);
                master.WriteCoils(1, 3001, false, true);
            }
            //Modbus RTU over isolated serial
            using (var master = ModbusMaster.IsolatedRTU(settings))
            {
                master.WriteCoil(1, 3000, false);
                master.WriteCoils(1, 3001, false, true);
            }
            //Modbus TCP over socket
            using (var master = ModbusMaster.TCP("10.77.0.2", 502))
            {
                master.WriteCoils(1, 4, false, true);
            }
        }
    }
}
