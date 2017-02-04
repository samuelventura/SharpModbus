
using System;
using NUnit.Framework;
using SharpModbus;

namespace SharpModbusTest
{
	[TestFixture]
	public class ParserTest
	{
		readonly IModbusProtocol rtuProto = new ModbusRTUProtocol();
		readonly IModbusProtocol tcpProto = new ModbusTCPProtocol();
		
		[Test]
		public void ParseTest()
		{
			Test(new ModbusF01ReadCoils(0x01, 0x0203, 0x0405));
			Test(new ModbusF02ReadInputs(0x01, 0x0203, 0x0405));
			Test(new ModbusF03ReadHoldingRegisters(0x01, 0x0203, 0x0405));
			Test(new ModbusF04ReadInputRegisters(0x01, 0x0203, 0x0405));
			Test(new ModbusF05WriteCoil(0x01, 0x0203, true));
			Test(new ModbusF05WriteCoil(0x01, 0x0203, false));
			Test(new ModbusF06WriteRegister(0x01, 0x0203, 0x0405));
			Test(new ModbusF15WriteCoils(0x01, 0x0203, bo()));
			Test(new ModbusF15WriteCoils(0x01, 0x0203, bo(true)));
			Test(new ModbusF15WriteCoils(0x01, 0x0203, bo(false)));
			Test(new ModbusF15WriteCoils(0x01, 0x0203, bo(true, false, true, false, true, false, true)));
			Test(new ModbusF15WriteCoils(0x01, 0x0203, bo(true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false)));
			Test(new ModbusF16WriteRegisters(0x01, 0x0203, us()));
			Test(new ModbusF16WriteRegisters(0x01, 0x0203, us(0x0405, 0x0607, 0x0809, 0x0A0B)));
			Test(new ModbusF16WriteRegisters(0x01, 0x0203, us(0x0405, 0x0607, 0x0809, 0x0A0B, 0x0405, 0x0607, 0x0809, 0x0A0B, 0x0405, 0x0607, 0x0809, 0x0A0B)));
		}
		
		void Test(IModbusCommand cmd)
		{
			const int offset = 2;
			var request = new byte[offset + cmd.RequestLength];
			cmd.FillRequest(request, offset);
			var pcmd = ModbusParser.Parse(request, offset);
			Assert.AreEqual(pcmd.ToString(), cmd.ToString());
			
			var rtuWrap = rtuProto.Wrap(cmd);
			var rtuRequest = new byte[offset + rtuWrap.RequestLength];
			rtuWrap.FillRequest(rtuRequest, offset);
			var prtuWrap = rtuProto.Parse(rtuRequest, offset);
			Assert.AreEqual(prtuWrap.ToString(), rtuWrap.ToString());
			
			var tcpWrap = tcpProto.Wrap(cmd);
			var tcpRequest = new byte[offset + tcpWrap.RequestLength];
			tcpWrap.FillRequest(tcpRequest, offset);
			var ptcpWrap = tcpProto.Parse(tcpRequest, offset);
			Assert.AreEqual(ptcpWrap.ToString(), tcpWrap.ToString());
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
