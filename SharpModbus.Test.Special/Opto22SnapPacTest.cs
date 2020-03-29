using System;
using System.Threading;
using NUnit.Framework;

namespace SharpModbus.Test
{
    public class Opto22SnapPacTest
    {
        [Test]
        public void RackSweepTest()
        {
            //http://www.opto22.com/documents/1678_Modbus_TCP_Protocol_Guide.pdf
            //required otg included in SharpOpto22
            //m0.0 = 24V
            //m0.1 = 0V
            //m0.2 = m1.2
            //m0.3 = m1.3
            //m2.1 = m3.1
            //m2.2 = m3.2
            using (var master = ModbusMaster.TCP("10.77.0.2", 502))
            {
                master.WriteCoils(1, 4, Bools(4, false)); //clear module
                Thread.Sleep(50);
                master.WriteCoils(1, 64, Bools(4, true)); //clear on latches
                master.WriteCoils(1, 128, Bools(4, true)); //clear off latches

                Assert.AreEqual(true, master.ReadCoil(1, 0));
                Assert.AreEqual(false, master.ReadCoil(1, 1));
                Assert.AreEqual(false, master.ReadCoil(1, 2));
                Assert.AreEqual(false, master.ReadCoil(1, 3));

                master.WriteCoil(1, 6, false);
                master.WriteCoil(1, 7, false);
                Thread.Sleep(50);
                Assert.AreEqual(new bool[] { false, false }, master.ReadInputs(1, 64 + 2, 2));
                Assert.AreEqual(new bool[] { false, false }, master.ReadInputs(1, 128 + 2, 2));
                Assert.AreEqual(new bool[] { false, false }, master.ReadCoils(1, 6, 2));
                Assert.AreEqual(new bool[] { true, false, false, false }, master.ReadCoils(1, 0, 4));

                master.WriteCoil(1, 6, true);
                master.WriteCoil(1, 7, false);
                Thread.Sleep(50);
                Assert.AreEqual(true, master.ReadCoil(1, 0));
                Assert.AreEqual(false, master.ReadCoil(1, 1));
                Assert.AreEqual(true, master.ReadCoil(1, 2));
                Assert.AreEqual(false, master.ReadCoil(1, 3));
                Assert.AreEqual(new bool[] { true, false }, master.ReadInputs(1, 64 + 2, 2));
                Assert.AreEqual(new bool[] { false, false }, master.ReadInputs(1, 128 + 2, 2));
                Assert.AreEqual(new bool[] { true, false }, master.ReadCoils(1, 6, 2));
                Assert.AreEqual(new bool[] { true, false, true, false }, master.ReadCoils(1, 0, 4));

                master.WriteCoil(1, 6, false);
                master.WriteCoil(1, 7, true);
                Thread.Sleep(50);
                Assert.AreEqual(true, master.ReadCoil(1, 0));
                Assert.AreEqual(false, master.ReadCoil(1, 1));
                Assert.AreEqual(false, master.ReadCoil(1, 2));
                Assert.AreEqual(true, master.ReadCoil(1, 3));
                Assert.AreEqual(new bool[] { true, true }, master.ReadInputs(1, 64 + 2, 2));
                Assert.AreEqual(new bool[] { true, false }, master.ReadInputs(1, 128 + 2, 2));
                Assert.AreEqual(new bool[] { false, true }, master.ReadCoils(1, 6, 2));
                Assert.AreEqual(new bool[] { true, false, false, true }, master.ReadCoils(1, 0, 4));

                master.WriteCoils(1, 64, Bools(4, true)); //clear on latches
                master.WriteCoils(1, 128, Bools(4, true)); //clear off latches
                Assert.AreEqual(new bool[] { false, false }, master.ReadInputs(1, 64 + 2, 2));
                Assert.AreEqual(new bool[] { false, false }, master.ReadInputs(1, 128 + 2, 2));


                //analog
                SetAnalog(master, 12, 0);
                SetAnalog(master, 13, 0);
                Assert.AreEqual(0, GetAnalog(master, 12));
                Assert.AreEqual(0, GetAnalog(master, 13));
                Thread.Sleep(50);
                Assert.That(0f, Is.EqualTo(GetAnalog2(master, 8)).Within(0.1));
                Assert.That(0f, Is.EqualTo(GetAnalog2(master, 9)).Within(0.1));

                SetAnalog(master, 12, 5);
                SetAnalog(master, 13, 10);
                Assert.AreEqual(5, GetAnalog(master, 12));
                Assert.AreEqual(10, GetAnalog(master, 13));
                Thread.Sleep(50);
                Assert.That(5f, Is.EqualTo(GetAnalog2(master, 8)).Within(0.1));
                Assert.That(10f, Is.EqualTo(GetAnalog2(master, 9)).Within(0.1));

                SetAnalog2(master, 12, -5);
                SetAnalog2(master, 13, -10);
                Assert.AreEqual(-5, GetAnalog(master, 12));
                Assert.AreEqual(-10, GetAnalog(master, 13));
                Thread.Sleep(50);
                Assert.That(-5f, Is.EqualTo(GetAnalog2(master, 8)).Within(0.1));
                Assert.That(-10f, Is.EqualTo(GetAnalog2(master, 9)).Within(0.1));
            }
        }

        private bool[] Bools(int count, bool value)
        {
            var bools = new bool[count];
            for (var i = 0; i < count; i++) bools[i] = value;
            return bools;
        }

        //Opto22 32-bit IEEE float. Data is in Big Endian format
        private void SetAnalog(ModbusMaster master, int point, float value)
        {
            var bytes = FloatToByteArray(value);
            master.WriteRegister(1, (ushort)(2 * point + 0), (ushort)(bytes[0] << 8 | bytes[1]));
            master.WriteRegister(1, (ushort)(2 * point + 1), (ushort)(bytes[2] << 8 | bytes[3]));
        }

        private void SetAnalog2(ModbusMaster master, int point, float value)
        {
            var bytes = FloatToByteArray(value);
            master.WriteRegisters(1, (ushort)(2 * point), new ushort[] {
                (ushort)(bytes[0] << 8 | bytes[1]),
                (ushort)(bytes[2] << 8 | bytes[3])
            });
        }

        private float GetAnalog(ModbusMaster master, int point)
        {
            var words = master.ReadInputRegisters(1, (ushort)(2 * point), 2);
            var bytes = new byte[4];
            bytes[0] = (byte)((words[0] >> 8) & 0xff);
            bytes[1] = (byte)((words[0] >> 0) & 0xff);
            bytes[2] = (byte)((words[1] >> 8) & 0xff);
            bytes[3] = (byte)((words[1] >> 0) & 0xff);
            return ByteArrayToFloat(bytes);
        }

        private float GetAnalog2(ModbusMaster master, int point)
        {
            var words = master.ReadHoldingRegisters(1, (ushort)(2 * point), 2);
            var bytes = new byte[4];
            bytes[0] = (byte)((words[0] >> 8) & 0xff);
            bytes[1] = (byte)((words[0] >> 0) & 0xff);
            bytes[2] = (byte)((words[1] >> 8) & 0xff);
            bytes[3] = (byte)((words[1] >> 0) & 0xff);
            return ByteArrayToFloat(bytes);
        }

        private float ByteArrayToFloat(byte[] d)
        {
            if (BitConverter.IsLittleEndian) Array.Reverse(d);
            return BitConverter.ToSingle(d, 0);
        }

        private byte[] FloatToByteArray(float value)
        {
            var ar = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) Array.Reverse(ar);
            return ar;
        }
    }
}
