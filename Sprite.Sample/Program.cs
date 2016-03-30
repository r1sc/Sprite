using System.Drawing;

namespace Sprite.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var window = new SpriteWindow("Test");
            var glBitmap = SpriteBitmap.FromImage(Properties.Resources.test, 4, 4);
            var i = 0;
            while (window.Running)
            {
                for (int y = 0; y < 25; y++)
                {
                    for (int x = 0; x < 40; x++)
                    {
                        window.DrawGlBitmap(glBitmap, x * 8, y * 8, 5);
                    }
                }

                if (window.IsKeyDown(77))
                {
                    i++;
                }
                if (window.IsKeyDown(72))
                {
                    i--;
                }

                window.DrawGlBitmap(glBitmap, 12+i, 15, 0);
                window.Swap();
            }
        }
    }
}
