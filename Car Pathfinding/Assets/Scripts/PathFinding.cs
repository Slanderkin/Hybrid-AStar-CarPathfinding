using System.Collections;
using System.Collections.Generic;
using System.Linq; // Used to copy a list
using UnityEngine;


/*https://github.com/BlueRaja/High-Speed-Priority-Queue-for-C-Sharp
 * Code for Priority Queue from above
 */
using Priority_Queue;

namespace Pathfinding
{
    public class PathFinding : MonoBehaviour
    {

        List<Vector3> path = new List<Vector3>();


        //A list of the possible turn directions and move directions
        private List<Vector2Int> movementParameters = new List<Vector2Int> { new Vector2Int(1, -1), new Vector2Int(0, -1), new Vector2Int(-1, -1), new Vector2Int(1, 1), new Vector2Int(0, 1), new Vector2Int(-1, 1) };

        //Local versions of car parameters
        private float carSpeed = Parameters.speed;
        private float carLength = Parameters.carLength;
        private float turnAngle = Parameters.turnAngle;

        //Get the attributes of the arrow (also its game object and script)
        private float goalX;
        private float goalY;
        private float goalHeading;
        private Vector3 goalPose;
        private Vector3 prevGoalPose;

        private List<Node> nodeList;
        private Node currNode;
        private List<Node> prevNodeList;
        private Node prevCurrNode;

        private FastPriorityQueue<Node> costQueue;
        public bool startedPathfinding = false;
        public bool donePathfinding = false;
        public bool goalSet = false;

        private int pathDebuggerCurrentInd = 0;
        private const int maxIterations = 5000;
        private int iterationNum = 0;
        private const int priorityQueueMaxSize = 1000000; //Max # of Nodes allowed in the queue

        //public List<Vector4> pathViewer;

        private int worldWidth;
        private int worldHeight;

        public GameObject carCollisionDummy;
        public BoxCollider2D carCollisionDummyCollider;

        public Helper helper;
        public ArrowScript arrowScript;
        public DrawManager drawManager;
        public UIScript uiScript;
        public WorldScript worldScript;
        public Car car;

        public bool doDraw;

        public Cell[,] cellList;
      

        void Start()
        {
            worldWidth = (int)Parameters.worldSizeX;
            worldHeight = (int)Parameters.worldSizeY;

            helper = new Helper();

            costQueue = new FastPriorityQueue<Node>(priorityQueueMaxSize);

            nodeList = new List<Node>();
            prevNodeList = new List<Node>();
            prevCurrNode = new Node();

            carCollisionDummy.transform.localScale = new Vector3(30, 30, 1);
            carCollisionDummyCollider = carCollisionDummy.GetComponent<BoxCollider2D>();

            cellList = worldScript.cellList;


        }

        private void Update()
        {
            if (arrowScript.finishedPlacingGoal && !donePathfinding)
            {
                setGoal();
                if (! uiScript.getDebugToggleEnabled())
                    hybridAStar();
            }

            if (donePathfinding)
            {
                drawManager.drawFinalPath(currNode);
                resetPathfinding();
                arrowScript.resetArrowScript();
            }
        }

        //This function performs the "hybrid a*" search
        private void hybridAStar()
        {

            //Don't run this in the case when we have already started (doing the debugger)
            if (startedPathfinding)               
                return;



            drawManager.resetDrawManager();
            
            Vector3 currPose = new Vector3(car.currPose[0], car.currPose[1], car.currPose[2]);
            Vector3 goalPose = new Vector3(goalX, goalY, goalHeading * Mathf.Deg2Rad);

            startedPathfinding = true;

            currNode = new Node();
            nodeList.Add(currNode);
            currNode.pose = currPose;
            currNode.dir = 1;//set initial direction to forward to encourage more forward driving nodes
            currNode.assignHCost(goalPose, false, false);


            /*
             * 
             * 
             * 
             */
            while (iterationNum < maxIterations)
            {
                //Something went wrong or there is no path to be found
                if (costQueue.Count == 0 && iterationNum != 0 )
                {
                    Debug.Log("Exited A* due to empty queue");
                    return;
                }
                    

                for (int i = pathDebuggerCurrentInd; i < movementParameters.Count; i++)
                {
                    Node newNode = new Node();
                    nodeList.Add(newNode);
                    newNode.cameFrom = currNode;
                    move(currNode, newNode, movementParameters[i]);
                    Vector2Int discreteCarPos = helper.worldPos2Cell(newNode.pose.x, newNode.pose.y);

                    if (!willCollide(newNode.pose))
                    {

                        if (cellList[discreteCarPos.x, discreteCarPos.y].anglesContained.Add((Mathf.RoundToInt(Mathf.Rad2Deg * newNode.pose.z / 15) * 15) % 360))
                        {

                            newNode.assignHCost(goalPose, newNode.dir == 1 ? false : true, newNode.cameFrom.steerAngle == newNode.steerAngle ? false : true);

                            if (costQueue.Count == priorityQueueMaxSize)
                            {
                                donePathfinding = true;
                                return;
                            }
                            costQueue.Enqueue(newNode, newNode.fCost);
                            currNode.isTopNode = false;
                            currNode.numChildren++;
                            currNode.childPoses.Add(new Vector3(newNode.pose.x, newNode.pose.y, movementParameters[i][1]));
                            if (closeEnough(newNode, goalPose))
                            {
                                donePathfinding = true;
                                currNode = newNode;
                                car.updatePose(currNode.pose);
                                drawManager.pathfindingCopy = nodeList.ToList();
                                drawManager.updateCarSpritePoses();
                                return;
                            }
                        }
                    }
                }
                iterationNum++;
                currNode = costQueue.Dequeue();
            }
            
            donePathfinding = true;
            car.updatePose(currNode.pose);
            drawManager.pathfindingCopy = nodeList.ToList();
            drawManager.updateCarSpritePoses();
        }

        //====================================
        //         Misc Helper Funcs
        //====================================

        //Basic car motion that takes a turn direction (1 left, 0 straight, -1 right) and a drive direction (1 fwd, -1 back)
        //Returns whether or not the movement is valid
        private void move(Node currNode, Node newNode,Vector2Int movementParameters)
        {

            Vector3 currPose = currNode.pose;
            int turnDir = movementParameters[0];
            int driveDir = movementParameters[1];
            float currX = currPose[0];
            float currY = currPose[1];
            float currHeading = currPose[2];

            //The distance the car move (with sign representing forward/backward)
            float distMoved;
            //The angle at which the car is steering (the turnAngle with a sign [+ CCW] and in radians)
            float steeringAngle;

            Vector2 pos0 = new Vector2(currX, currY);


            steeringAngle = turnAngle * turnDir * Mathf.Deg2Rad;
            distMoved = carSpeed*driveDir;

            float beta = distMoved / carLength * Mathf.Tan(steeringAngle);

            float cx;
            float cy;

            float distanceMoved;

            if (turnDir != 0)
            {
                float R = distMoved / beta;
                cx = currX - Mathf.Sin(currHeading) * R;
                cy = currY + Mathf.Cos(currHeading) * R;
                currX = cx + Mathf.Sin(currHeading + beta) * R;
                currY = cy - Mathf.Cos(currHeading + beta) * R;
                currHeading = (currHeading + beta + 2 * Mathf.PI) % (2 * Mathf.PI); //Adding 2 pi makes sure this value is never negative
                distanceMoved = Mathf.Abs((pos0- new Vector2(currX, currY)).magnitude); //Need to take abs otherwise it can move a negative distance (and require lots of debugging to find this issue :) )
            }
            else
            {
                currX += distMoved * Mathf.Cos(currHeading);
                currY += distMoved * Mathf.Sin(currHeading);
                distanceMoved = Mathf.Abs(distMoved);
            }

            newNode.updateValues(new Vector3(currX, currY, currHeading), distanceMoved, driveDir, turnAngle*turnDir);

            //The gCost will be penalized if the car is moving backwards or changes driving direction
            newNode.assignGCost(currNode.dir, distanceMoved);
            
            

        }

        //Determine if the current node is within error of the goal pose
        private bool closeEnough(Node currNode,Vector3 goalpose)
        {
            return (currNode.hCost < 20 && Mathf.Abs(currNode.pose[2] - goalPose[2]) < 10*Mathf.Deg2Rad );
        }

        //Sets the goal pose once it has been determined from placing the arrow
        private void setGoal()
        {
            //Update the goal pose now that the arrow has been set correctly
            goalX = arrowScript.transform.position.x;
            goalY = arrowScript.transform.position.y;
            goalHeading = arrowScript.transform.GetChild(0).eulerAngles[2];
            goalPose = new Vector3(goalX, goalY, goalHeading * Mathf.Deg2Rad);
            goalSet = true;
        }

        //Resets the pathfinding values to their base values
        public void resetPathfinding()
        {
            startedPathfinding = false;
            donePathfinding = false;
            goalSet = false;
            iterationNum = 0;


            prevGoalPose = goalPose;
            prevCurrNode.copy(currNode);
            prevNodeList = nodeList.ToList();

            costQueue.Clear();
            currNode = null;
            nodeList.Clear();
            cleancellList();
        }

        //Empties the hash table that stores the various angles that we have observed at each x,y location, this is basically 3D A* capturing 2D motion: x,y,theta
        private void cleancellList()
        {
            for (int i = 0; i < worldWidth; i++)
            {
                for (int j = 0; j < worldHeight; j++)
                {
                    cellList[i, j].anglesContained.Clear();
                }
            }
        }

        //Detects if being in the proposed pose would cause a collision with an obstacle or the "world border" (This is not efficient)
        public bool willCollide(Vector3 collisionGhostPose)
        {
            carCollisionDummyCollider.transform.rotation = Quaternion.Euler(0, 0, collisionGhostPose[2] * Mathf.Rad2Deg);
            carCollisionDummyCollider.attachedRigidbody.position = (Vector2)collisionGhostPose;
            carCollisionDummyCollider.attachedRigidbody.transform.position = (Vector2)collisionGhostPose;


            //Detect if this new pose would cause a wall coliision
            //if (maxPoints[0] > Parameters.worldSizeX || maxPoints[1] > Parameters.worldSizeY || minPoints[0] < 0 || minPoints[1] < 0)
            //return true;
            if (carCollisionDummyCollider.Distance(arrowScript.worldScript.borderCollider).isOverlapped)
                return true;

            //Loop through all of the obstacle colliders
            //Do .distance from the ghost to each obstacle
            //If any is overlapped, return as a collision occured and this is not a valid pose
            foreach (Obstacle obstacle in arrowScript.worldScript.obstacleList)
            {
                if (carCollisionDummyCollider.Distance(obstacle.obstacleCollider).isOverlapped)
                {
                    return true;
                }
            }

            return false;
        }

        //====================================
        //       Misc Getter Functions
        //====================================

        //Return the goal pose of the previous run
        public Vector3 getPrevGoalPose()
        {
            return prevGoalPose;
        }

        //return iteration number
        public int getIerationNum()
        {
            return iterationNum;
        }

        //Returns the final node in the last A* sequence ran
        public Node getPrevNode()
        {
            if (prevCurrNode != null)
                return prevCurrNode;
            return null;
        }

        //Returns the node most recently used
        public Node getLatestNode()
        {
            if (currNode != null)
                return currNode;
            return null;
        }

        public List<Node> getPrevNodeList()
        {
            if (prevNodeList != null)
                return prevNodeList;
            return null;
        }

        //====================================
        // Public Functions for Path Debugger
        //====================================

        public void simNextNode()
        {
            if (!startedPathfinding && arrowScript.finishedPlacingGoal)
            {
                startedPathfinding = true;

                Vector3 currPose = new Vector3(Parameters.carX0, Parameters.carY0, 0);

                currNode = new Node();
                nodeList.Add(currNode);
                currNode.pose = currPose;
                currNode.dir = 1;//set initial direction to forward to encourage more forward driving nodes

                currNode.assignHCost(goalPose, false, false);


                for (int i = pathDebuggerCurrentInd; i < movementParameters.Count; i++)
                {
                    Node newNode = new Node();
                    nodeList.Add(newNode);
                    newNode.cameFrom = currNode;
                    move(currNode, newNode, movementParameters[i]);
                    Vector2Int discreteCarPos = helper.worldPos2Cell(newNode.pose.x, newNode.pose.y);

                    if (!willCollide(newNode.pose))
                    {
                        
                        if (cellList[discreteCarPos.x, discreteCarPos.y].anglesContained.Add((Mathf.RoundToInt(Mathf.Rad2Deg * newNode.pose.z / 15) * 15) % 360))
                        {

                            newNode.assignHCost(goalPose, newNode.dir == 1 ? false : true, newNode.cameFrom.steerAngle == newNode.steerAngle ? false : true);
                            
                            if (costQueue.Count == priorityQueueMaxSize)
                            {
                                donePathfinding = true;
                                return;
                            }
                            costQueue.Enqueue(newNode, newNode.fCost);
                            currNode.isTopNode = false;
                            currNode.numChildren++;
                            currNode.childPoses.Add(new Vector3(newNode.pose.x, newNode.pose.y,movementParameters[i][1]));
                            if (closeEnough(newNode,goalPose))
                            {
                                donePathfinding = true;
                                currNode = newNode;
                                car.updatePose(currNode.pose);
                                return;
                            }
                        }
                    }
                }
                iterationNum++;
                pathDebuggerCurrentInd = 0;
                currNode = costQueue.Dequeue();

            }
            else if (startedPathfinding && !donePathfinding)
            {
                for (int i = pathDebuggerCurrentInd; i < movementParameters.Count; i++)
                {
                    Node newNode = new Node();
                    nodeList.Add(newNode);
                    newNode.cameFrom = currNode;
                    move(currNode, newNode, movementParameters[i]);
                    Vector2Int discreteCarPos = helper.worldPos2Cell(newNode.pose.x, newNode.pose.y);

                    if (!willCollide(newNode.pose))
                    {
                        
                        if (cellList[discreteCarPos.x, discreteCarPos.y].anglesContained.Add((Mathf.RoundToInt(Mathf.Rad2Deg * newNode.pose.z / 15) * 15) % 360))
                        {

                            newNode.assignHCost(goalPose, newNode.dir == 1 ? false : true, newNode.cameFrom.steerAngle == newNode.steerAngle ? false : true);
                            
                            if (costQueue.Count == priorityQueueMaxSize)
                            {
                                donePathfinding = true;
                                return;
                            }
                            costQueue.Enqueue(newNode, newNode.fCost);
                            currNode.isTopNode = false;
                            currNode.numChildren++;
                            currNode.childPoses.Add(new Vector3(newNode.pose.x, newNode.pose.y, movementParameters[i][1]));
                            if (closeEnough(newNode,goalPose))
                            {
                                donePathfinding = true;
                                currNode = newNode;
                                car.updatePose(currNode.pose);
                                return;
                            }
                        }
                    }
                }
                iterationNum++;
                pathDebuggerCurrentInd = 0;
                currNode = costQueue.Dequeue();
            }
            else if (donePathfinding)
            {
                return;
            }


        }

        public void simNextPath()
        {
            if (!startedPathfinding && arrowScript.finishedPlacingGoal)
            {

                startedPathfinding = true;

                Vector3 currPose = new Vector3(Parameters.carX0, Parameters.carY0,0);

                currNode = new Node();
                nodeList.Add(currNode);
                currNode.pose = currPose;
                currNode.dir = 1;//set initial direction to forward to encourage more forward driving nodes

                currNode.assignHCost(goalPose, false, false);

                Node newNode = new Node();
                nodeList.Add(newNode);
                newNode.cameFrom = currNode;

                move(currNode, newNode, movementParameters[pathDebuggerCurrentInd]);
                Vector2Int discreteCarPos = helper.worldPos2Cell(newNode.pose.x, newNode.pose.y);

                if (!willCollide(newNode.pose))
                {
                    
                    if (cellList[discreteCarPos.x, discreteCarPos.y].anglesContained.Add((Mathf.RoundToInt(Mathf.Rad2Deg * newNode.pose.z / 15) * 15) % 360))
                    {

                        newNode.assignHCost(goalPose, newNode.dir == 1 ? false : true, newNode.cameFrom.steerAngle == newNode.steerAngle ? false : true);
                        
                        if (costQueue.Count == priorityQueueMaxSize)
                        {
                            donePathfinding = true;
                            return;
                        }
                            
                        costQueue.Enqueue(newNode, newNode.fCost);
                        currNode.isTopNode = false;
                        currNode.numChildren++;
                        currNode.childPoses.Add(new Vector3(newNode.pose.x, newNode.pose.y, movementParameters[pathDebuggerCurrentInd][1]));
                        if (closeEnough(newNode,goalPose))
                        {
                            donePathfinding = true;
                            currNode = newNode;
                            car.updatePose(currNode.pose);
                            return;
                        }
                    }
                }
                pathDebuggerCurrentInd = (1+ pathDebuggerCurrentInd )% movementParameters.Count;
                if (pathDebuggerCurrentInd == 0)
                {
                    currNode = costQueue.Dequeue();
                    iterationNum++;
                }


            }
            else if (startedPathfinding && !donePathfinding)
            {
                Node newNode = new Node();
                nodeList.Add(newNode);
                newNode.cameFrom = currNode;
                move(currNode, newNode, movementParameters[pathDebuggerCurrentInd]);
                Vector2Int discreteCarPos = helper.worldPos2Cell(newNode.pose.x, newNode.pose.y);

                if (!willCollide(newNode.pose))
                {
                    
                    if (cellList[discreteCarPos.x, discreteCarPos.y].anglesContained.Add((Mathf.RoundToInt(Mathf.Rad2Deg * newNode.pose.z / 15) * 15) % 360))
                    {

                        newNode.assignHCost(goalPose, newNode.dir == 1 ? false : true, newNode.cameFrom.steerAngle == newNode.steerAngle ? false : true);
                       
                        if (costQueue.Count == priorityQueueMaxSize)
                        {
                            donePathfinding = true;
                            return;
                        }

                        costQueue.Enqueue(newNode, newNode.fCost);
                        currNode.isTopNode = false;
                        currNode.numChildren++;
                        currNode.childPoses.Add(new Vector3(newNode.pose.x, newNode.pose.y, movementParameters[pathDebuggerCurrentInd][1]));
                        if (closeEnough(newNode,goalPose))
                        {
                            donePathfinding = true;
                            currNode = newNode;
                            car.updatePose(currNode.pose);
                            return;
                        }
                    }
                }
                pathDebuggerCurrentInd = (1 + pathDebuggerCurrentInd) % movementParameters.Count;
                if (pathDebuggerCurrentInd == 0)
                {
                    currNode = costQueue.Dequeue();
                    iterationNum++;
                }
                    
            }
            else if (donePathfinding)
            {
                return;
            }
        }

        
    }
}

