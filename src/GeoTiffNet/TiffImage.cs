using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoTiffNet
{
  public class TiffImage : ITiffImage, IGeoTiffImage
  {
    private const int FieldBytes = 12;

    private readonly IEndianHandler ByteHandler;
    private readonly Stream Stream;
    private readonly long Offset;
    private IList<ITiffField> FieldCol;

    public TiffImage(IEndianHandler byteHandler, Stream stream, long offset, bool isGeoTiff)
    {
      this.ByteHandler = byteHandler;
      this.Stream = stream;
      this.Offset = offset;

      this.Load(isGeoTiff);
    }

    public IList<ITiffField> Fields => this.FieldCol;

    public IList<IGeoKey> GeoKeys { get; private set; }

    public long NextImageOffset { get; private set; }

    private void Load(bool isGeoTiff)
    {
      int count;

      this.Stream.Position = this.Offset;
      using (var reader = new BinaryReader(this.Stream, Encoding.UTF8, true))
      {
        count = this.ByteHandler.ReadUInt16(reader.ReadBytes(2));

        this.LoadFields(reader, count);

        this.NextImageOffset = this.ByteHandler.ReadUInt32(reader.ReadBytes(4));
      }

      if (isGeoTiff)
      {
        this.LoadGeoKeys();
      }
    }

    private void LoadFields(BinaryReader reader, int count)
    {
      this.FieldCol = new List<ITiffField>();

      for (int i = 0; i < count; i++)
      {
        var field = new TiffField(this.ByteHandler, this.Stream, reader.ReadBytes(FieldBytes));

        if (Enum.IsDefined(typeof(TiffTagTypeEnum), field.Type))
        {
          this.FieldCol.Add(field);
        }
      }
    }

    private void LoadGeoKeys()
    {
      this.GeoKeys = new List<IGeoKey>();

      var geoKeyField = this.Fields.FirstOrDefault(field => field.Tag == TiffTagEnum.GeoKeyDirectory);
      var doublesField = this.Fields.FirstOrDefault(field => field.Tag == TiffTagEnum.GeoDoubleParams);
      var asciiField = this.Fields.FirstOrDefault(field => field.Tag == TiffTagEnum.GeoAsciiParams);
      if (geoKeyField != null)
      {
        var geoKeyValues = geoKeyField.GetUInt16Values();
        var doubleValues = doublesField != null ? doublesField.GetDoubleValues() : new double[0];
        var ascii = asciiField != null ? asciiField.GetBytes() : new byte[0];

        var version = new Version(geoKeyValues[0], geoKeyValues[1], geoKeyValues[2]);
        if (version.Major != 1)
        {
          throw new Exception("Invalid GeoTiff, the directory bytes are badly formatted.");
        }

        var keyCount = geoKeyValues[3];
        for (int i = 0; i < keyCount; i++)
        {
          this.GeoKeys.Add(new GeoKey(this.ByteHandler, this.Stream, new ReadOnlySpan<ushort>(geoKeyValues).Slice(4 + (i * 4), 4), doubleValues, ascii));
        }
      }
    }
  }
}
