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

    public string GetAsciiValue()
    {
      if (this.Type != TiffTagTypeEnum.Ascii)
      {
        throw new Exception("This tag does not contain Ascii data.");
      }

      return Encoding.ASCII.GetString(this.Bytes, 0, this.Bytes.Length - 1);
    }

    public ushort[] GetShortValues()
    {
      if (this.Type != TiffTagTypeEnum.Short)
      {
        throw new Exception("This tag does not contain Short data.");
      }

      var values = new ushort[this.Count];
      var bytes = this.Bytes.AsSpan();

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadUInt16(bytes, i * this.BytesPerValue);
      }

      return values;
    }

    public uint[] GetLongValues()
    {
      if (this.Type != TiffTagTypeEnum.Long)
      {
        throw new Exception("This tag does not contain Long data.");
      }

      var values = new uint[this.Count];
      var bytes = this.Bytes.AsSpan();

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadUInt32(bytes, i * this.BytesPerValue);
      }

      return values;
    }

    public Tuple<uint, uint>[] GetRationalValues()
    {
      if (this.Type != TiffTagTypeEnum.Rational)
      {
        throw new Exception("This tag does not contain Rational data.");
      }

      var values = new Tuple<uint, uint>[this.Count];
      var bytes = this.Bytes.AsSpan();

      for (int i = 0; i < this.Count; i++)
      {
        var offset = i * this.BytesPerValue;
        var step = this.BytesPerValue / 2;

        values[i] = Tuple.Create(this.ByteHandler.ReadUInt32(bytes, offset), this.ByteHandler.ReadUInt32(bytes, offset + step));
      }

      return values;
    }

    public sbyte[] GetSByteValues()
    {
      if (this.Type != TiffTagTypeEnum.SByte)
      {
        throw new Exception("This tag does not contain SByte data.");
      }

      var values = new sbyte[this.Count];

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = (sbyte)this.Bytes[i];
      }

      return values;
    }

    public byte[] GetUndefinedValues()
    {
      if (this.Type != TiffTagTypeEnum.Undefined)
      {
        throw new Exception("This tag does not contain Undefined data.");
      }

      return this.Bytes;
    }

    public short[] GetSShortValues()
    {
      if (this.Type != TiffTagTypeEnum.SShort)
      {
        throw new Exception("This tag does not contain SShort data.");
      }

      var values = new short[this.Count];
      var bytes = this.Bytes.AsSpan();

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadInt16(bytes, i * this.BytesPerValue);
      }

      return values;
    }

    public int[] GetSLongValues()
    {
      if (this.Type != TiffTagTypeEnum.SLong)
      {
        throw new Exception("This tag does not contain SLong data.");
      }

      var values = new int[this.Count];
      var bytes = this.Bytes.AsSpan();

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadInt32(bytes, i * this.BytesPerValue);
      }

      return values;
    }

    public Tuple<int, int>[] GetSRationalValues()
    {
      if (this.Type != TiffTagTypeEnum.SRational)
      {
        throw new Exception("This tag does not contain SRational data.");
      }

      var values = new Tuple<int, int>[this.Count];
      var bytes = this.Bytes.AsSpan();

      for (int i = 0; i < this.Count; i++)
      {
        var offset = i * this.BytesPerValue;
        var step = this.BytesPerValue / 2;

        values[i] = Tuple.Create(this.ByteHandler.ReadInt32(bytes, offset), this.ByteHandler.ReadInt32(bytes, offset + step));
      }

      return values;
    }

    public float[] GetFloatValues()
    {
      if (this.Type != TiffTagTypeEnum.Float)
      {
        throw new Exception("This tag does not contain Float data.");
      }

      var values = new float[this.Count];
      var bytes = this.Bytes.AsSpan();

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadSingle(bytes, i * this.BytesPerValue);
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
      var bytes = this.Bytes.AsSpan();

      for (int i = 0; i < this.Count; i++)
      {
        values[i] = this.ByteHandler.ReadDouble(bytes, i * this.BytesPerValue);
      }

      return values;
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
        case TiffTagTypeEnum.SByte:
        case TiffTagTypeEnum.Undefined:
          return 1;
        case TiffTagTypeEnum.Short:
        case TiffTagTypeEnum.SShort:
          return 2;
        case TiffTagTypeEnum.Long:
        case TiffTagTypeEnum.SLong:
        case TiffTagTypeEnum.Float:
          return 4;
        case TiffTagTypeEnum.Rational:
        case TiffTagTypeEnum.SRational:
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
          return string.Join(", ", this.Bytes);
        case TiffTagTypeEnum.Ascii:
          return this.GetAsciiValue();
        case TiffTagTypeEnum.Short:
          return string.Join(", ", this.GetShortValues());
        case TiffTagTypeEnum.Long:
          return string.Join(", ", this.GetLongValues());
        case TiffTagTypeEnum.Rational:
          return string.Join<Tuple<uint, uint>>(", ", this.GetRationalValues());
        case TiffTagTypeEnum.SByte:
          return string.Join(", ", this.GetSByteValues());
        case TiffTagTypeEnum.Undefined:
          return string.Join(", ", this.GetUndefinedValues());
        case TiffTagTypeEnum.SShort:
          return string.Join(", ", this.GetSShortValues());
        case TiffTagTypeEnum.SLong:
          return string.Join(", ", this.GetSLongValues());
        case TiffTagTypeEnum.SRational:
          return string.Join<Tuple<int, int>>(", ", this.GetSRationalValues());
        case TiffTagTypeEnum.Float:
          return string.Join(", ", this.GetFloatValues());
        case TiffTagTypeEnum.Double:
          return string.Join(", ", this.GetDoubleValues());
        default:
          return "Unknown";
      }
    }
  }
}

