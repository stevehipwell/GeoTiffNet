using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Buffers.Binary;
using System.Text;

namespace GeoTiffNet
{
  public class TiffHeader : ITiffHeader
  {
    private const int HeaderBytes = 8;

    public TiffHeader(Stream stream, bool isGeoTiff) : this(isGeoTiff)
    {
      using (var reader = new BinaryReader(stream, Encoding.UTF8, true))
      {
        this.Load(reader.ReadBytes(HeaderBytes));
      }
    }

    public TiffHeader(ReadOnlySpan<byte> bytes, bool isGeoTiff) : this(isGeoTiff)
    {
      this.Load(bytes);
    }

    private TiffHeader(bool isGeoTiff)
    {
      this.IsGeoTiff = isGeoTiff;
    }

    public bool IsGeoTiff { get; private set; }

    public TiffByteOrderEnum ByteOrder { get; private set; }

    public IByteHandler ByteHandler { get; private set; }

    public ushort VersionNumber { get; private set; }

    public long FirstImageOffset { get; private set; }

    private void Load(ReadOnlySpan<byte> bytes)
    {
      this.ByteOrder = (TiffByteOrderEnum)BinaryPrimitives.ReadInt16LittleEndian(bytes.Slice(0, 2));
      this.ByteHandler = new ByteHandler(this.ByteOrder == TiffByteOrderEnum.BigEndian);
      this.VersionNumber = this.ByteHandler.ReadUInt16(bytes.Slice(2, 2));
      this.FirstImageOffset = this.ByteHandler.ReadUInt32(bytes.Slice(4, 4));
    }
  }
}
