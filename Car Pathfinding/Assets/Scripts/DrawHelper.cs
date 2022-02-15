using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class DrawHelper
    {


        public bool doDraw;


        //Takes in 2 points, the first is the starting node with Vector3 (x,y,heading). The second is the end node with Vector3 (x,y,movementDir)
        public void drawPath(Vector3 point1, Vector3 point2, Material lineMaterial)
        {

            Color red = new Color(256, 0, 0);
            Color green = new Color(0, 256, 0);
            //Loop over all nodes

            GL.Vertex3(point1[0], point1[1], 0);
            GL.Vertex3(point2[0], point2[1], 0);

            //Red if reversing
            if (point2[2] == -1)
            {
                GL.Color(red);
            }
            //Green else
            else
            {
                GL.Color(green);
            }


        }

        //This will draw a circle for the goal point and for the current node (when debugging)
        public void drawCircle(Vector2 circlePose)
        {

        }
    }
}



