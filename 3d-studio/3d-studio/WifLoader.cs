using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using WifViewer.Rendering;
using Avalonia;
using Avalonia.Platform;
using Avalonia.Media.Imaging;

namespace WifViewer
{
    public class WifLoader
    {
        public static unsafe List<WriteableBitmap> Load(string path)
        {
            var result = new List<WriteableBitmap>();

            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                while (true)
                {
                    var buffer = new byte[4];

                    fileStream.Read(buffer, 0, 4);
                    var width = BitConverter.ToUInt32(buffer, 0);

                    if (width == 0)
                        return result;

                    fileStream.Read(buffer, 0, 4);
                    var height = BitConverter.ToUInt32(buffer, 0);
                    buffer = new byte[4 * width * height];

                    for (int i = 0; i < width * height; i++)
                    {
                        fileStream.Read(buffer, i * 4, 3);
                        buffer[i + 3] = byte.MaxValue;
                    }

                    var bitmap = new WriteableBitmap(new PixelSize((int)width, (int)height), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Opaque);
                    
                    fixed (byte* ptr = buffer)
                    {
                        using var lockedBitmap = bitmap.Lock();
                        Buffer.MemoryCopy(ptr, (byte*)lockedBitmap.Address, buffer.Length, buffer.Length);
                    }

                    result.Add(bitmap);
                }
            }
        }

        public static unsafe void Load(string path, IRenderReceiver receiver)
        {
            using (var fileStream = new FileStream(path, FileMode.Open))
            {
                while (true)
                {
                    var buffer = new byte[4];

                    fileStream.Read(buffer, 0, 4);
                    var width = BitConverter.ToUInt32(buffer, 0);

                    if (width == 0)
                    {
                        break;
                    }

                    fileStream.Read(buffer, 0, 4);
                    var height = BitConverter.ToUInt32(buffer, 0);
                    buffer = new byte[4 * width * height];

                    for (int i = 0; i < width * height; i++)
                    {
                        fileStream.Read(buffer, i * 4, 3);
                        buffer[i + 3] = byte.MaxValue;
                    }

                    var bitmap = new WriteableBitmap(new PixelSize((int)width, (int)height), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Opaque);

                    fixed (byte* ptr = buffer)
                    {
                        using var lockedBitmap = bitmap.Lock();
                        Buffer.MemoryCopy(ptr, (byte*)lockedBitmap.Address, buffer.Length, buffer.Length);
                    }

                    receiver.FrameRendered(bitmap);
                }

                receiver.RenderingDone();
            }
        }

        public static void LoadInSeparateThread(string path, IRenderReceiver receiver)
        {
            var thread = new Thread(() => Load(path, receiver));
            thread.Start();
        }

        public static unsafe WriteableBitmap DecodeFrame(byte[] buffer)
        {
            var width = BitConverter.ToUInt32(buffer, 0);

            if (width == 0)
                return null;

            var height = BitConverter.ToUInt32(buffer, 4);

            var copy = new byte[4 * width * height];

            for (int i = 0; i < width * height; i++)
            {
                int pc = i * 4;
                int pb = i * 3;
                copy[pc] = buffer[pb];
                copy[pc+1] = buffer[pb+1];
                copy[pc+2] = buffer[pb+2];
                copy[pc+3] = byte.MaxValue;
            }

            var bitmap = new WriteableBitmap(new PixelSize((int)width, (int)height), new Vector(96, 96), PixelFormat.Rgba8888, AlphaFormat.Opaque);

            fixed (byte* ptr = copy)
            {
                using var lockedBitmap = bitmap.Lock();
                Buffer.MemoryCopy(ptr, (byte*)lockedBitmap.Address, copy.Length, copy.Length);
            }

            return bitmap;
        }
    }
}
