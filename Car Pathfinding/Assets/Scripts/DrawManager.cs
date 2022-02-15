using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{


    public class DrawManager : MonoBehaviour
    {

        public DrawHelper drawHelper;
        public SpriteRenderer goalRenderer;
        public SpriteRenderer currNodeRenderer;
        public SpriteRenderer carSpriteRenderer;
        public PathFinding pathFinding;
        public LineRenderer finalPath;
        private bool drawnFinal = false;

        void Start()
        {
            goalRenderer.transform.position = new Vector2(-10000,-10000);
            goalRenderer.sortingOrder = 1;
            currNodeRenderer.transform.position = new Vector2(-10000, -10000);
            currNodeRenderer.sortingOrder = 1;

            carSpriteRenderer.transform.localScale = new Vector3(Parameters.carSpriteScale, Parameters.carSpriteScale, 1);
            carSpriteRenderer.transform.position = new Vector3(Parameters.carX0, Parameters.carY0, 1);
        }

        void Update()
        {
            if (pathFinding.startedPathfinding && !pathFinding.donePathfinding)
            {
                drawCircle(pathFinding.getLatestNode().pose, false); //Takes the vector3 and casts it as a vector2
                currNodeRenderer.enabled = true;
            }
                
            else
                currNodeRenderer.enabled = false;

            if (pathFinding.goalSet)
                drawCircle(pathFinding.getGoalPose(),true);

        }

        //Takes in 2 points, the first is the starting node with Vector3 (x,y,heading). The second is the end node with Vector3 (x,y,movementDir)
        public void drawPath(Vector3 point1, Vector3 point2, Material lineMaterial)
        {

            Color red = new Color(256, 0, 0);
            Color green = new Color(0, 256, 0);

            if (point2[2] == -1)
            {
                GL.Color(red);
            }
            //Green else
            else
            {
                GL.Color(green);
            }

            GL.Vertex3(point1[0], point1[1], 0);
            GL.Vertex3(point2[0], point2[1], 0);

            //Red if reversing
            


        }

        //This will draw a circle for the goal point and for the current node (when debugging), takes the position and a bool (true is goal circle and false is curr node)
        public void drawCircle(Vector2 circlePose, bool doGoal)
        {
            if (doGoal)
                goalRenderer.transform.position = circlePose;
            else
                currNodeRenderer.transform.position = circlePose;
        }

        //Takes the final node and draws a line renderer line for it
        public void drawFinalPath(Node endNode)
        {
            if (drawnFinal)
                return;
            drawnFinal = true;
            List<Vector3> nodePositions = new List<Vector3>();

            Node currNode = endNode;
            while(currNode != null)
            {
                nodePositions.Add(new Vector3(currNode.pose.x,currNode.pose.y,-1)); // making the z -1 has it draw over the base lines
                currNode = currNode.cameFrom;
            }
            finalPath.positionCount = nodePositions.Count;
            finalPath.SetPositions(nodePositions.ToArray());
            finalPath.startColor = Color.green;
            finalPath.endColor = Color.green;
            finalPath.startWidth = 5f;
            finalPath.endWidth = 5f;
            finalPath.sortingOrder = 2;
        }

        
    }
}

