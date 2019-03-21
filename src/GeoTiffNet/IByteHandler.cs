using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Buffers.Binary;

namespace GeoTiffNet
{
  public interface IByteHandler
  {
    short ReadInt16(ReadOnlySpan<byte> bytes, int offset);

    short ReadInt16(ReadOnlySpan<byte> bytes);

    ushort ReadUInt16(ReadOnlySpan<byte> bytes, int offset);

    ushort ReadUInt16(ReadOnlySpan<byte> bytes);

    int ReadInt32(ReadOnlySpan<byte> bytes, int offset);

    int ReadInt32(ReadOnlySpan<byte> bytes);

    uint ReadUInt32(ReadOnlySpan<byte> bytes, int offset);

    uint ReadUInt32(ReadOnlySpan<byte> bytes);

    float ReadSingle(ReadOnlySpan<byte> bytes, int offset);

    float ReadSingle(ReadOnlySpan<byte> bytes);

    double ReadDouble(ReadOnlySpan<byte> bytes, int offset);

    double ReadDouble(ReadOnlySpan<byte> bytes);
  }
}
