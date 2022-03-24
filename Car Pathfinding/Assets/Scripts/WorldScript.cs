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

    public List<Obstacle> obstacleList; //Contains the list of obstacles

    public Cell[,] cellList; 

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

        obstacleList = new List<Obstacle>();

        generateObstacles();

        cellList = new Cell[(int)worldSizeX, (int)worldSizeY];

        for (int i = 0; i < worldSizeX; i++)
        {
            for (int j = 0; j < worldSizeY; j++)
            {
                cellList[i, j] = new Cell(new Vector2Int(i,j));
            }
        }
    }

    //Populate the obstacle list
    private void generateObstacles()
    {
        obstacleList.Add(new Obstacle(new Vector3(950,750,0),700,200, 90 * Mathf.Deg2Rad));
        obstacleList.Add(new Obstacle(new Vector3(2500, 900, 0), 600, 100, 0));
     
    }

}
