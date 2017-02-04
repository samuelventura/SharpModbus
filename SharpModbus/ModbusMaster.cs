using System;

namespace SharpModbus
{
	public class ModbusMaster: IDisposable
	{
		private readonly IModbusProtocol protocol;
		private readonly IModbusStream stream;

		public ModbusMaster(IModbusStream stream, IModbusProtocol protocol)
		{
			this.stream = stream;
			this.protocol = protocol;
		}
		
		public void Dispose()
		{
			Disposer.Dispose(stream);
		}
		
		public bool ReadCoil(byte slave, ushort address)
		{
			return ReadCoils(slave, address, 1)[0];
		}
		
		public bool ReadInput(byte slave, ushort address)
		{
			return ReadInputs(slave, address, 1)[0];
		}
		
		public ushort ReadInputRegister(byte slave, ushort address)
		{
			return ReadInputRegisters(slave, address, 1)[0];
		}
		
		public ushort ReadHoldingRegister(byte slave, ushort address)
		{
			return ReadHoldingRegisters(slave, address, 1)[0];
		}
		
		public bool[] ReadCoils(byte slave, ushort address, ushort count)
		{
			return (bool[])Execute(new ModbusF01ReadCoils(slave, address, count));
		}
		
		public bool[] ReadInputs(byte slave, ushort address, ushort count)
		{
			return (bool[])Execute(new ModbusF02ReadInputs(slave, address, count));
		}
		
		public ushort[] ReadInputRegisters(byte slave, ushort address, ushort count)
		{
			return (ushort[])Execute(new ModbusF04ReadInputRegisters(slave, address, count));
		}
		
		public ushort[] ReadHoldingRegisters(byte slave, ushort address, ushort count)
		{
			return (ushort[])Execute(new ModbusF03ReadHoldingRegisters(slave, address, count));
		}
		
		public void WriteCoil(byte slave, ushort address, bool value)
		{
			Execute(new ModbusF05WriteCoil(slave, address, value));
		}
		
		public void WriteRegister(byte slave, ushort address, ushort value)
		{
			Execute(new ModbusF06WriteRegister(slave, address, value));
		}
		
		public void WriteCoils(byte slave, ushort address, bool[] values)
		{
			Execute(new ModbusF15WriteCoils(slave, address, values));
		}
		
		public void WriteRegisters(byte slave, ushort address, ushort[] values)
		{
			Execute(new ModbusF16WriteRegisters(slave, address, values));
		}
		
		private object Execute(ModbusCommand cmd)
		{
			var wrapper = protocol.Wrap(cmd);
			var request = new byte[wrapper.RequestLength];
			var response = new byte[wrapper.ResponseLength];
			wrapper.FillRequest(request, 0);
			stream.Write(request);
			stream.Read(response);
			return wrapper.ParseResponse(response, 0);
		}
	}
}