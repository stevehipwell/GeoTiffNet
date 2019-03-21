using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoTiffNet
{
  public class TiffField : ITiffField
  {
    public const int DirectoryByteCount = 12;

    private readonly IByteHandler ByteHandler;

    public TiffField(IByteHandler byteHandler, ReadOnlySpan<byte> bytes, Stream stream)
    {
      this.ByteHandler = byteHandler;

      this.Load(bytes, stream);
    }

    public TiffTagEnum Tag { get; private set; }

    public TiffTagTypeEnum Type { get; private set; }

    public int Count { get; private set; }

    public uint ValueOffset { get; private set; }

    public int BytesPerValue { get; private set; }

    public bool IsOffSet => this.BytesPerValue * this.Count > 4;

    public byte[] Bytes { get; private set; }

    public ushort[] GetUInt16Values()
    {
      if (this.Type != TiffTagTypeEnum.Short)
      {
        throw new Exception("This tag does not contain UInt16 data.");
      }

      var values = new ushort[this.Count];

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadUInt16(this.Bytes.AsSpan().Slice(i * this.BytesPerValue, this.BytesPerValue));
      }

      return values;
    }

    public uint[] GetUInt32Values()
    {
      if (this.Type != TiffTagTypeEnum.Long)
      {
        throw new Exception("This tag does not contain UInt32 data.");
      }

      var values = new uint[this.Count];

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadUInt32(this.Bytes.AsSpan().Slice(i * this.BytesPerValue, this.BytesPerValue));
      }

      return values;
    }

    public double[] GetDoubleValues()
    {
      if (this.Type != TiffTagTypeEnum.Double)
      {
        throw new Exception("This tag does not contain Double data.");
      }

      var values = new double[this.Count];

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadDouble(this.Bytes.AsSpan().Slice(i * this.BytesPerValue, this.BytesPerValue));
      }

      return values;
    }

    public string GetStringValue()
    {
      if (this.Type != TiffTagTypeEnum.Ascii)
      {
        throw new Exception("This tag does not contain String data.");
      }

      return Encoding.ASCII.GetString(this.Bytes, 0, this.Bytes.Length - 1);
    }

    public override string ToString()
    {
      return string.Format("{0} ({1}): {2}", this.Tag, this.Type, this.GetValueAsString());
    }

    private void Load(ReadOnlySpan<byte> bytes, Stream stream)
    {
      this.Tag = (TiffTagEnum)this.ByteHandler.ReadUInt16(bytes.Slice(0, 2));
      this.Type = (TiffTagTypeEnum)this.ByteHandler.ReadUInt16(bytes.Slice(2, 2));
      this.Count = (int)this.ByteHandler.ReadUInt32(bytes.Slice(4, 4));
      this.ValueOffset = this.ByteHandler.ReadUInt32(bytes.Slice(8, 4));
      this.BytesPerValue = this.GetBytesPerValue();

      if (!this.IsOffSet)
      {
        this.Bytes = bytes.Slice(8, 4).ToArray();
        return;
      }

      stream.Position = this.ValueOffset;
      using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
      {
        this.Bytes = reader.ReadBytes(this.BytesPerValue * this.Count);
      }
    }

    private int GetBytesPerValue()
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
          return 0;
      }
    }

    private string GetValueAsString()
    {
      switch (this.Type)
      {
        case TiffTagTypeEnum.Byte:
          return string.Join(", ", this.Bytes.Select(x => (char)x));
        case TiffTagTypeEnum.Ascii:
          return this.GetStringValue();
        case TiffTagTypeEnum.Short:
          return string.Join(", ", this.GetUInt16Values());
        case TiffTagTypeEnum.Long:
          return string.Join(", ", this.GetUInt32Values());
        case TiffTagTypeEnum.Double:
          return string.Join(", ", this.GetDoubleValues());
        default:
          return "Unknown";
      }
    }
  }
}

