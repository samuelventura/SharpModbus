using System;

namespace SharpModbus
{
	public class ModbusF15WriteCoils : IModbusCommand
	{
		private readonly byte slave;
		private readonly ushort address;
		private readonly bool[] values;
		
		public byte Slave { get { return slave; } }
		public ushort Address { get { return address; } }
		public bool[] Values { get { return ModbusHelper.Clone(values); } }
		public int RequestLength { get { return 7 + ModbusHelper.BytesForBools(values.Length); } }
		public int ResponseLength { get { return 6; } }
		
		public ModbusF15WriteCoils(byte slave, ushort address, bool[] values)
		{
			this.slave = slave;
			this.address = address;
			this.values = values;
		}
		
		public void FillRequest(byte[] request, int offset)
		{
			FillResponse(request, offset, null);
			var bytes = ModbusHelper.EncodeBools(values);
			request[offset + 6] = (byte)bytes.Length;
			ModbusHelper.Copy(bytes, 0, request, offset + 7, bytes.Length);
		}
		
		public object ParseResponse(byte[] response, int offset)
		{
			Assert.Equal(response[offset + 0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[offset + 1], 15, "Function mismatch {0} expected:{1}");
			Assert.Equal(ModbusHelper.GetUShort(response, offset + 2), address, "Address mismatch {0} expected:{1}");
			Assert.Equal(ModbusHelper.GetUShort(response, offset + 4), values.Length, "Coil count mismatch {0} expected:{1}");
			return null;
		}
		
		public object ApplyTo(IModbusModel model)
		{
			model.setDOs(slave, address, values);
			return null;
		}
		
		public void FillResponse(byte[] response, int offset, object value)
		{
			response[offset + 0] = slave;
			response[offset + 1] = 15;
			response[offset + 2] = ModbusHelper.High(address);
			response[offset + 3] = ModbusHelper.Low(address);
			response[offset + 4] = ModbusHelper.High(values.Length);
			response[offset + 5] = ModbusHelper.Low(values.Length);			
		}
		
		public override string ToString()
		{
			return string.Format("[ModbusF15WriteCoils Slave={0}, Address={1}, Values={2}]", slave, address, values);
		}
	}
}