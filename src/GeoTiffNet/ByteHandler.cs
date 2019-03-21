using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Buffers.Binary;

namespace GeoTiffNet
{
  public class ByteHandler : IByteHandler
  {
    private readonly bool IsBigEndian;

    public ByteHandler(bool isBigEndian)
    {
      this.IsBigEndian = isBigEndian;
    }

    public ushort ReadUInt16(ReadOnlySpan<byte> bytes)
    {
      return this.IsBigEndian ? BinaryPrimitives.ReadUInt16BigEndian(bytes) : BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public uint ReadUInt32(ReadOnlySpan<byte> bytes)
    {
      return this.IsBigEndian ? BinaryPrimitives.ReadUInt32BigEndian(bytes) : BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public double ReadDouble(ReadOnlySpan<byte> bytes)
    {
      return BitConverter.Int64BitsToDouble(this.IsBigEndian ? BinaryPrimitives.ReadInt64BigEndian(bytes) : BinaryPrimitives.ReadInt64LittleEndian(bytes));
    }
  }
}
