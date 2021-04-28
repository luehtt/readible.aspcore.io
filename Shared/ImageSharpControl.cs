using System;
using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
 
namespace Readible.Shared
{
    public class ImageSharpControl
    {
        public static string Resize(string imageString, int maxWidth, int maxHeight, bool upscale = true)
        {
            using (var image = Image.Load(ToImageByte(imageString)))
            {
                var memoryStream = new MemoryStream();
                image.Mutate(x => x.AutoOrient());
                
                // resize image if needed
                var size = CalcSize(image.Width, image.Height, maxWidth, maxHeight, upscale);
                if (image.Width != size.Width || image.Height != size.Height)
                    image.Mutate(x => x.Resize(size.Width, size.Height));
                
                var format = GetFormat(imageString);
                switch (format)
                {
                    case "gif":
                        var gif = new GifEncoder();
                        image.Save(memoryStream, gif);
                        return image.ToBase64String(GifFormat.Instance);
                    case "png":
                        var png = new PngEncoder();
                        image.Save(memoryStream, png);
                        return image.ToBase64String(PngFormat.Instance);
                    default:
                        var jpeg = new JpegEncoder {Quality = Const.DEFAULT_JPEG_QUALITY};
                        image.Save(memoryStream, jpeg);
                        return image.ToBase64String(JpegFormat.Instance);
                }
            }
        }
        
        private static string TrimBase64Prefix(string base64String)
        {
            var iterator = base64String.IndexOf(',');
            return iterator > 1 ? base64String.Substring(iterator + 1) : base64String;
        }

        private static byte[] ToImageByte(string base64String)
        {
            return Convert.FromBase64String(TrimBase64Prefix(base64String));
        }

        private static System.Drawing.Size CalcSize(int imageWidth, int imageHeight, int maxWidth, int maxHeight, bool upscale)
        {
            var ratioWidth = imageWidth / (double) maxWidth;
            var ratioHeight = imageHeight / (double) maxHeight;
            var size = new System.Drawing.Size(imageWidth, imageHeight);
            if (!upscale && size.Width <= maxWidth && size.Height <= maxHeight) return size;
            
            var ratio = ratioWidth < ratioHeight ? ratioHeight : ratioWidth;
            size.Width = (int) (imageWidth / ratio);
            size.Height = (int) (imageHeight / ratio);
            return size;
        }

        private static string GetFormat(string imageString)
        {
            var start = imageString.IndexOf("/", StringComparison.Ordinal);
            var end = imageString.IndexOf(";", StringComparison.Ordinal);
            return imageString.Substring(start + 1, end - start - 1);
        }
    }
}