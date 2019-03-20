using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GeoTiffNet
{
  public abstract class TiffBase : IDisposable
  {
    protected readonly Stream Stream;
    private readonly bool OwnsStream;
    protected readonly ITiffHeader Header;
    private bool ImagesLoaded;
    private IList<TiffImage> Images;

    protected TiffBase(Stream stream, bool ownsStream, bool isGeoTiff)
    {
      this.Stream = stream;
      this.OwnsStream = ownsStream;
      this.Header = new TiffHeader(this.Stream, isGeoTiff);
      this.ImagesLoaded = false;
    }

    ~TiffBase()
    {
      this.Dispose(false);
    }

    protected IList<TiffImage> GetImagesBase()
    {
      if (!this.ImagesLoaded)
      {
        this.LoadImages();
      }

      return this.Images;
    }

    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
      if (disposing)
      {
        if (this.OwnsStream)
        {
          this.Stream?.Dispose();
        }
      }
    }

    private void LoadImages()
    {
      this.Images = new List<TiffImage>();

      long imageOffset = this.Header.FirstImageOffset;
      while (imageOffset != 0)
      {
        var image = new TiffImage(this.Header.ByteHandler, this.Stream, imageOffset, this.Header.IsGeoTiff);
        this.Images.Add(image);

        imageOffset = image.NextImageOffset;
      }

      this.ImagesLoaded = true;
    }
  }
}
