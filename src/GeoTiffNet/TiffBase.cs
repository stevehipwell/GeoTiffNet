using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GeoTiffNet
{
  public abstract class TiffBase : IDisposable
  {
    private const int HeaderByteCount = 8;

    protected IByteHandler ByteHandler;
    protected readonly Stream Stream;
    private readonly bool OwnsStream;
    private bool ImagesLoaded;
    private IList<TiffImage> Images;

    protected TiffBase(Stream stream, bool ownsStream, bool isGeoTiff)
    {
      this.Stream = stream;
      this.OwnsStream = ownsStream;
      this.IsGeoTiff = isGeoTiff;
      this.ImagesLoaded = false;

      this.Load();
    }

    ~TiffBase()
    {
      this.Dispose(false);
    }

    public TiffByteOrderEnum ByteOrder { get; private set; }

    public int VersionNumber { get; private set; }

    public bool IsGeoTiff { get; private set; }

    public long FirstImageOffset { get; private set; }

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

    private void Load()
    {
      this.Stream.Position = 0;
      using (var reader = new BinaryReader(this.Stream, Encoding.UTF8, true))
      {
        this.ByteOrder = (TiffByteOrderEnum)BinaryPrimitives.ReadInt16LittleEndian(reader.ReadBytes(2));
        this.ByteHandler = new ByteHandler(this.ByteOrder == TiffByteOrderEnum.BigEndian);
        this.VersionNumber = this.ByteHandler.ReadUInt16(reader.ReadBytes(2));
        this.FirstImageOffset = this.ByteHandler.ReadUInt32(reader.ReadBytes(4));
      }
    }

    private void LoadImages()
    {
      this.Images = new List<TiffImage>();

      long imageOffset = this.FirstImageOffset;
      while (imageOffset != 0)
      {
        var image = new TiffImage(this.ByteHandler, this.Stream, imageOffset, this.IsGeoTiff);
        this.Images.Add(image);

        imageOffset = image.NextImageOffset;
      }

      this.ImagesLoaded = true;
    }
  }
}
