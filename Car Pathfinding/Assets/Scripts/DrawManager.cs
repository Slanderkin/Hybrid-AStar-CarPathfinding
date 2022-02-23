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
        public Material lineMaterial;

        private bool drawnFinal = false;
        public bool doDrawPathfinding = true;

        public GameObject nodeCirclePrefab;
        public GameObject lineRendererPrefab;

        private List<GameObject> nodeCircleList;
        private List<GameObject> hitboxLineRendererList;

        void Start()
        {
            goalRenderer.transform.position = new Vector2(-10000,-10000);
            goalRenderer.sortingOrder = 1;
            currNodeRenderer.transform.position = new Vector2(-10000, -10000);
            currNodeRenderer.sortingOrder = 1;

            carSpriteRenderer.transform.localScale = new Vector3(Parameters.carSpriteScale, Parameters.carSpriteScale, 1);
            carSpriteRenderer.transform.position = new Vector3(Parameters.carX0, Parameters.carY0, 1);

            nodeCircleList = new List<GameObject>();
            hitboxLineRendererList = new List<GameObject>();
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

            Vector3 tempPose;
            Node currNode = endNode;
            int counter = 0;
            while(currNode != null)
            {
                tempPose = new Vector3(currNode.pose.x, currNode.pose.y, -1);
                nodePositions.Add(tempPose); // making the z -1 has it draw over the base lines
                if(counter != 0) { }
                    drawFinalPathNode(tempPose,counter);
                //drawCarHitbox(currNode.pose); This doesn't seem to add much information. More distracting than anything
                currNode = currNode.cameFrom;
                counter++;
            }
            finalPath.positionCount = nodePositions.Count;
            finalPath.SetPositions(nodePositions.ToArray());
            finalPath.startColor = Color.green;
            finalPath.endColor = Color.green;
            finalPath.startWidth = 2f;
            finalPath.endWidth = 2f;
            finalPath.sortingOrder = 2;
        }

        //Places a circle at the given point in the node path
        public void drawFinalPathNode(Vector3 nodeLocation, int num)
        {
            GameObject tempObj = Instantiate(nodeCirclePrefab);
            tempObj.transform.position = nodeLocation;
            tempObj.name = "Node Circle: " + num.ToString();
            nodeCircleList.Add(tempObj);

        }

        //Draws a rectangle at each node on the path to simulate the car's hitbox
        public void drawCarHitbox(Vector3 nodePose)
        {
            Matrix4x4 rotationMatrix = Matrix4x4.Rotate(Quaternion.Euler(0, 0, nodePose[2]*180/Mathf.PI));
            GameObject tmpObj =  Instantiate(lineRendererPrefab);
            tmpObj.transform.position = (Vector2)nodePose;
            tmpObj.transform.rotation = Quaternion.Euler(0, 0, nodePose[2] * 180 / Mathf.PI);
            LineRenderer tmpLR = tmpObj.GetComponent<LineRenderer>();

            tmpLR.useWorldSpace = false;

            //List<Vector3> rectPoints = new List<Vector3>() {new Vector3(nodePose.x + Parameters.carLength/2, nodePose.y + Parameters.carWidth / 2,-1), new Vector3(nodePose.x + Parameters.carLength / 2, nodePose.y - Parameters.carWidth / 2, -1), new Vector3(nodePose.x - Parameters.carLength / 2, nodePose.y - Parameters.carWidth / 2, -1), new Vector3(nodePose.x - Parameters.carLength / 2, nodePose.y + Parameters.carWidth / 2, -1) };
            List<Vector3> rectPoints = new List<Vector3>() { new Vector3(Parameters.carLength / 2,Parameters.carWidth / 2, -1), new Vector3(Parameters.carLength / 2, -Parameters.carWidth / 2, -1), new Vector3(-Parameters.carLength / 2, -Parameters.carWidth / 2, -1), new Vector3(-Parameters.carLength / 2, Parameters.carWidth / 2, -1) };

            tmpLR.positionCount = rectPoints.Count + 1;

            for (int i = 0; i < rectPoints.Count; i++)
            {
                tmpLR.SetPosition(i, rotationMatrix.MultiplyPoint3x4(rectPoints[i]));
            }
            tmpLR.SetPosition(rectPoints.Count, rotationMatrix.MultiplyPoint3x4(rectPoints[0]));

            hitboxLineRendererList.Add(tmpObj);

        }

        //Deletes and sprites and line renderers instantiated
        public void resetDrawManager()
        {
            //Need two different loops because the length of these loops won't always be the same (
            foreach (GameObject lineRendererObj in hitboxLineRendererList)
            {
                Destroy(lineRendererObj);
            }
            foreach (GameObject nodeCircle in nodeCircleList)
            {
                Destroy(nodeCircle);
            }
            finalPath.positionCount = 0;
            goalRenderer.transform.position = new Vector2(-10000, -10000);
            drawnFinal = false;
            doDrawPathfinding = true;
        }
    

        //Draws lines using the DrawManager
        void OnRenderObject()
        {
            List<Node> latestNodeList = pathFinding.getNodeList();
            if (pathFinding.startedPathfinding && latestNodeList != null && doDrawPathfinding)
            {
                if (latestNodeList.Count > 0)
                {
                    lineMaterial.SetPass(0);
                    GL.PushMatrix();
                    GL.Begin(GL.LINES);

                    for (int i = 0; i < latestNodeList.Count; i++)
                    {
                        for (int j = 0; j < latestNodeList[i].numChildren; j++)
                        {
                            drawPath(latestNodeList[i].pose, latestNodeList[i].childPoses[j], lineMaterial);
                        }
                    }


                    GL.End();
                    GL.PopMatrix();
                }
            }

        }

    }
}

