using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Security.Cryptography;
using System.Drawing;
using System.IO;

namespace SolveFreeCell
{
    class BitMapHash
    {
        private static System.Drawing.ImageConverter m_ic = new System.Drawing.ImageConverter();
        private static SHA256Managed m_shaM = new SHA256Managed();
        private static Type m_Type = (new byte[1]).GetType();

        private byte[] byteHash;
        private int m_Value;
        public int Value
        {
            get
            {
                return m_Value;
            }
        }
        public BitMapHash(int nValue, Bitmap bmap)
        {
            m_Value = nValue;
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    bmap.Save(stream, System.Drawing.Imaging.ImageFormat.MemoryBmp);
            //    stream.Close();

            //    //byte[] btImage = stream.ToArray();
            //    //byteHash = m_shaM.ComputeHash(btImage);
            //    byteHash = stream.ToArray();
            //}
            Rectangle rect = new Rectangle(0, 0, bmap.Width, bmap.Height);
            System.Drawing.Imaging.BitmapData bmpData = bmap.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,bmap.PixelFormat);

            // Get the address of the first line.
            IntPtr ptr = bmpData.Scan0;

            // Declare an array to hold the bytes of the bitmap.
            int bytes = Math.Abs(bmpData.Stride) * bmap.Height;
            byteHash = new byte[bytes];

            // Copy the RGB values into the array.
            System.Runtime.InteropServices.Marshal.Copy(ptr, byteHash, 0, bytes);

            bmap.UnlockBits(bmpData);

        }
        public int Compare(BitMapHash other)
        {
            int nBad = 0;
            for (int i = 0; i < other.byteHash.Length && i < byteHash.Length; i++)
            {
                if (other.byteHash[i] != byteHash[i])
                {
                    nBad++;
                }
            }
            return nBad;

        }
    }
}
