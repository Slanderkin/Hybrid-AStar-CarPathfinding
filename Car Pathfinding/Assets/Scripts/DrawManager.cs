using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{


    public class DrawManager : MonoBehaviour
    {

        public SpriteRenderer goalRenderer;
        public SpriteRenderer currNodeRenderer;

        private SpriteRenderer carSpriteRenderer;
        private SpriteRenderer ghostSpriteRenderer;
        private SpriteRenderer arrowSpriteRenderer;
        private LineRenderer arrowCarColliderHitbox;
        

        public LineRenderer finalPath;
        public Material lineMaterial;

        public DrawHelper drawHelper;
        public PathFinding pathFinding;
        public WorldScript worldScript;
        public Helper helper;
        public Car car;
        public ArrowScript arrow;

        private bool drawnFinal = false;
        public bool doDrawPathfindingTree = true;

        public GameObject nodeCirclePrefab;
        public GameObject lineRendererPrefab;
        public GameObject obstaclePrefab;
        public GameObject arrowCarColliderObj;

        private List<GameObject> nodeCircleList;
        private List<GameObject> hitboxLineRendererList;

        public List<Node> pathfindingCopy; //This exists so that when the pathfinding is reset to prepare for the next run that we can still draw the pathfinding "tree"

        void Start()
        {
            helper = new Helper();

            goalRenderer.transform.position = new Vector2(-10000,-10000);
            goalRenderer.sortingOrder = 1;
            currNodeRenderer.transform.position = new Vector2(-10000, -10000);
            currNodeRenderer.sortingOrder = 1;


            carSpriteRenderer = car.carSprite;
            ghostSpriteRenderer = car.ghostSprite;
            arrowSpriteRenderer = arrow.GetComponentInChildren<SpriteRenderer>();

            carSpriteRenderer.transform.localScale = new Vector3(car.carSpriteScale, car.carSpriteScale, 1);
            carSpriteRenderer.transform.position = new Vector3(car.currPose[0], car.currPose[1], 1);

            ghostSpriteRenderer.transform.localScale = new Vector3(car.carSpriteScale, car.carSpriteScale, 1);
            ghostSpriteRenderer.transform.position = new Vector3(car.currPose[0], car.currPose[1], 1);
            ghostSpriteRenderer.enabled = false;
            Color tmpColor = ghostSpriteRenderer.color;
            tmpColor.a = 0.5f;
            ghostSpriteRenderer.color = tmpColor;

            arrowSpriteRenderer.transform.localScale = new Vector3(22.72f, 21.7f, 1);
            arrowSpriteRenderer.color = new Color(arrowSpriteRenderer.color.r, arrowSpriteRenderer.color.g, arrowSpriteRenderer.color.b, 0.5f); //Can't just change the color.a field :(

            arrowCarColliderHitbox = arrowCarColliderObj.GetComponent<LineRenderer>();
            setupArrowCarColliderHitbox();

            nodeCircleList = new List<GameObject>();
            hitboxLineRendererList = new List<GameObject>();

            pathfindingCopy = new List<Node>();

            setupObstacles();

            
        }

        void Update()
        {
            if (pathFinding.startedPathfinding && !pathFinding.donePathfinding)
            {
                drawCircle(pathFinding.getPrevNode().pose, false); //Takes the vector3 and casts it as a vector2
                currNodeRenderer.enabled = true;
            }   
            else
                currNodeRenderer.enabled = false;

            if (pathfindingCopy.Count != 0)
                drawCircle(pathFinding.getPrevGoalPose(),true);
            
            updatePlacerArrowOpacity();
            updateArrowCarColliderHitbox();

        }


        //Instantiate a copy of the obstacle prefab to draw an obstacle set up by the world script
        public void setupObstacles()
        {
            foreach (Obstacle obstacle in worldScript.obstacleList)
            {
                GameObject tmpObj = Instantiate(obstaclePrefab); 
                LineRenderer tmpLR = tmpObj.GetComponent<LineRenderer>();
                
                tmpLR.positionCount = 2;
                tmpLR.startWidth = obstacle.width;
                tmpLR.endWidth = obstacle.width;
                tmpLR.startColor = Color.gray;
                tmpLR.endColor = Color.gray;
                tmpLR.allowOcclusionWhenDynamic = false;

                tmpLR.SetPosition(0, obstacle.startLRPoint);
                tmpLR.SetPosition(1, obstacle.endLRPoint);

                obstacle.obstacleGameobject = tmpObj;

                helper.setUpObstacleCollider(obstacle);
            }
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

            List<Vector3> rectPoints = new List<Vector3>() { new Vector3(car.carLength / 2, car.carWidth / 2, -1), new Vector3(car.carLength / 2, -car.carWidth / 2, -1), new Vector3(-car.carLength / 2, -car.carWidth / 2, -1), new Vector3(-car.carLength / 2, car.carWidth / 2, -1) };

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
        }
    
        //Takes the new car pose and updates the actual and ghost sprites (called after car pose is updated)
        public void updateCarSpritePoses()
        {
            Vector3 newPose = car.currPose;
            ghostSpriteRenderer.enabled = true;
            ghostSpriteRenderer.transform.position = carSpriteRenderer.transform.position;
            ghostSpriteRenderer.transform.rotation = carSpriteRenderer.transform.rotation;

            carSpriteRenderer.transform.position = new Vector3(newPose[0], newPose[1], 1);
            carSpriteRenderer.transform.rotation = Quaternion.Euler(0, 0, newPose[2] * Mathf.Rad2Deg);

        }

        //Handles all drawing of the "placer arrow"
        private void updatePlacerArrowOpacity()
        {
            if(arrow.startedPlacingGoal && !arrow.finishedPlacingGoal)
            {
                arrowSpriteRenderer.color = new Color(arrowSpriteRenderer.color.r, arrowSpriteRenderer.color.g, arrowSpriteRenderer.color.b, 1f);
            }
            else
            {
                arrowSpriteRenderer.color = new Color(arrowSpriteRenderer.color.r, arrowSpriteRenderer.color.g, arrowSpriteRenderer.color.b, 0.5f);
            }
        }

        //This sets up a visual aid when placing the arrow, it will show the car's future hitbox allowing the user to see when they will be placing the 
        //car into a collision on start
        private void setupArrowCarColliderHitbox()
        {
            List<Vector3> rectPoints = new List<Vector3>() { new Vector3(car.carLength / 2, car.carWidth / 2, -1), new Vector3(car.carLength / 2, -car.carWidth / 2, -1), new Vector3(-car.carLength / 2, -car.carWidth / 2, -1), new Vector3(-car.carLength / 2, car.carWidth / 2, -1) };

            arrowCarColliderHitbox.useWorldSpace = false;
            arrowCarColliderHitbox.positionCount = rectPoints.Count;
            arrowCarColliderHitbox.startWidth = 5;
            arrowCarColliderHitbox.endWidth = 5;

            for (int i = 0; i < rectPoints.Count; i++)
            {
                arrowCarColliderHitbox.SetPosition(i, (rectPoints[i]));
            }
            arrowCarColliderHitbox.loop = true;
        }

        //Updates the Parents GameObj's rotation to match that of the arrow
        private void updateArrowCarColliderHitbox()
        {
            arrowCarColliderHitbox.enabled = true;
            arrowCarColliderObj.transform.position = arrow.transform.position;
            arrowCarColliderObj.transform.rotation = Quaternion.Euler(0, 0, arrow.child.transform.rotation.eulerAngles[2]);
            if (arrow.willCollide(arrow.pose))
            {
                arrowCarColliderHitbox.startColor = Color.red;
                arrowCarColliderHitbox.endColor = Color.red;
            }
            else
            {
                arrowCarColliderHitbox.startColor = Color.white;
                arrowCarColliderHitbox.endColor = Color.white;
            }

                

            
        }

        //Draws lines using the DrawManager
        private void OnRenderObject()
        {
            if (pathfindingCopy.Count != 0 && doDrawPathfindingTree)
            {
                if (pathfindingCopy.Count > 0)
                {
                    lineMaterial.SetPass(0);
                    GL.PushMatrix();
                    GL.Begin(GL.LINES);

                    for (int i = 0; i < pathfindingCopy.Count; i++)
                    {
                        for (int j = 0; j < pathfindingCopy[i].numChildren; j++)
                        {
                            drawPath(pathfindingCopy[i].pose, pathfindingCopy[i].childPoses[j], lineMaterial);
                        }
                    }


                    GL.End();
                    GL.PopMatrix();
                }
            }

        }

    }
}

