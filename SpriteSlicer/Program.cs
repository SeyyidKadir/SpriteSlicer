using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Threading;
namespace SpriteSlicer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 1)
            {
                string path1 = args[0].Replace(".", "_") + "_slices";
                Directory.CreateDirectory(path1);
                Bitmap bmpKaynak = new Bitmap(args[0]);
                bmpKaynak = TransparanYap(bmpKaynak);
                var dilimler = FindSprites(bmpKaynak);
                if (dilimler.Count() == 0)
                {
                    Console.Write("[-] Ayristirilamadi !");
                    Console.ReadKey();
                }
                else
                {
                    int bulunan = 1;
                    Console.WriteLine($"[*] {dilimler.Count} Kadar bulundu.");
                    Console.WriteLine("[+] Islem baslatiliyor..");
                    foreach (var dilim in dilimler)
                    {
                        Bitmap bmpKlon = bmpKaynak.Clone(dilim, bmpKaynak.PixelFormat);
                        bmpKlon.Save(path1 + "\\" + bulunan.ToString() + ".png", ImageFormat.Png);
                        bulunan++;
                    }
                    Console.WriteLine("[+] Islem tamamlandi...\r\nCikiliyor.....");
                    Console.ReadKey();
                }
            }
        }
       
        // Sprite'ları otomatik olarak bulma fonksiyonu
        static List<Rectangle> FindSprites(Bitmap source)
        {
            var sprites = new List<Rectangle>();
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            bool[,] visited = new bool[sourceWidth, sourceHeight];

            // Her pikseli kontrol et
            for (int y = 0; y < sourceHeight; y++)
            {
                for (int x = 0; x < sourceWidth; x++)
                {
                    // Eğer bu piksel şeffaf değilse ve henüz ziyaret edilmediyse
                    if (!IsTransparent(source.GetPixel(x, y)) && !visited[x, y])
                    {
                        // Burada yeni bir sprite başlıyor
                        Rectangle spriteBounds = FindSpriteBounds(source, x, y, visited);
                        if (spriteBounds.Width > 0 && spriteBounds.Height > 0)
                        {
                            sprites.Add(spriteBounds);
                        }
                    }
                }
            }

            return sprites;
        }

        // Şeffaf olup olmadığını kontrol et
        static bool IsTransparent(Color pixel)
        {
            return pixel.A == 0; // Alpha kanalını kontrol et (şeffaflık)
        }

        // Yeni bir sprite'ın sınırlarını bulma
        static Rectangle FindSpriteBounds(Bitmap source, int startX, int startY, bool[,] visited)
        {
            int sourceWidth = source.Width;
            int sourceHeight = source.Height;

            int minX = startX, maxX = startX, minY = startY, maxY = startY;

            // BFS veya DFS ile sprite'ın sınırlarını keşfet
            var queue = new Queue<Point>();
            queue.Enqueue(new Point(startX, startY));
            visited[startX, startY] = true;

            while (queue.Count > 0)
            {
                Point current = queue.Dequeue();
                int x = current.X, y = current.Y;

                // Sınırları güncelle
                minX = Math.Min(minX, x);
                maxX = Math.Max(maxX, x);
                minY = Math.Min(minY, y);
                maxY = Math.Max(maxY, y);

                // Komşu pikselleri kontrol et (yukarı, aşağı, sağ, sol)
                CheckNeighbor(x - 1, y, queue, visited, source);
                CheckNeighbor(x + 1, y, queue, visited, source);
                CheckNeighbor(x, y - 1, queue, visited, source);
                CheckNeighbor(x, y + 1, queue, visited, source);
            }

            return new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        // Komşu pikselleri kontrol et
        static void CheckNeighbor(int x, int y, Queue<Point> queue, bool[,] visited, Bitmap source)
        {
            int width = source.Width;
            int height = source.Height;

            if (x >= 0 && y >= 0 && x < width && y < height && !visited[x, y] && !IsTransparent(source.GetPixel(x, y)))
            {
                visited[x, y] = true;
                queue.Enqueue(new Point(x, y));
            }
        }
        static Bitmap TransparanYap(Bitmap kaynak)
        {
            Bitmap sonuc = new Bitmap(kaynak);
            Color renkL = sonuc.GetPixel(0, 0);
            for (int i = 0; i < sonuc.Width; i++)
            {
                for (int j = 0; j < sonuc.Height; j++)
                {
                    if (sonuc.GetPixel(i, j) == renkL)
                    {
                        sonuc.SetPixel(i, j, Color.Transparent);
                    }
                  
                }
            }
            return sonuc;
        }
    }
}
