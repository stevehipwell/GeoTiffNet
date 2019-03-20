using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoTiffNet
{
  public class TiffField : ITiffField
  {
    private readonly IEndianHandler ByteHandler;
    private readonly Stream Stream;

    public TiffField(IEndianHandler byteHandler, Stream stream, ReadOnlySpan<byte> bytes)
    {
      this.ByteHandler = byteHandler;
      this.Stream = stream;

      this.Load(bytes);
    }

    public TiffTagEnum Tag { get; private set; }

    public TiffTagTypeEnum Type { get; private set; }

    public int Count { get; private set; }

    public uint ValueOffset { get; private set; }

    public ushort GetUInt16Value()
    {
      if (this.Type != TiffTagTypeEnum.Short)
      {
        throw new Exception("This tag does not contain ushort data.");
      }

      if (!this.IsOffset())
      {
        return BitConverter.ToUInt16(BitConverter.GetBytes(this.ValueOffset), 0);
      }

      this.Stream.Position = this.ValueOffset;
      using (var reader = new BinaryReader(this.Stream, Encoding.UTF8, true))
      {
        return this.ByteHandler.ReadUInt16(reader.ReadBytes(this.GetByteCount()));
      }
    }

    public ushort[] GetUInt16Values()
    {
      if (this.Type != TiffTagTypeEnum.Short)
      {
        throw new Exception("This tag does not contain ushort data.");
      }

      var byteCount = this.GetByteCount();

      if (!this.IsOffset())
      {
        var values = new ushort[this.Count];
        var bytes = BitConverter.GetBytes(this.ValueOffset);

        for (int i = 0; i < this.Count; i++)
        {
          values[i] = BitConverter.ToUInt16(bytes, i * byteCount);
        }

        return values;
      }

      this.Stream.Position = this.ValueOffset;
      using (var reader = new BinaryReader(this.Stream, Encoding.UTF8, true))
      {
        var values = new ushort[this.Count];

        for (int i = 0; i < this.Count; i++)
        {
          values[i] = this.ByteHandler.ReadUInt16(reader.ReadBytes(byteCount));
        }

        return values;
      }
    }

    public double GetDoubleValue()
    {
      if (this.Type != TiffTagTypeEnum.Double)
      {
        throw new Exception("This tag does not contain double data.");
      }

      this.Stream.Position = this.ValueOffset;
      using (var reader = new BinaryReader(this.Stream, Encoding.UTF8, true))
      {
        return reader.ReadDouble();
      }
    }

    public double[] GetDoubleValues()
    {
      if (this.Type != TiffTagTypeEnum.Double)
      {
        throw new Exception("This tag does not contain double data.");
      }

      this.Stream.Position = this.ValueOffset;
      using (var reader = new BinaryReader(this.Stream, Encoding.UTF8, true))
      {
        var values = new double[this.Count];

        for (int i = 0; i < this.Count; i++)
        {
          values[i] = reader.ReadDouble();
        }

        return values;
      }
    }

    public ReadOnlySpan<byte> GetBytes()
    {
      if (!this.IsOffset())
      {
        return this.ByteHandler.ToByteArray(this.ValueOffset);
      }

      this.Stream.Position = this.ValueOffset;
      using (var reader = new BinaryReader(this.Stream, Encoding.UTF8, true))
      {
        return reader.ReadBytes(this.GetByteCount() * this.Count);
      }
    }

    private void Load(ReadOnlySpan<byte> bytes)
    {
      this.Tag = (TiffTagEnum)this.ByteHandler.ReadUInt16(bytes.Slice(0, 2));
      this.Type = (TiffTagTypeEnum)this.ByteHandler.ReadUInt16(bytes.Slice(2, 2));
      this.Count = (int)this.ByteHandler.ReadUInt32(bytes.Slice(4, 4));
      this.ValueOffset = this.ByteHandler.ReadUInt32(bytes.Slice(8, 4));
    }

    private int GetByteCount()
    {
      switch (this.Type)
      {
        case TiffTagTypeEnum.Byte:
        case TiffTagTypeEnum.Ascii:
          return 1;
        case TiffTagTypeEnum.Short:
          return 2;
        case TiffTagTypeEnum.Long:
          return 4;
        case TiffTagTypeEnum.Rational:
        case TiffTagTypeEnum.Double:
          return 8;
        default:
          throw new Exception("Unknown tiff tag type.");
      }
    }

    private bool IsOffset()
    {
      return this.GetByteCount() * this.Count > 4;
    }
  }
}
