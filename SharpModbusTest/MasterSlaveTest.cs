
using System;
using NUnit.Framework;
using SharpModbus;

namespace SharpModbusTest
{
	[TestFixture]
	public class SlaveTest
	{
		[Test]
		public void RtuSlaveTest()
		{
			var model = new ModbusModel();
			var stream = new ModelStream(model, new ModbusRTUScanner());
			var master = new ModbusMaster(stream, new ModbusRTUProtocol());
			Test(model, master);
		}
		
		[Test]
		public void TcpSlaveTest()
		{
			var model = new ModbusModel();
			var stream = new ModelStream(model, new ModbusTCPScanner());
			var master = new ModbusMaster(stream, new ModbusTCPProtocol());
			//ensure TransactionId wraps around 0xFFFF
			for(var i=0; i<=0xFFFF; i++) Test(model, master);
		}
		
		void Test(ModbusModel model, ModbusMaster master)
		{
			master.WriteCoil(1, 2, true);
			Assert.AreEqual(true, master.ReadCoil(1, 2));
			master.WriteCoil(1, 3, false);
			Assert.AreEqual(false, master.ReadCoil(1, 3));
			master.WriteCoils(1, 4, bo(false, true));
			Assert.AreEqual(bo(true, false, false, true), master.ReadCoils(1, 2, 4));
			
			model.setDIs(11, 12, bo(true, true, false, false));
			Assert.AreEqual(true, master.ReadInput(11, 12));
			Assert.AreEqual(true, master.ReadInput(11, 13));
			Assert.AreEqual(false, master.ReadInput(11, 14));
			Assert.AreEqual(false, master.ReadInput(11, 15));
			Assert.AreEqual(bo(true, true, false, false), master.ReadInputs(11, 12, 4));
			
			master.WriteRegister(1, 2, 0xabcd);
			Assert.AreEqual(0xabcd, master.ReadHoldingRegister(1, 2));
			master.WriteRegister(1, 3, 0xcdab);
			Assert.AreEqual(0xcdab, master.ReadHoldingRegister(1, 3));
			master.WriteRegisters(1, 4, us(0xcda1, 0xcda2));
			Assert.AreEqual(us(0xabcd, 0xcdab, 0xcda1, 0xcda2), master.ReadHoldingRegisters(1, 2, 4));

			model.setWIs(11, 12, us(0xabcd, 0xcdab, 0xcda1, 0xcda2));
			Assert.AreEqual(0xabcd, master.ReadInputRegister(11, 12));
			Assert.AreEqual(0xcdab, master.ReadInputRegister(11, 13));
			Assert.AreEqual(0xcda1, master.ReadInputRegister(11, 14));
			Assert.AreEqual(0xcda2, master.ReadInputRegister(11, 15));
			Assert.AreEqual(us(0xabcd, 0xcdab, 0xcda1, 0xcda2), master.ReadInputRegisters(11, 12, 4));
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
	
	public class ModelStream : IModbusStream
	{
		private readonly ModbusModel model;
		private readonly IModbusScanner scanner;
		
		public ModelStream(ModbusModel model, IModbusScanner scanner)
		{
			this.model = model;
			this.scanner = scanner;
		}
		
		public void Dispose()
		{
		}
		
		public void Write(byte[] data)
		{
			scanner.Append(data, 0, data.Length);
		}
		
		public void Read(byte[] data)
		{
			var cmd = scanner.Scan();
			var value = cmd.ApplyTo(model);
			cmd.FillResponse(data, 0, value);
		}
	}
}
