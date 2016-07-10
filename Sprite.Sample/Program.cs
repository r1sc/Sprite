using System.Drawing;

namespace Sprite.Sample
{
    class Program
    {
        private static SpriteBitmap glBitmap;

        static void Main(string[] args)
        {
            var gfx = new SpriteWindow("Test", new Size(320, 200));
            gfx.OnLoadResources += Gfx_OnLoadResources;
            gfx.Swap(); //Let windows forms event happen.
            var i = 0;
            float p = 0;
            var frame = 0;
            var yOffset = 0;

            while (gfx.Running)
            {
                if (!gfx.IsPaused)
                {
                    //Game logic.
                    yOffset = (yOffset + 1) % 8;

                    p += 0.1f;
                    if (p > 1)
                        p = 0;

                    frame = 0;
                    if (gfx.IsKeyDown(VirtualKeys.Right))
                    {
                        i++;
                        frame = 2;
                    }
                    else if (gfx.IsKeyDown(VirtualKeys.Left))
                    {
                        i--;
                        frame = 1;
                    }

                }
                //Draw background.
                for (int y = -1; y < 30; y++)
                {
                    for (int x = 0; x < 40; x++)
                    {
                        gfx.DrawGlBitmap(glBitmap, x * 8, y * 8 + yOffset, 5);
                    }
                }
                //Draw ship
                Gl.glColor4f(1, 1, 1, 1);
                gfx.DrawGlBitmap(glBitmap, 12+i, 200-16, frame);
                Gl.glColor4f(1, 1, 1, 1);
                //Display
                gfx.Swap();
            }
        }

        private static void Gfx_OnLoadResources(object sender, System.EventArgs e) =>
            glBitmap = SpriteBitmap.FromImage(Properties.Resources.test, 4, 4);
    }
}
