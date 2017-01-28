using System;

namespace SharpModbus
{
	public class TcpModbusProtocol : IModbusProtocol
	{
		private readonly IModbusStream stream;
		private ushort transactionId;
		
		public TcpModbusProtocol(IModbusStream stream)
		{
			this.stream = stream;
		}
		
		public void Dispose()
		{
			stream.Dispose();
		}
		
		public byte[] Query(byte[] request, ushort responseLength)
		{
			var requestWrapper = new byte[6 + request.Length];
			var responseWrapper = new byte[6 + responseLength];
			requestWrapper[0] = (byte)((transactionId >> 8) & 0xff);
			requestWrapper[1] = (byte)((transactionId >> 0) & 0xff);
			requestWrapper[2] = 0;
			requestWrapper[3] = 0;
			requestWrapper[4] = (byte)((request.Length >> 8) & 0xff);
			requestWrapper[5] = (byte)((request.Length >> 0) & 0xff);
			for (var i = 0; i < request.Length; i++)
				requestWrapper[6 + i] = request[i];
			transactionId++;
			stream.Write(requestWrapper);
			stream.Read(responseWrapper);
			for (var i = 0; i < 4; i++)
				Assert.Equal(requestWrapper[i], responseWrapper[i], i, "Header mismatch {0} {1} at pos:{2}");
			var declaredLength = (ushort)(responseWrapper[4] << 8 | responseWrapper[5]);
			Assert.Equal(declaredLength, responseLength, "Length mismatch {0} {1}");
			var response = new byte[responseLength];
			for (var i = 0; i < responseLength; i++)
				response[i] = responseWrapper[i + 6];
			return response;
		}
	}
}
