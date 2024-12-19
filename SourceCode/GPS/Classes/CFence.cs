using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace AgOpenGPS
{
    public partial class CBoundary
    {
        public List<vec3> bndBeingMadePts = new List<vec3>(128);

        public double createBndOffset;
        public bool isBndBeingMade;

        public bool isDrawRightSide = true, isOkToAddPoints = false;

        public int closestFenceNum;

        //point at the farthest boundary segment from pivotAxle
        public vec3 closestFencePt = new vec3(-10000, -10000, 9);

        public bool IsPointInsideFenceArea(vec3 testPoint)
        {
            //first where are we, must be inside outer and outside of inner geofence non drive thru turn borders
            if (bndList[0].fenceLineEar.IsPointInPolygon(testPoint))
            {
                for (int i = 1; i < bndList.Count; i++)
                {
                    //make sure not inside a non drivethru boundary
                    //if (bndList[i].isDriveThru) continue;
                    if (bndList[i].fenceLineEar.IsPointInPolygon(testPoint))
                    {
                        return false;
                    }
                }
                //we are safely inside outer, outside inner boundaries
                return true;
            }
            return false;
        }

        public bool IsPointInsideFenceArea(vec2 testPoint)
        {
            //first where are we, must be inside outer and outside of inner geofence non drive thru turn borders
            if (bndList[0].fenceLineEar.IsPointInPolygon(testPoint))
            {
                for (int i = 1; i < bndList.Count; i++)
                {
                    //make sure not inside a non drivethru boundary
                    //if (bndList[i].isDriveThru) continue;
                    if (bndList[i].fenceLineEar.IsPointInPolygon(testPoint))
                    {
                        return false;
                    }
                }
                //we are safely inside outer, outside inner boundaries
                return true;
            }
            return false;
        }

        public void DrawFenceLines()
        {
            if (!mf.mc.isOutOfBounds)
            {
                GL.Color4(0,0,0,0.0); // change from (0,0,0,0.)
                GL.LineWidth(0); // change from 6 to 0

                for (int i = 0; i < bndList.Count; i++)
                {
                    bndList[i].fenceLineEar.DrawPolygon();
                }

                GL.Color4(0.031f, 0.024f, 0.847f, 1.0f); //change color boundary 
                GL.LineWidth(mf.ABLine.lineWidth * 4); // change from 2 to 6 to lineWidth * 4

                for (int i = 0; i < bndList.Count; i++)
                {
                    bndList[i].fenceLineEar.DrawPolygon();
                }
            }
            else
            {
                GL.LineWidth(mf.ABLine.lineWidth * 4); // change from 3 to 4
                GL.Color3(0.031f, 0.024f, 0.847f); //change boundary color if out of boundary

                for (int i = 0; i < bndList.Count; i++)
                {
                    bndList[i].fenceLineEar.DrawPolygon();
                }
            }

            ////closest points  TooDoo
            //GL.Color3(0.70f, 0.95f, 0.95f);
            //GL.PointSize(6.0f);
            //GL.Begin(PrimitiveType.Points);
            //GL.Vertex3(mf.bnd.closestTurnPt.easting, mf.bnd.closestTurnPt.northing, 0);
            //GL.End();

            if (bndBeingMadePts.Count > 0)
            {
                //the boundary so far
                vec3 pivot = mf.pivotAxlePos;
                GL.LineWidth(mf.ABLine.lineWidth);
                GL.Color3(0.031f, 0.024f, 0.847f); //change waring color if out of boundary 0.031f, 0.024f, 0.847f
                GL.Begin(PrimitiveType.LineStrip);
                for (int h = 0; h < bndBeingMadePts.Count; h++) GL.Vertex3(bndBeingMadePts[h].easting, bndBeingMadePts[h].northing, 0);
                GL.Color3(0.295f, 0.972f, 0.290f); 
                GL.Vertex3(bndBeingMadePts[0].easting, bndBeingMadePts[0].northing, 0);
                GL.End();

                //line from last point to pivot marker
                GL.Color3(0.825f, 0.842f, 0.0f);
                GL.Enable(EnableCap.LineStipple);
                GL.LineStipple(1, 0x0700);
                GL.Begin(PrimitiveType.LineStrip);
                if (isDrawRightSide)
                {
                    GL.Vertex3(bndBeingMadePts[0].easting, bndBeingMadePts[0].northing, 0);

                    GL.Vertex3(pivot.easting + (Math.Sin(pivot.heading - glm.PIBy2) * -createBndOffset),
                            pivot.northing + (Math.Cos(pivot.heading - glm.PIBy2) * -createBndOffset), 0);
                    GL.Vertex3(bndBeingMadePts[bndBeingMadePts.Count - 1].easting, bndBeingMadePts[bndBeingMadePts.Count - 1].northing, 0);
                }
                else
                {
                    GL.Vertex3(bndBeingMadePts[0].easting, bndBeingMadePts[0].northing, 0);

                    GL.Vertex3(pivot.easting + (Math.Sin(pivot.heading - glm.PIBy2) * createBndOffset),
                            pivot.northing + (Math.Cos(pivot.heading - glm.PIBy2) * createBndOffset), 0);
                    GL.Vertex3(bndBeingMadePts[bndBeingMadePts.Count - 1].easting, bndBeingMadePts[bndBeingMadePts.Count - 1].northing, 0);
                }
                GL.End();
                GL.Disable(EnableCap.LineStipple);

                //boundary points
                GL.Color3(0.0f, 0.95f, 0.95f);
                GL.PointSize(6.0f);
                GL.Begin(PrimitiveType.Points);
                for (int h = 0; h < bndBeingMadePts.Count; h++) GL.Vertex3(bndBeingMadePts[h].easting, bndBeingMadePts[h].northing, 0);
                GL.End();
            }
        }
    }
}
