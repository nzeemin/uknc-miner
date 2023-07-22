using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SpriteRotate
{
    class Program
    {
        private static Bitmap bmp;

        private static readonly Color[] colors = new Color[]
        {
            Color.Black,
            Color.FromArgb(255, 255, 0, 0),
            Color.FromArgb(255, 0, 0, 255),
            Color.FromArgb(255, 0, 255, 255),
        };

        static void Main(string[] args)
        {
            bmp = (Bitmap)Bitmap.FromFile(@"..\tiles.png");

            FileStream fs = new FileStream("TILES.MAC", FileMode.Create);
            StreamWriter writer = new StreamWriter(fs);
            writer.WriteLine("; START OF TILES.MAC");
            writer.WriteLine();

            ProcessSprite(writer, "SPLOGO", 2, 2, 128 / 8, 36);

            //TODO: Cursor

            ProcessSprite(writer, "SPFLAG", 56, 58, 16 / 8, 16);
            ProcessSprite(writer, "SPQSGN", 74, 58, 16 / 8, 16);
            ProcessSprite(writer, "SPUNKN", 92, 58, 16 / 8, 16);
            ProcessSprite(writer, "SPMINE",  2, 58, 16 / 8, 16);

            for (int i = 0; i < 9; i++)
                ProcessSprite(writer, $"SP{i}", 2 + i * 18, 40, 16 / 8, 16);

            for (int i = 0; i < 10; i++)
                ProcessSprite(writer, $"SPN{i}", 2 + i * 16, 102, 16 / 8, 21);

            ProcessSprite(writer, "SPBLOCK", 20, 58, 16 / 8, 16);
            ProcessSprite(writer, "SPBAD", 2, 76, 24 / 8, 24);
            ProcessSprite(writer, "SPGOOD", 28, 76, 24 / 8, 24);
            ProcessSprite(writer, "SPWIN", 54, 76, 24 / 8, 24);
            ProcessSpriteWithMask(writer, "SPCURS", 38, 58, 16 / 8, 16);

            writer.WriteLine();
            writer.WriteLine("; END OF TILES.MAC");

            writer.Flush();
        }

        static void ProcessSprite(StreamWriter writer, string label, int x0, int y0, int cols, int rows)
        {
            writer.Write($"{label}:");

            int perline = 8;
            int count = 0;
            for (int row = 0; row < rows; row++)
            {
                int y = y0 + row;
                for (int col = 0; col < cols; col++)
                {
                    if (count % perline == 0)
                    {
                        writer.WriteLine();
                        writer.Write("\t.WORD\t");
                    }
                    else
                    {
                        writer.Write(", ");
                    }

                    int x = x0 + col * 8;
                    int word = 0;
                    for (int b = 0; b < 8; b++)
                    {
                        word = word >> 1;
                        int index = GetColorIndex(bmp, x + b, y);
                        word |= (index & 1) << 7;
                        word |= (index & 2) << 14;
                    }
                    writer.Write(EncodeOctalString2(word));

                    count++;
                }
            }

            writer.WriteLine();
            writer.WriteLine("\t.EVEN");
        }

        static void ProcessSpriteWithMask(StreamWriter writer, string label, int x0, int y0, int cols, int rows)
        {
            writer.Write($"{label}:");

            int perline = 4;
            int count = 0;
            for (int row = 0; row < rows; row++)
            {
                int y = y0 + row;
                for (int col = 0; col < cols; col++)
                {
                    if (count % perline == 0)
                    {
                        writer.WriteLine();
                        writer.Write("\t.WORD\t");
                    }
                    else
                    {
                        writer.Write(", ");
                    }

                    int x = x0 + col * 8;
                    int mask = 0;
                    int word = 0;
                    for (int b = 0; b < 8; b++)
                    {
                        word = word >> 1;
                        mask = mask >> 1;
                        if (bmp.GetPixel(x + b, y) != Color.FromArgb(255, 128, 128, 128))
                        {
                            mask |= (1 << 7) | (2 << 14);
                            int index = GetColorIndex(bmp, x + b, y);
                            word |= (index & 1) << 7;
                            word |= (index & 2) << 14;
                        }
                    }
                    writer.Write(EncodeOctalString2(mask));
                    writer.Write(",");
                    writer.Write(EncodeOctalString2(word));

                    count++;
                }
            }

            writer.WriteLine();
            writer.WriteLine("\t.EVEN");
        }

        static int GetColorIndex(Bitmap bmp, int x, int y)
        {
            Color color = bmp.GetPixel(x, y);
            for (int i = 0; i < 4; i++)
            {
                if (colors[i] == color)
                    return i;
            }
            return 0;
        }

        static string EncodeOctalString(byte value)
        {
            //convert to int, for cleaner syntax below. 
            int x = (int)value;

            return string.Format(
                @"{0}{1}{2}",
                ((x >> 6) & 7),
                ((x >> 3) & 7),
                (x & 7)
            );
        }

        static string EncodeOctalString2(int x)
        {
            return string.Format(
                @"{0}{1}{2}{3}{4}{5}",
                ((x >> 15) & 7),
                ((x >> 12) & 7),
                ((x >> 9) & 7),
                ((x >> 6) & 7),
                ((x >> 3) & 7),
                (x & 7)
            );
        }

        static string EncodeHexString2(int x)
        {
            return x.ToString("X4");
        }
    }
}
