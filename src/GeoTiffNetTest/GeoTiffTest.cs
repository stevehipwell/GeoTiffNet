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
      using (var tiff = new GeoTiff(@"C:\Data\OS\OpenMap\TM\data\TM47SE.tif"))
      {
        var index = 0;
        foreach (var image in tiff.GetImages())
        {
          Console.WriteLine("----------------------------------------------------");
          Console.WriteLine("Image: {0}", index++);

          Console.WriteLine("  Fields:");
          foreach (var field in image.Fields)
          {
            Console.WriteLine("    {0}: {1}", field.Tag, field.ValueOffset);
          }

          Console.WriteLine("  GeoKeys:");
          foreach (var key in image.GeoKeys)
          {
            Console.WriteLine("    {0}: {1}", key.Tag, key.StringValue as object ?? key.Int32Value ?? key.DoubleValue);
          }

          Console.WriteLine("----------------------------------------------------");
        }
      }

      Assert.True(true);
    }
  }
}
