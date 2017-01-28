using System;
using NUnit.Framework;
using SharpModbus;

namespace SharpModbusTest
{
	[TestFixture]
	public class EncodeTest
	{
		[Test]
		public void DecodeBoolsTest()
		{
			Assert.AreEqual(new bool[]{ }, ModbusMaster.DecodeBools(new byte[]{ 0x00 }, 1, 0));
			Assert.AreEqual(new bool[]{ true, false, true }, ModbusMaster.DecodeBools(new byte[] {
				0x00,
				0x05
			}, 1, 3));
			Assert.AreEqual(new bool[] {
				true,
				false,
				true,
				false,
				false,
				false,
				false,
				false,
				false,
				true
			}, ModbusMaster.DecodeBools(new byte[] {
				0x00,
				0x05,
				0x02
			}, 1, 10));
		}
		
		[Test]
		public void EncodeBoolsTest()
		{
			Assert.AreEqual(new byte[]{ }, ModbusMaster.EncodeBools(new bool[]{ }));
			Assert.AreEqual(new byte[]{ 0x01 }, ModbusMaster.EncodeBools(new bool[]{ true }));
			Assert.AreEqual(new byte[]{ 0x03 }, ModbusMaster.EncodeBools(new bool[] {
				true,
				true
			}));
			Assert.AreEqual(new byte[]{ 0x13 }, ModbusMaster.EncodeBools(new bool[] {
				true,
				true,
				false,
				false,
				true
			}));
			Assert.AreEqual(new byte[]{ 0x13, 0x05 }, ModbusMaster.EncodeBools(new bool[] {
				true,
				true,
				false,
				false,
				true,
				false,
				false,
				false,
				true,
				false,
				true
			}));
		}
	}
}
