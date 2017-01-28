using System;

namespace SharpModbus
{
	public class RtuModbusProtocol : IModbusProtocol
	{
		private readonly IModbusStream stream;
		
		public RtuModbusProtocol(IModbusStream stream)
		{
			this.stream = stream;
		}
		
		public void Dispose()
		{
			stream.Dispose();
		}
		
		public byte[] Query(byte[] request, ushort responseLength)
		{
			var requestWrapper = new byte[2 + request.Length];
			var responseWrapper = new byte[2 + responseLength];
			for (int i = 0; i < request.Length; i++)
				requestWrapper[i] = request[i];
			var crc = crc16(request, 0, request.Length);
			requestWrapper[request.Length] = (byte)((crc >> 8) & 0xff);
			requestWrapper[request.Length + 1] = (byte)((crc >> 0) & 0xff);
			stream.Write(requestWrapper);
			stream.Read(responseWrapper);
			var crc1 = crc16(responseWrapper, 0, responseLength);
			var crc2 = (ushort)(responseWrapper[responseLength] << 8 | responseWrapper[responseLength + 1]);
			Assert.Equal(crc2, crc1, "CRC mismatch {0} {1}");
			var response = new byte[responseLength];
			for (int i = 0; i < responseLength; i++)
				response[i] = responseWrapper[i];
			return response;
		}
		
		private ushort crc16(byte[] buf, int offset, int count)
		{
			ushort crc = 0xFFFF;
			for (int pos = offset; pos < count; pos++) {
				crc ^= (ushort)buf[pos];
				for (int i = 8; i > 0; i--) {
					if ((crc & 0x0001) != 0) {
						crc >>= 1;
						crc ^= 0xA001;
					} else
						crc >>= 1;
				}
			}
			return (ushort)((crc >> 8) & 0x00ff | (crc << 8) & 0xff00);
		}
	}
}

