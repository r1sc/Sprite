using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Sprite
{
    public class CollisionCell
    {
        public Point? LeftFoot { get; set; }
        public Point? RightFoot { get; set; }
        public Point? Head { get; set; }
        public Rectangle? Body { get; set; }
        internal bool Collide(Point myOffset, Point otherOffset, CollisionCell otherCell, out bool body, out bool head, out bool leftFoot, out bool rightFoot)
        {
            body = false;
            head = false;
            leftFoot = false;
            rightFoot = false;
            //Is there anything to test?
            if (Body == null && otherCell.Body == null)
                return false;
            Size s = new Size(1, 1);
            //Apply all offsets.
            Rectangle? myBody = null, otherBody = null;
            if (Body != null)                { var temp = Body.Value; temp.Offset(myOffset); myBody = temp; }
            if (otherCell?.Body != null)     { var temp = otherCell.Body.Value; temp.Offset(otherOffset); otherBody = temp; }
            Point? myHead = null, otherHead = null, myLeftFoot = null, otherLeftFoot = null, myRightFoot = null, otherRightFoot = null;
            if (Head != null)                { var temp = Head.Value; temp.Offset(myOffset); myHead = temp; }
            if (otherCell.Head != null)      { var temp = otherCell.Head.Value; temp.Offset(otherOffset); otherHead = temp; }
            if (LeftFoot != null)            { var temp = LeftFoot.Value; temp.Offset(myOffset); myLeftFoot = temp; }
            if (otherCell.LeftFoot != null)  { var temp = otherCell.LeftFoot.Value; temp.Offset(otherOffset); otherLeftFoot = temp; }
            if (RightFoot != null)           { var temp = RightFoot.Value; temp.Offset(myOffset); myRightFoot = temp; }
            if (otherCell.RightFoot != null) { var temp = otherCell.RightFoot.Value; temp.Offset(otherOffset); otherRightFoot = temp; }
            //Check if my body collides with other body.
            if (myBody != null && otherBody != null)
                body = myBody.Value.IntersectsWith(otherBody.Value);
            //Check if my head or feed collides with other body.
            if (otherBody != null)
            {
                if (myHead != null)
                    head = otherBody.Value.IntersectsWith(new Rectangle(myHead.Value, s));
                if (myLeftFoot != null)
                    leftFoot = otherBody.Value.IntersectsWith(new Rectangle(myLeftFoot.Value, s));
                if (myRightFoot != null)
                    rightFoot = otherBody.Value.IntersectsWith(new Rectangle(myRightFoot.Value, s));
            }
            //Check if my body collides collides with other head or feet.
            if (myBody != null)
            {
                if (otherHead != null)
                    body = body || myBody.Value.IntersectsWith(new Rectangle(otherHead.Value, s));
                if (otherLeftFoot != null)
                    body = body || myBody.Value.IntersectsWith(new Rectangle(otherLeftFoot.Value, s));
                if (otherRightFoot != null)
                    body = body || myBody.Value.IntersectsWith(new Rectangle(otherRightFoot.Value, s));
            }
            return body || head || leftFoot || rightFoot;
        }
    }
}
