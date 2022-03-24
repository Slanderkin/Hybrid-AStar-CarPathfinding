using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle
{
    public Vector2 center;
    public float length;
    public float width;
    public float theta;
    public Vector3 startLRPoint;
    public Vector3 endLRPoint;

    public GameObject obstacleGameobject;
    public BoxCollider2D obstacleCollider;

    public Obstacle(Vector2 centerIn, float lengthIn, float widthIn, float thetaIn)
    {
        center = centerIn;
        length = lengthIn;
        width = widthIn;
        theta = thetaIn;
        generateLineRendererPoints();
    }

    //Takes in the relevant info of an obstacle and then returns the start and end point for an obstacle Vector4 -> (x1,y1,x2,y2)
    private void generateLineRendererPoints()
    {
        float x1 = center[0] - length/2 * Mathf.Cos(theta);
        float y1 = center[1] - length/2 * Mathf.Sin(theta);

        startLRPoint = new Vector2(x1, y1);

        float x2 = center[0] + length/2 * Mathf.Cos(theta);
        float y2 = center[1] + length/2 * Mathf.Sin(theta);

        endLRPoint = new Vector2(x2, y2);
    }

}
