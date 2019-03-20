using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoTiffNet
{
  public class GeoTiff : TiffBase, IGeoTiff, IDisposable
  {
    public GeoTiff(string filePath) : base(File.OpenRead(filePath), true, true)
    {
    }

    public GeoTiff(byte[] bytes) : base(new MemoryStream(bytes), true, true)
    {
    }

    public GeoTiff(Stream stream) : base(stream, false, true)
    {
    }

    public int GetImageCount()
    {
      return this.GetImages().Count;
    }

    public IList<IGeoTiffImage> GetImages()
    {
      return base.GetImagesBase().AsEnumerable<IGeoTiffImage>().Where(image => image.GeoKeys.Any()).ToList();
    }

    public IGeoTiffImage GetImage(int index)
    {
      return this.GetImages()[index];
    }
  }
}
