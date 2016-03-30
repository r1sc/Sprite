using System.Drawing;

namespace Sprite.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var gfx = new SpriteWindow("Test");
            var glBitmap = SpriteBitmap.FromImage(Properties.Resources.test, 4, 4);
            var i = 0;
            while (gfx.Running)
            {
                for (int y = 0; y < 25; y++)
                {
                    for (int x = 0; x < 40; x++)
                    {
                        gfx.DrawGlBitmap(glBitmap, x * 8, y * 8, 5);
                    }
                }

                if (gfx.IsKeyDown(77))
                {
                    i++;
                }
                if (gfx.IsKeyDown(72))
                {
                    i--;
                }

                gfx.DrawGlBitmap(glBitmap, 12+i, 15, 0);
                gfx.Swap();
            }
        }
    }
}
