using System;

namespace SharpModbus.Serial
{
    public interface ISerialStream : IDisposable
    {
        void Write(byte[] data);
        byte[] Read(int size, int eop, int toms);
    }
}
