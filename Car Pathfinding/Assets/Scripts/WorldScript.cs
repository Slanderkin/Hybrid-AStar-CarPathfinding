using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class WorldScript : MonoBehaviour
{

    float worldSizeX = Parameters.worldSizeX;
    float worldSizeY = Parameters.worldSizeY;

    private LineRenderer border;
    public EdgeCollider2D borderCollider;

    // Start is called before the first frame update
    void Start()
    {
        border = gameObject.GetComponent<LineRenderer>();
        border.positionCount = 5;
        border.startWidth = 10;
        border.endWidth = 10;
        Vector3[] borderPoints = new Vector3[] { new Vector3(0, 0), new Vector3(worldSizeX, 0), new Vector3(worldSizeX, worldSizeY), new Vector3(0, worldSizeY), new Vector3(0, -border.startWidth/2) };
        border.SetPositions(borderPoints);

        borderCollider = gameObject.GetComponent<EdgeCollider2D>();
        
        List<Vector2> colliderBorderPoints = new List<Vector2> { new Vector2(border.startWidth / 2, border.startWidth / 2), new Vector2(worldSizeX - border.startWidth / 2, border.startWidth / 2), new Vector2(worldSizeX - border.startWidth / 2, worldSizeY - border.startWidth / 2), new Vector2(border.startWidth / 2, worldSizeY - border.startWidth / 2), new Vector2(border.startWidth / 2, border.startWidth / 2) };
        borderCollider.SetPoints(colliderBorderPoints);
    }


    /*Takes the goal position and the size of the world and then computes the euclidian distance from the center of each tile to the target pos
     * 
     * If performance is an issue this could be helpful, it loses accuracy as a tradeoff in hCost
    void generateEuclidianDistanceMap()
    {

    }*/
}
