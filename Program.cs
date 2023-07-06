using System.Drawing;
using System.Drawing.Imaging;

#pragma warning disable CA1416

class binarization
{
    public Bitmap binarizationFast(Bitmap grayImage, byte threshold, bool useMean = false)
    {

         if (grayImage.PixelFormat != PixelFormat.Format8bppIndexed)
        {
            throw new ArgumentException("Parameter should be (8bpp) image");
        }



        Bitmap binImage = new Bitmap(grayImage.Width, grayImage.Height, PixelFormat.Format1bppIndexed);


        BitmapData GrayData = grayImage.LockBits(new Rectangle(0, 0, grayImage.Width, grayImage.Height),
            ImageLockMode.ReadWrite, PixelFormat.Format8bppIndexed);

        BitmapData binData = binImage.LockBits(new Rectangle(0, 0, grayImage.Width, grayImage.Height),
            ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

        int Height = grayImage.Height;
        int Width = grayImage.Width;

        int stride = GrayData.Stride;
        System.IntPtr Scan0 = GrayData.Scan0;

        unsafe
        {
            byte* p = (byte*)(void*)Scan0;

            byte mean;

            if (useMean)
            {
                int sum = 0;
                byte* left = (byte*)(void*)Scan0;
                byte* end = left + Height * stride;

                while(left < end)
                {
                    sum += *left++;
                }
                mean = (byte)(sum / (Width * Height));
                threshold = mean;
            }

            for (int y = 0; y < Height; y++)
            {
                byte* grayRow = (byte*)GrayData.Scan0 + y * GrayData.Stride;
                byte* binRow = (byte*)binData.Scan0 + y * binData.Stride;
                for (int x = 0; x < Width; x++)
                {
                    if (*grayRow++ >= threshold)
                    {
                        binRow[x / 8] |= (byte)(0x80 >> (x % 8));
                    }
                }
            }
        }
        grayImage.UnlockBits(GrayData);
        binImage.UnlockBits(binData);
        return binImage;
    }
    public void Run()
    {
        string input;
        byte threshold;
        
        string dir = @"results";
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        Console.Write("Path of the Image : ");
        input = Console.ReadLine();
        Console.Write("threshold 0-255 : ");
        threshold = (byte)int.Parse(Console.ReadLine());
        
        Bitmap b = new Bitmap(input);
        
        var watch = new System.Diagnostics.Stopwatch();
        watch.Start();

       
        binarizationFast(b, threshold, false).Save("results/Stastic_Threshold.bmp");
        binarizationFast(b, threshold, true).Save("results/Mean_Threshold.bmp");

        watch.Stop();
        Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
    }
}
class Solution
{
      static void Main(String[] args)
    {
        binarization b = new binarization();
        b.Run();
    }
}
