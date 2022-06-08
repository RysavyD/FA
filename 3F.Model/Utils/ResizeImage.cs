using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace _3F.Model.Utils
{
    public static class ResizeImage
    {
        public static Image ResizeImageIfNeeded(Image image, int newWidth, int newHeight)
        {
            if (newWidth == 0 && newHeight == 0)
            {
                throw new InvalidOperationException("newWidth or newHeight must be specified.");
            }

            if (newWidth == 0)
            {
                newWidth = Convert.ToUInt16(image.Width * (newHeight / (double)image.Height));
            }
            else if (newHeight == 0)
            {
                newHeight = Convert.ToUInt16(image.Height * (newWidth / (double)image.Width));
            }

            var temp = new Bitmap(newWidth, newHeight);
            try
            {
                temp.SetResolution(image.HorizontalResolution, image.VerticalResolution);

                int srcWidth = image.Width;
                int srcHeight = image.Height;

                if (srcWidth >= newWidth || srcHeight >= newHeight)
                {
                    double scaleX = newWidth / (double)image.Width;
                    double scaleY = newHeight / (double)image.Height;

                    if (scaleY < scaleX)
                    {
                        double ratio = newWidth / (double)newHeight;
                        srcHeight = Convert.ToInt32(Math.Round(image.Width / ratio));
                    }
                    else
                    {
                        double ratio = newHeight / (double)newWidth;
                        srcWidth = Convert.ToInt32(Math.Round(image.Height / ratio));
                    }

                    int srcX = (image.Width - srcWidth) / 2;
                    int srcY = (image.Height - srcHeight) / 2;

                    Graphics g = Graphics.FromImage(temp);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.DrawImage(image, new Rectangle(0, 0, newWidth, newHeight), new Rectangle(srcX, srcY, srcWidth, srcHeight), GraphicsUnit.Pixel);
                }
                else
                {
                    var PositionX = (newWidth - srcWidth) / 2;
                    var PositionY = (newHeight - srcHeight) / 2;

                    Graphics g = Graphics.FromImage(temp);
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.FillRectangle(Brushes.White, new Rectangle(0, 0, newWidth, newHeight)); // background color
                    g.DrawRectangle(Pens.Black, 0, 0, newWidth - 1, newHeight - 1);
                    g.DrawImage(image, new Rectangle(PositionX, PositionY, srcWidth, srcHeight), new Rectangle(0, 0, srcWidth, srcHeight), GraphicsUnit.Pixel);
                }
            }
            catch
            {
                temp.Dispose();
                throw;
            }

            return temp;
        }
    }
}
