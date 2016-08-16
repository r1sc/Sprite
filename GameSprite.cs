using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sprite
{
    public class GameSprite
    {
        public SpriteBitmap SpriteBitmap { get; }
        public int FramePointer { get; set; }
        public int CellCount { get; }
        public CollisionCell[] CollisionCells { get; }
        public int CurrentX { get; set; }
        public int CurrentY { get; set; }
        public bool IsAlive { get; set; }
        public GameSprite(Bitmap bitmap, int cols = 0, int rows = 0)
        {
            SpriteBitmap = SpriteBitmap.FromImage(bitmap, cols, rows);
            FramePointer = 0;
            CellCount = cols * rows;
            CollisionCells = new CollisionCell[CellCount];
        }
        public GameSprite(SpriteBitmap bitmap)
        {
            SpriteBitmap = bitmap;
            FramePointer = 0;
            CellCount = bitmap.Cols * bitmap.Rows;
            CollisionCells = new CollisionCell[CellCount];
        }
        public void DrawGlBitmap(SpriteWindow gfx) =>
            gfx.DrawGlBitmap(SpriteBitmap, CurrentX, CurrentY, FramePointer);
        public bool Collide(GameSprite foreginSprite, out bool body, out bool head, out bool leftFoot, out bool rightFoot)
        {
            body = false;
            head = false;
            leftFoot = false;
            rightFoot = false;
            if (!IsAlive || foreginSprite == null || !foreginSprite.IsAlive)
                return false;
            var foreginZone = foreginSprite.GetCurrentCollisionCell();
            var myZone = GetCurrentCollisionCell();
            //At least one sprite must have a body.
            if (myZone == null && foreginZone == null)
                return false;
            return myZone.Collide(new Point(CurrentX, CurrentY), new Point(foreginSprite.CurrentX, foreginSprite.CurrentY), foreginZone, out body, out head, out leftFoot, out rightFoot);
        }
        internal CollisionCell GetCurrentCollisionCell() => CollisionCells[FramePointer];
    }
}
