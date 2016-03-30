using System.Drawing;
using System.Drawing.Imaging;

namespace Sprite
{
    public class SpriteBitmap
    {
        public SizeF FrameSize { get; set; }
        public Size FramePixelSize { get; set; }

        public Size Size { get; set; }
        public int Cols { get; set; }
        public int Rows { get; set; }
        public uint GLTexture { get; set; }

        public SpriteBitmap(Size size, int cols = 0, int rows = 0)
        {
            Size = size;
            Cols = cols;
            Rows = rows;
            FrameSize = new SizeF(Cols == 0 ? 1 : 1.0f / Cols, Rows == 0 ? 1 : 1.0f / Rows);
            FramePixelSize = new Size(Cols == 0 ? Size.Width : Size.Width / Cols, Rows == 0 ? Size.Height : Size.Height / Rows);
            uint[] textureNames = new uint[1];
            Gl.glGenTextures(1, textureNames);
            GLTexture = textureNames[0];
        }

        public static SpriteBitmap FromImage(Bitmap bitmap, int cols = 0, int rows = 0)
        {
            var glBitmap = new SpriteBitmap(bitmap.Size, cols, rows);
            var bmpData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);

            const int GL_UNSIGNED_INT_8_8_8_8_REV = 0x8367;
            const int GL_BGRA = 0x80E1;
            Gl.glBindTexture((uint) Gl.GetTarget.GL_TEXTURE_2D, glBitmap.GLTexture);

            Gl.glTexImage2D((uint) Gl.GetTarget.GL_TEXTURE_2D, 0, (int) Gl.Texture.GL_RGBA8, bmpData.Width, bmpData.Height, 0, GL_BGRA, GL_UNSIGNED_INT_8_8_8_8_REV, bmpData.Scan0);
            Gl.glTexParameteri((uint)Gl.GetTarget.GL_TEXTURE_2D, (uint) Gl.TextureParameterName.GL_TEXTURE_MAG_FILTER, (int) Gl.TextureMagFilter.GL_NEAREST);
            Gl.glTexParameteri((uint)Gl.GetTarget.GL_TEXTURE_2D, (uint)Gl.TextureParameterName.GL_TEXTURE_MIN_FILTER, (int)Gl.TextureMagFilter.GL_NEAREST);
            Gl.glTexParameteri((uint) Gl.GetTarget.GL_TEXTURE_2D, (uint) Gl.TextureParameterName.GL_TEXTURE_WRAP_S, (int) Gl.TextureWrapMode.GL_CLAMP);
            Gl.glTexParameteri((uint)Gl.GetTarget.GL_TEXTURE_2D, (uint)Gl.TextureParameterName.GL_TEXTURE_WRAP_T, (int)Gl.TextureWrapMode.GL_CLAMP);

            bitmap.UnlockBits(bmpData);
            return glBitmap;
        }
    }
}
