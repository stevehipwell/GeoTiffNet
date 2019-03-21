using System;

namespace GeoTiffNet
{
  public enum TiffTagEnum : ushort
  {
    ImageWidth = 256,
    ImageLength = 257,
    BitsPerSample = 258,
    Compression = 259,
    PhotometricInterpretation = 262,
    StripOffsets = 273,
    SamplesPerPixel = 277,
    RowsPerStrip = 278,
    StripByteCounts = 279,
    XResolution = 282,
    YResolution = 283,
    PlanarConfiguration = 284,
    ResolutionUnit = 296,
    ColorMap = 320,
    SampleFormat = 339,
    ModelPixelScale = 33550,
    ModelTiepoint = 33922,
    ModelTransformation = 34264,
    GeoKeyDirectory = 34735,
    GeoDoubleParams = 34736,
    GeoAsciiParams = 34737
  }
}
