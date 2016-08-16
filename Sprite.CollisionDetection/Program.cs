using System;
using System.Drawing;

namespace Sprite.CollisionDetection
{
    class Program
    {
        private static SpriteBitmap glBitmap;
        private static GameSprite player;
        private static GameSprite other;

        static void Main(string[] args)
        {
            var gfx = new SpriteWindow("Collision detection", new Size(160, 100));
            gfx.OnLoadResources += Gfx_OnLoadResources;
            gfx.Swap(); //Let windows forms event happen.

            Console.WriteLine("Bodys collide with bodies, head and feet.");
            Console.WriteLine("Head and feed collide only with bodies.");
            var collideCount = 0;

            while (gfx.Running)
            {
                if (!gfx.IsPaused)
                {
                    //Game logic.
                    if (gfx.IsKeyDown(VirtualKeys.Right))
                        player.CurrentX++;
                    else if (gfx.IsKeyDown(VirtualKeys.Left))
                        player.CurrentX--;
                    if (gfx.IsKeyDown(VirtualKeys.Down))
                        player.CurrentY++;
                    else if (gfx.IsKeyDown(VirtualKeys.Up))
                        player.CurrentY--;
                    //Blurp out collision information to console window.
                    bool body, head, leftFoot, rightFoot;
                    if (player.Collide(other, out body, out head, out leftFoot, out rightFoot))
                    {
                        Console.WriteLine(collideCount++);
                        if (body)
                            Console.WriteLine("Body");
                        if (head)
                            Console.WriteLine("Head");
                        if (leftFoot)
                            Console.WriteLine("Left foot");
                        if (rightFoot)
                            Console.WriteLine("Right foot");
                    }
                }
                //Draw both characters.
                player.DrawGlBitmap(gfx);
                other.DrawGlBitmap(gfx);
                //Display
                gfx.Swap();
            }
        }

        private static void Gfx_OnLoadResources(object sender, System.EventArgs e)
        {
            glBitmap = SpriteBitmap.FromImage(Properties.Resources.mansprite, 2, 1);
            var allCollisions = new CollisionCell() {
                Body = new Rectangle(3, 3, 10, 10),
                Head = new Point(7, 0),
                LeftFoot = new Point(0, 15),
                RightFoot = new Point(15, 15)
            };
            player = new GameSprite(glBitmap)
            {
                FramePointer = 0,
                CurrentX = 10,
                CurrentY = 50,
                IsAlive = true
            };
            other = new GameSprite(glBitmap)
            {
                FramePointer = 1,
                CurrentX = 100,
                CurrentY = 50,
                IsAlive = true
            };
            player.CollisionCells[0] = allCollisions;
            other.CollisionCells[1] = allCollisions;
        }
    }
}
