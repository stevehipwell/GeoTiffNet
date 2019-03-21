using System;
using FakeItEasy;
using Xunit;

using GeoTiffNet;

namespace GeoTiffNetTest
{
  public class GeoTiffTest
  {
    [Fact]
    public void GeoTiffsCanBeLoaded()
    {
      using (var tiff = new GeoTiff(@"/home/steve/downloads/cea.tif"))
      {
        var index = 0;
        foreach (var image in tiff.GetImages())
        {
          Console.WriteLine("----------------------------------------------------");
          Console.WriteLine("Image: {0}", index++);

          Console.WriteLine("  Fields:");
          foreach (var field in image.Fields)
          {
            Console.WriteLine("    {0}", field);
          }

          Console.WriteLine("  GeoKeys:");
          foreach (var key in image.GeoKeys)
          {
            Console.WriteLine("    {0}", key);
          }

          Console.WriteLine("----------------------------------------------------");
        }
      }

      Assert.True(true);
    }
  }
}
