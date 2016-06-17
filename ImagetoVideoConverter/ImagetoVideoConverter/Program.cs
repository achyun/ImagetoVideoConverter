using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using SharpAvi;
using SharpAvi.Output;


namespace ImageToVideoConverter
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(@"C:\Users\Public\Pictures\Sample Pictures","*.jpg");
            GenerateSingleImageVideo(files, "FileName");


        }
        public static void GenerateSingleImageVideo(string[] imagePaths, String fileName)
        {
            String frameRate = imagePaths.Count() + "";
            String videoLength = imagePaths.Count() + "";
            Bitmap thisBitmap;
            using (Stream BitmapStream = System.IO.File.Open(imagePaths.FirstOrDefault(), FileMode.Open))
            {
                Image img = Image.FromStream(BitmapStream);
                thisBitmap = new Bitmap(img);
            }

            int width = thisBitmap.Width;
            int height = thisBitmap.Height;
            //creates the writer of the file (to save the video)
            var writer = new AviWriter(fileName + ".avi")
            {
                FramesPerSecond = 1,
                EmitIndex1 = true
            };
            var stream = writer.AddVideoStream();
            stream.Width = width;
            stream.Height = height;
            stream.Codec = KnownFourCCs.Codecs.Uncompressed;
            stream.BitsPerPixel = BitsPerPixel.Bpp32;



            int numberOfFrames = ((int.Parse(frameRate)) * (int.Parse(videoLength)));
            int count = 0;
            var rndGen = new Random(); // do this only once in your app/class/IoC container


            while (count <= numberOfFrames)
            {
                int random = rndGen.Next(0, imagePaths.Count());
                String imagePath = imagePaths.ElementAt(random);
                //foreach (var imagePath in imagePaths)
                {
                    //generate bitmap from image file
                    using (Stream BitmapStream = System.IO.File.Open(imagePath, FileMode.Open))
                    {
                        Image img = Image.FromStream(BitmapStream);
                        thisBitmap = new Bitmap(img);
                    }

                    //convert the bitmap to a byte array
                    byte[] byteArray = BitmapToByteArray(thisBitmap);


                    byte[] Header = byteArray.Take(54).ToArray();
                    byte[] picture = byteArray.Skip(54).ToArray();

                    stream.WriteFrame(false, picture, 0, picture.Length);

                }
                count++;
            }
            writer.Close();

        }

        public static byte[] BitmapToByteArray(Image img)
        {
            byte[] byteArray = new byte[0];
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Close();

                byteArray = stream.ToArray();
            }
            return byteArray;
        }


    }
}

