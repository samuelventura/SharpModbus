using System;
using NUnit.Framework;
using SharpModbus;

namespace SharpModbusTest
{
	public class ScannerTest
	{
		[Test]
		public void RtuScanTest()
		{
			Test(new ModbusRTUScanner(), new ModbusRTUProtocol());
		}
		
		[Test]
		public void TcpScanTest()
		{
			Test(new ModbusTCPScanner(), new ModbusTCPProtocol());
		}
		
		void Test(IModbusScanner scanner, IModbusProtocol protocol)
		{
			Test(scanner, protocol, new ModbusF01ReadCoils(0x01, 0x0203, 0x0405));
			Test(scanner, protocol, new ModbusF02ReadInputs(0x01, 0x0203, 0x0405));
			Test(scanner, protocol, new ModbusF03ReadHoldingRegisters(0x01, 0x0203, 0x0405));
			Test(scanner, protocol, new ModbusF04ReadInputRegisters(0x01, 0x0203, 0x0405));
			Test(scanner, protocol, new ModbusF05WriteCoil(0x01, 0x0203, true));
			Test(scanner, protocol, new ModbusF05WriteCoil(0x01, 0x0203, false));
			Test(scanner, protocol, new ModbusF06WriteRegister(0x01, 0x0203, 0x0405));
			Test(scanner, protocol, new ModbusF15WriteCoils(0x01, 0x0203, bo()));
			Test(scanner, protocol, new ModbusF15WriteCoils(0x01, 0x0203, bo(true)));
			Test(scanner, protocol, new ModbusF15WriteCoils(0x01, 0x0203, bo(false)));
			Test(scanner, protocol, new ModbusF15WriteCoils(0x01, 0x0203, bo(true, false, true, false, true, false, true)));
			Test(scanner, protocol, new ModbusF15WriteCoils(0x01, 0x0203, bo(true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false, true, false)));
			Test(scanner, protocol, new ModbusF16WriteRegisters(0x01, 0x0203, us()));
			Test(scanner, protocol, new ModbusF16WriteRegisters(0x01, 0x0203, us(0x0405, 0x0607, 0x0809, 0x0A0B)));
			Test(scanner, protocol, new ModbusF16WriteRegisters(0x01, 0x0203, us(0x0405, 0x0607, 0x0809, 0x0A0B, 0x0405, 0x0607, 0x0809, 0x0A0B, 0x0405, 0x0607, 0x0809, 0x0A0B)));
		}
		
		void Test(IModbusScanner scanner, IModbusProtocol protocol, IModbusCommand cmd)
		{
			const int offset = 2;
			var wrap = protocol.Wrap(cmd);
			var request = new byte[offset + wrap.RequestLength];
			wrap.FillRequest(request, offset);
			scanner.Append(request, offset, wrap.RequestLength);
			Assert.AreEqual(wrap.ToString(), scanner.Scan().ToString());
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
