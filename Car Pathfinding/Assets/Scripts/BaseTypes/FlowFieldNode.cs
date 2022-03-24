using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public struct FlowFieldNode
{

    public Vector2Int cellPos;
    public Vector2Int cellCenter;
    public HashSet<int>[,] distFrom;

    public FlowFieldNode(Vector2Int cellPosIn, Vector2Int cellCenterIn)
    {
        cellPos = cellPosIn;
        cellCenter = cellCenterIn;
        distFrom = new HashSet<int>[(int)Parameters.worldSizeX, (int)Parameters.worldSizeY];
    }



    /* B = Bas, R = Reachable, U = Unreachable, O = Obstacle
     [F][F][F]  [F][F][U]
     [F][F][F]  [F][F][O]
     [F][F][F]  [F][F][U] 
     */

    //Generates the distance from every other cell to this cell, we don't care about cells with obstacles in them, see above for various cases
    public void generateDistances()
    {
        distFrom[cellPos[0], cellPos[1]].Add(0);

        /**
         * Loop:
         * 
         * Select node from Queue
         *  Add its neighbors (checking for corner obstacles and already in list) and set their distance
         *  
         * 
         */

    }
}
