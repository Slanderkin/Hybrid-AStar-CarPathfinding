using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;


public struct Cell
{
   
    public bool containsObstacle;
    public Vector2Int cellPos;
    public Vector2Int cellCenter;
    public HashSet<int> anglesContained;
    public FlowFieldNode flowFieldNode;

    public Cell(Vector2Int cellPosIn)
    {
        containsObstacle = false;
        cellPos = cellPosIn;
        cellCenter = cellPosIn + new Vector2Int(Parameters.cellWidth / 2, Parameters.cellWidth / 2);
        flowFieldNode = new FlowFieldNode();
        anglesContained = new HashSet<int>();
    }
    
}
