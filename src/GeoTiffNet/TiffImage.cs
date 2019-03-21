using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoTiffNet
{
  public class TiffImage : ITiffImage, IGeoTiffImage
  {
    private readonly IByteHandler ByteHandler;
    private readonly Stream Stream;

    public TiffImage(IByteHandler byteHandler, Stream stream, long offset, bool isGeoTiff)
    {
      this.ByteHandler = byteHandler;
      this.Stream = stream;

      this.Load(offset, isGeoTiff);
    }

    public IList<ITiffField> Fields { get; private set; }

    public IList<IGeoKey> GeoKeys { get; private set; }

    public long NextImageOffset { get; private set; }

    private void Load(long offset, bool isGeoTiff)
    {
      List<byte[]> byteCol;
      this.Stream.Position = offset;
      using (var reader = new BinaryReader(this.Stream, Encoding.UTF8, true))
      {
        var count = this.ByteHandler.ReadUInt16(reader.ReadBytes(2));
        byteCol = new List<byte[]>(count);

        for (int i = 0; i < count; i++)
        {
          byteCol.Add(reader.ReadBytes(TiffField.DirectoryByteCount));
        }

        this.NextImageOffset = this.ByteHandler.ReadUInt32(reader.ReadBytes(4));
      }

      this.LoadFields(byteCol);

      if (isGeoTiff)
      {
        this.LoadGeoKeys();
      }
    }

    private void LoadFields(List<byte[]> byteCol)
    {
      this.Fields = new List<ITiffField>();

      foreach (var bytes in byteCol)
      {
        var field = new TiffField(this.ByteHandler, bytes, this.Stream);

        if (Enum.IsDefined(typeof(TiffTagTypeEnum), field.Type))
        {
          this.Fields.Add(field);
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
        var geoKeyValues = geoKeyField.GetShortValues().AsSpan();
        var doubleValues = doublesField != null ? doublesField.GetDoubleValues() : new double[0];
        var ascii = asciiField != null ? asciiField.Bytes : new byte[0];

        if (geoKeyValues[0] != 1)
        {
          throw new Exception("Invalid GeoTiff, the directory bytes are badly formatted.");
        }

        var keyCount = geoKeyValues[3];
        for (int i = 0; i < keyCount; i++)
        {
          this.GeoKeys.Add(new GeoKey(geoKeyValues.Slice(GeoKey.FieldCount + (i * GeoKey.FieldCount), GeoKey.FieldCount), doubleValues, ascii));
        }
      }
    }
  }
}

