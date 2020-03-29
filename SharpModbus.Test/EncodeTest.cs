using System;
using NUnit.Framework;

namespace SharpModbus.Test
{
    public class EncodeTest
    {
        [Test]
        public void CrcTest()
        {
            Assert.AreEqual(
                ModbusHelper.CRC16(new byte[] { 0, 0, 1, 1, 2, 3, 4, 5, 0, 0 }, 2, 6),
                ModbusHelper.CRC16(new byte[] { 1, 1, 2, 3, 4, 5, 0, 0 }, 0, 6)
            );
        }

        [Test]
        public void DecodeBoolsTest()
        {
            Assert.AreEqual(bo(), ModbusHelper.DecodeBools(by(0x00), 1, 0));
            Assert.AreEqual(bo(true, false, true), ModbusHelper.DecodeBools(by(0x00, 0x05), 1, 3));
            Assert.AreEqual(bo(true, false, true, false, false, false, false, false, false, true),
                ModbusHelper.DecodeBools(by(0x00, 0x05, 0x02), 1, 10));
        }

        [Test]
        public void EncodeBoolsTest()
        {
            Assert.AreEqual(by(), ModbusHelper.EncodeBools(bo()));
            Assert.AreEqual(by(0x01), ModbusHelper.EncodeBools(bo(true)));
            Assert.AreEqual(by(0x03), ModbusHelper.EncodeBools(bo(true, true)));
            Assert.AreEqual(by(0x13), ModbusHelper.EncodeBools(bo(true, true, false, false, true)));
            Assert.AreEqual(by(0x13, 0x05), ModbusHelper.EncodeBools(bo(true, true, false, false, true, false, false, false, true, false, true)));
        }

        ushort[] us(params ushort[] args)
        {
            return args;
        }

        bool[] bo(params bool[] args)
        {
            return args;
        }

        byte[] by(params byte[] args)
        {
            return args;
        }
    }
}
