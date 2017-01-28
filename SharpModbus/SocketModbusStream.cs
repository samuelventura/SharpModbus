using System;
using System.Threading;
using System.Net.Sockets;

namespace SharpModbus
{
	public class SocketModbusStream : IModbusStream, IDisposable
	{
		private readonly TcpClient socket;
		private readonly Action<char, byte[], int> monitor;
	
		public SocketModbusStream(TcpClient socket, int timeout, Action<char, byte[], int> monitor = null)
		{
			socket.ReceiveTimeout = timeout;
			socket.SendTimeout = timeout;
			this.monitor = monitor;
			this.socket = socket;
		}

		public void Dispose()
		{
			Closer.Close(socket);
		}
		
		public void Write(byte[] data)
		{
			if (monitor != null)
				monitor('>', data, data.Length);
			socket.GetStream().Write(data, 0, data.Length);
		}
	
		public void Read(byte[] data)
		{
			var dl = DateTime.Now.AddMilliseconds(socket.ReceiveTimeout);
			var count = 0;
			while (DateTime.Now < dl && count < data.Length) {
				var available = socket.Available;
				if (available == 0)
					Thread.Sleep(1);
				count += socket.GetStream().Read(data, count, (int)Math.Min(available, data.Length - count));
			}
			if (monitor != null)
				monitor('<', data, count);
			Assert.Equal(count, data.Length, "Partial read {0} {1}");
		}
	}
}

