using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoTiffNet
{
  public class GeoKey : IGeoKey
  {
    public const int FieldCount = 4;

    public GeoKey(ReadOnlySpan<ushort> values, IList<double> doubles, ReadOnlySpan<byte> ascii)
    {
      this.Load(values, doubles, ascii);
    }

    public GeoKeyEnum Tag { get; private set; }

    public int? Int32Value { get; private set; }

    public double? DoubleValue { get; private set; }

    public string StringValue { get; private set; }

    private void Load(ReadOnlySpan<ushort> values, IList<double> doubles, ReadOnlySpan<byte> ascii)
    {
      this.Tag = (GeoKeyEnum)values[0];

      var location = (GeoKeyLocationEnum)values[1];
      var count = values[2];
      var valueOffset = values[3];

      switch (location)
      {
        case GeoKeyLocationEnum.DoubleParams:
          this.DoubleValue = doubles[valueOffset];
          break;
        case GeoKeyLocationEnum.AsciiParams:
          this.StringValue = Encoding.ASCII.GetString(ascii.ToArray(), valueOffset, count).TrimEnd('|');
          break;
        default:
          this.Int32Value = valueOffset;
          break;
      }
    }

    public override string ToString()
    {
      return string.Format("{0}: {1}", this.Tag, this.StringValue as object ?? this.DoubleValue ?? this.Int32Value);
    }
  }
}
