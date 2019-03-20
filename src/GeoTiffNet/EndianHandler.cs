using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Buffers.Binary;

namespace GeoTiffNet
{
  public class EndianHandler : IEndianHandler
  {
    private readonly bool IsBigEndian;

    public EndianHandler(bool isBigEndian)
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

    public ReadOnlySpan<byte> ToByteArray(uint value)
    {
      var bytes = new byte[4];

      if (this.IsBigEndian)
      {
        BinaryPrimitives.WriteUInt32BigEndian(bytes, value);
      }
      else
      {
        BinaryPrimitives.WriteUInt32LittleEndian(bytes, value);
      }

      return bytes;
    }
  }
}
