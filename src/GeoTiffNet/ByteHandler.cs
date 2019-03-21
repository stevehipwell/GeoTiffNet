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

    public short ReadInt16(ReadOnlySpan<byte> bytes, int offset)
    {
      return this.ReadInt16(bytes.Slice(offset, 2));
    }

    public short ReadInt16(ReadOnlySpan<byte> bytes)
    {
      return this.IsBigEndian ? BinaryPrimitives.ReadInt16BigEndian(bytes) : BinaryPrimitives.ReadInt16LittleEndian(bytes);
    }

    public ushort ReadUInt16(ReadOnlySpan<byte> bytes, int offset)
    {
      return this.ReadUInt16(bytes.Slice(offset, 2));
    }

    public ushort ReadUInt16(ReadOnlySpan<byte> bytes)
    {
      return this.IsBigEndian ? BinaryPrimitives.ReadUInt16BigEndian(bytes) : BinaryPrimitives.ReadUInt16LittleEndian(bytes);
    }

    public int ReadInt32(ReadOnlySpan<byte> bytes, int offset)
    {
      return this.ReadInt32(bytes.Slice(offset, 4));
    }

    public int ReadInt32(ReadOnlySpan<byte> bytes)
    {
      return this.IsBigEndian ? BinaryPrimitives.ReadInt32BigEndian(bytes) : BinaryPrimitives.ReadInt32LittleEndian(bytes);
    }

    public uint ReadUInt32(ReadOnlySpan<byte> bytes, int offset)
    {
      return this.ReadUInt32(bytes.Slice(offset, 4));
    }

    public uint ReadUInt32(ReadOnlySpan<byte> bytes)
    {
      return this.IsBigEndian ? BinaryPrimitives.ReadUInt32BigEndian(bytes) : BinaryPrimitives.ReadUInt32LittleEndian(bytes);
    }

    public float ReadSingle(ReadOnlySpan<byte> bytes, int offset)
    {
      return this.ReadSingle(bytes.Slice(offset, 8));
    }

    public float ReadSingle(ReadOnlySpan<byte> bytes)
    {
      return BitConverter.ToSingle(BitConverter.GetBytes(this.IsBigEndian ? BinaryPrimitives.ReadInt32BigEndian(bytes) : BinaryPrimitives.ReadInt32LittleEndian(bytes)), 0);
    }

    public double ReadDouble(ReadOnlySpan<byte> bytes, int offset)
    {
      return this.ReadDouble(bytes.Slice(offset, 8));
    }

    public double ReadDouble(ReadOnlySpan<byte> bytes)
    {
      return BitConverter.Int64BitsToDouble(this.IsBigEndian ? BinaryPrimitives.ReadInt64BigEndian(bytes) : BinaryPrimitives.ReadInt64LittleEndian(bytes));
    }
  }
}
