
using System;
using NUnit.Framework;
using SharpModbus;

namespace SharpModbusTest
{
	[TestFixture]
	public class ModelTest
	{
		[Test]
		public void ApplyTest()
		{
			var model = new ModbusModel();
			
			model.setDO(0x01, 0x0506, true);
			Assert.AreEqual(bo(true), Apply(model, new ModbusF01ReadCoils(0x01, 0x0506, 1)));
			model.setDO(0x01, 0x0507, false);
			Assert.AreEqual(bo(false), Apply(model, new ModbusF01ReadCoils(0x01, 0x0507, 1)));
			Assert.AreEqual(bo(true, false), Apply(model, new ModbusF01ReadCoils(0x01, 0x0506, 2)));
			
			model.setDI(0x01, 0x0506, true);
			Assert.AreEqual(bo(true), Apply(model, new ModbusF02ReadInputs(0x01, 0x0506, 1)));
			model.setDI(0x01, 0x0507, false);
			Assert.AreEqual(bo(false), Apply(model, new ModbusF02ReadInputs(0x01, 0x0507, 1)));
			Assert.AreEqual(bo(true, false), Apply(model, new ModbusF02ReadInputs(0x01, 0x0506, 2)));
			
			model.setWO(0x01, 0x0506, 0x0a0b);
			Assert.AreEqual(us(0x0a0b), Apply(model, new ModbusF03ReadHoldingRegisters(0x01, 0x0506, 1)));
			model.setWO(0x01, 0x0507, 0x0a0c);
			Assert.AreEqual(us(0x0a0c), Apply(model, new ModbusF03ReadHoldingRegisters(0x01, 0x0507, 1)));
			Assert.AreEqual(us(0x0a0b, 0x0a0c), Apply(model, new ModbusF03ReadHoldingRegisters(0x01, 0x0506, 2)));
			
			model.setWI(0x01, 0x0506, 0x0a0b);
			Assert.AreEqual(us(0x0a0b), Apply(model, new ModbusF04ReadInputRegisters(0x01, 0x0506, 1)));
			model.setWI(0x01, 0x0507, 0x0a0c);
			Assert.AreEqual(us(0x0a0c), Apply(model, new ModbusF04ReadInputRegisters(0x01, 0x0507, 1)));
			Assert.AreEqual(us(0x0a0b, 0x0a0c), Apply(model, new ModbusF04ReadInputRegisters(0x01, 0x0506, 2)));
			
			Apply(model, new ModbusF05WriteCoil(0x01, 0x0203, true));
			Assert.AreEqual(true, model.getDO(0x01, 0x0203));
			Apply(model, new ModbusF05WriteCoil(0x01, 0x0204, false));
			Assert.AreEqual(false, model.getDO(0x01, 0x0204));
			Assert.AreEqual(bo(true, false), model.getDOs(0x01, 0x0203, 2));
			
			Apply(model, new ModbusF06WriteRegister(0x01, 0x0203, 0x0a0b));
			Assert.AreEqual(0x0a0b, model.getWO(0x01, 0x0203));
			Apply(model, new ModbusF06WriteRegister(0x01, 0x0204, 0x0a0c));
			Assert.AreEqual(0x0a0c, model.getWO(0x01, 0x0204));
			Assert.AreEqual(us(0x0a0b, 0x0a0c), model.getWOs(0x01, 0x0203, 2));

			Apply(model, new ModbusF15WriteCoils(0x01, 0x0708, bo(false, true)));
			Assert.AreEqual(bo(false, true), model.getDOs(0x01, 0x0708, 2));

			Apply(model, new ModbusF16WriteRegisters(0x01, 0x0708, us(0x0a0b, 0x0a0c)));
			Assert.AreEqual(us(0x0a0b, 0x0a0c), model.getWOs(0x01, 0x0708, 2));
		}
		
		object Apply(ModbusModel model, IModbusCommand cmd)
		{
			return cmd.ApplyTo(model);
		}
		
		ushort[] us(params ushort[] args)
		{
			return args;
		}
		
		bool[] bo(params bool[] args)
		{
			return args;
		}
	}
}
