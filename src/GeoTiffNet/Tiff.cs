using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoTiffNet
{
  public class Tiff : TiffBase, ITiff, IDisposable
  {
    public Tiff(string filePath) : base(File.OpenRead(filePath), true, false)
    {
    }

    public Tiff(byte[] bytes) : base(new MemoryStream(bytes), true, false)
    {
    }

    public Tiff(Stream stream) : base(stream, false, false)
    {
    }

    public int GetImageCount()
    {
      return this.GetImages().Count;
    }

    public IList<ITiffImage> GetImages()
    {
      return base.GetImagesBase().AsEnumerable<ITiffImage>().ToList();
    }

    public ITiffImage GetImage(int index)
    {
      return this.GetImages()[index];
    }
  }
}
