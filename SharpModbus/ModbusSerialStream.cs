using System;
using System.Threading;
using System.IO.Ports;

namespace SharpModbus
{
	public class ModbusSerialStream : IModbusStream, IDisposable
	{
		private readonly SerialPort serialPort;
		private readonly Action<char, byte[], int> monitor;
		
		public ModbusSerialStream(SerialPort serialPort, int timeout, Action<char, byte[], int> monitor = null)
		{
			serialPort.ReadTimeout = timeout;
			serialPort.WriteTimeout = timeout;
			this.serialPort = serialPort;
			this.monitor = monitor;
		}

		public void Dispose()
		{
			Disposer.Dispose(serialPort);
		}
		
		public void Write(byte[] data)
		{
			if (monitor != null)
				monitor('>', data, data.Length);
			serialPort.Write(data, 0, data.Length);
		}
	
		public void Read(byte[] data)
		{
			var dl = DateTime.Now.AddMilliseconds(serialPort.ReadTimeout);
			var count = 0;
			while (DateTime.Now < dl && count < data.Length) {
				var available = serialPort.BytesToRead;
				if (available == 0)
					Thread.Sleep(1);
				count += serialPort.Read(data, count, (int)Math.Min(available, data.Length - count));
			}
			if (monitor != null)
				monitor('<', data, count);
			Assert.Equal(count, data.Length, "Partial read {0} {1}");
		}
	}
}
