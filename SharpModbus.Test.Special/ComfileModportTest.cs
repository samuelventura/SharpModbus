using System;
using System.IO.Ports;
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
            var serial = new SerialPort()
            {
                PortName = "COM10",
                BaudRate = 57600,
            };
            using (var master = ModbusMaster.RTU(serial))
            {
                master.WriteCoil(1, 3000, false);
                master.WriteCoil(1, 3001, true);
                master.WriteCoil(1, 3002, true);
                master.WriteCoil(1, 3003, false);
                Thread.Sleep(50);
                Assert.AreEqual(false, master.ReadCoil(1, 3000));
                Assert.AreEqual(true, master.ReadCoil(1, 3001));
                Thread.Sleep(50);
                Assert.AreEqual(new bool[] { false, true, true, false }, master.ReadCoils(1, 3000, 4));
            }
        }
    }
}
