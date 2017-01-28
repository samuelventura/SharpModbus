using System;

namespace SharpModbus
{
	public class ModbusMaster: IDisposable
	{
		private readonly IModbusProtocol protocol;
		
		public ModbusMaster(IModbusProtocol protocol)
		{
			this.protocol = protocol;
		}
		
		public void Dispose()
		{
			protocol.Dispose();
		}
		
		public bool ReadCoil(byte slave, ushort address)
		{
			return ReadDigital(slave, 1, address, 1)[0];
		}
		
		public bool[] ReadCoils(byte slave, ushort address, ushort count)
		{
			return ReadDigital(slave, 1, address, count);
		}
		
		public bool ReadInput(byte slave, ushort address)
		{
			return ReadDigital(slave, 2, address, 1)[0];
		}
		
		public bool[] ReadInputs(byte slave, ushort address, ushort count)
		{
			return ReadDigital(slave, 2, address, count);
		}
		
		public ushort ReadInputRegister(byte slave, ushort address)
		{
			return ReadRegister(slave, 4, address, 1)[0];
		}
		
		public ushort[] ReadInputRegisters(byte slave, ushort address, ushort count)
		{
			return ReadRegister(slave, 4, address, count);
		}
		
		public ushort ReadHoldingRegister(byte slave, ushort address)
		{
			return ReadRegister(slave, 3, address, 1)[0];
		}
		
		public ushort[] ReadHoldingRegisters(byte slave, ushort address, ushort count)
		{
			return ReadRegister(slave, 3, address, count);
		}
		
		public void WriteCoil(byte slave, ushort address, bool state)
		{
			var request = new byte[6];
			request[0] = slave;
			request[1] = 5;
			request[2] = (byte)((address >> 8) & 0xff);
			request[3] = (byte)((address >> 0) & 0xff);
			request[4] = (byte)(state ? 0xFF : 0x00);
			request[5] = 0;
			var response = protocol.Query(request, 6);
			Assert.Equal(response[0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[1], (byte)5, "Function mismatch {0} expected:{1}");
			Assert.Equal(response[2] << 8 | response[3], address, "Address mismatch {0} expected:{1}");
			Assert.Equal(response[4], state ? 0xFF : 0x00, "Value mismatch {0} expected:{1}");
			Assert.Equal(response[5], (byte)0, "Pad mismatch {0} expected:{1}");
		}
		
		public void WriteRegister(byte slave, ushort address, ushort value)
		{
			var request = new byte[6];
			request[0] = slave;
			request[1] = 6;
			request[2] = (byte)((address >> 8) & 0xff);
			request[3] = (byte)((address >> 0) & 0xff);
			request[4] = (byte)((value >> 8) & 0xff);
			request[5] = (byte)((value >> 0) & 0xff);
			var response = protocol.Query(request, 6);
			Assert.Equal(response[0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[1], (byte)6, "Function mismatch {0} expected:{1}");
			Assert.Equal(response[2] << 8 | response[3], address, "Address mismatch {0} expected:{1}");
			Assert.Equal(response[4] << 8 | response[5], value, "Value mismatch {0} expected:{1}");
		}
		
		public void WriteCoils(byte slave, ushort address, bool[] values)
		{
			var bytes = EncodeBools(values);
			var request = new byte[7 + bytes.Length];
			request[0] = slave;
			request[1] = 15;
			request[2] = (byte)((address >> 8) & 0xff);
			request[3] = (byte)((address >> 0) & 0xff);
			request[4] = (byte)((values.Length >> 8) & 0xff);
			request[5] = (byte)((values.Length >> 0) & 0xff);
			request[6] = (byte)(bytes.Length);
			for (var i = 0; i < bytes.Length; i++)
				request[7 + i] = bytes[i];
			var response = protocol.Query(request, 6);
			Assert.Equal(response[0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[1], (byte)15, "Function mismatch {0} expected:{1}");
			Assert.Equal(response[2] << 8 | response[3], address, "Address mismatch {0} expected:{1}");
			Assert.Equal(response[4] << 8 | response[5], (ushort)values.Length, "Coil count mismatch {0} expected:{1}");
		}
		
		public void WriteRegisters(byte slave, ushort address, ushort[] values)
		{
			var bytes = EncodeWords(values);
			var request = new byte[7 + 2 * values.Length];
			request[0] = slave;
			request[1] = 16;
			request[2] = (byte)((address >> 8) & 0xff);
			request[3] = (byte)((address >> 0) & 0xff);
			request[4] = (byte)((values.Length >> 8) & 0xff);
			request[5] = (byte)((values.Length >> 0) & 0xff);
			request[6] = (byte)(bytes.Length);
			for (var i = 0; i < bytes.Length; i++)
				request[7 + i] = bytes[i];
			var response = protocol.Query(request, 6);
			Assert.Equal(response[0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[1], (byte)16, "Function mismatch {0} expected:{1}");
			Assert.Equal(response[2] << 8 | response[3], address, "Address mismatch {0} expected:{1}");
			Assert.Equal(response[4] << 8 | response[5], (ushort)values.Length, "Register count mismatch {0} expected:{1}");
		}
		
		private bool[] ReadDigital(byte slave, byte function, ushort address, ushort count)
		{
			var request = new byte[6];
			request[0] = slave;
			request[1] = function;
			request[2] = (byte)((address >> 8) & 0xff);
			request[3] = (byte)((address >> 0) & 0xff);
			request[4] = (byte)((count >> 8) & 0xff);
			request[5] = (byte)((count >> 0) & 0xff);	
			var bytes = BytesForBools(count);
			var response = protocol.Query(request, (ushort)(3 + bytes));
			Assert.Equal(response[0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[1], function, "Function mismatch {0} expected:{1}");
			Assert.Equal(response[2], bytes, "Bytes mismatch {0} expected:{1}");
			return DecodeBools(response, 3, count);
		}
		
		private ushort[] ReadRegister(byte slave, byte function, ushort address, ushort count)
		{
			var request = new byte[6];
			request[0] = slave;
			request[1] = function;
			request[2] = (byte)((address >> 8) & 0xff);
			request[3] = (byte)((address >> 0) & 0xff);
			request[4] = (byte)((count >> 8) & 0xff);
			request[5] = (byte)((count >> 0) & 0xff);	
			var bytes = 2 * count;
			var response = protocol.Query(request, (byte)(3 + bytes));
			Assert.Equal(response[0], slave, "Slave mismatch {0} expected:{1}");
			Assert.Equal(response[1], function, "Function mismatch {0} expected:{1}");
			Assert.Equal(response[2], bytes, "Bytes mismatch {0} expected:{1}");
			return DecodeWords(response, 3, count);
		}
		
		public static byte[] EncodeBools(bool[] bools)
		{
			var count = BytesForBools(bools.Length);
			var bytes = new byte[count];
			for (var i = 0; i < count; i++) {
				bytes[i] = 0;
			}
			for (var i = 0; i < bools.Length; i++) {
				var v = bools[i];
				if (v) {
					var bi = i / 8;
					bytes[bi] |= (byte)(1 << (i % 8));
				}
			}
			return bytes;
		}
		
		public static bool[] DecodeBools(byte[] packet, int offset, ushort count)
		{
			var bools = new bool[count];
			var bytes = BytesForBools(count);
			for (var i = 0; i < bytes; i++) {
				var bits = count >= 8 ? 8 : count % 8;
				var b = packet[offset + i];
				ByteToBools(b, bools, bools.Length - count, bits);
				count -= (ushort)bits;
			}
			return bools;
		}
		
		public static byte[] EncodeWords(ushort[] words)
		{
			var count = 2 * words.Length;
			var bytes = new byte[count];
			for (var i = 0; i < count; i++) {
				bytes[i] = 0;
			}
			for (var i = 0; i < words.Length; i++) {
				bytes[2 * i + 0] = (byte)((words[i] >> 8) & 0xff);
				bytes[2 * i + 1] = (byte)((words[i] >> 0) & 0xff);
			}
			return bytes;
		}
		
		public static ushort[] DecodeWords(byte[] packet, int offset, ushort count)
		{
			var results = new ushort[count];
			for (int i = 0; i < count; i++) {
				results[i] = (ushort)(packet[offset + 2 * i] << 8 | packet[offset + 2 * i + 1]);
			}
			return results;
		}
		
		private static byte BytesForBools(int count)
		{
			return (byte)(count == 0 ? 0 : (count - 1) / 8 + 1);
		}
		
		private static void ByteToBools(byte b, bool[] bools, int offset, int count)
		{
			for (int i = 0; i < count; i++)
				bools[offset + i] = ((b >> i) & 0x01) == 1;
		}
		
		private static byte ByteToBools(bool[] bools, int offset, int count)
		{
			var b = (byte)0;
			for (int i = 0; i < count; i++)
				if (bools[offset + i])
					b |= (byte)(1 >> i);
			return b;
		}
	}
}