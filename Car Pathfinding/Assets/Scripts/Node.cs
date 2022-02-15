using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

namespace Pathfinding
{
    //Node used for pathfinding
    public class Node : FastPriorityQueueNode
    {
        public Node cameFrom;
        public Vector3 pose;
        public List<Vector3> childPoses;
        public int numChildren;
        public int dir;
        public float steerAngle;
        public float gCost; //Cost of movement made so far (penalized for reverse movement)
        public float hCost; //Cost of movement to be made (distance from goal)
        public float distMoved;
        public bool isTopNode; 

        public Node()
        {
            cameFrom = null;
            dir = 0;
            steerAngle = 0;
            gCost = -1; //Cost of movement made so far (distance travelled)
            hCost = -1; //Cost of movement to be made (distance from goal)
            distMoved = 0;
            isTopNode = true;
            numChildren = 0;
            childPoses = new List<Vector3>(6); //6 is the max num of children
        }

        //Assigns this node's h cost
        public void assignHCost(Vector3 goalPose, bool isBackwards, bool changeSteer)
        {
            Vector3 pose1 = this.pose;
            float euclidianDist = Mathf.Sqrt(Mathf.Pow(pose1[0] - goalPose[0], 2) + Mathf.Pow(pose1[1] - goalPose[1], 2) + Mathf.Pow(pose1[2] - goalPose[2], 2));

            this.hCost = euclidianDist;
        }

        //Assigns this node's g cost
        public void assignGCost(int previousDriveDir, float distanceMoved)
        {
            //Prefer to not drive in reverse
            float reversePenalty = this.dir == -1f ? .5f : 0f;

            //If the car changes driving direction, penalize it
            float jerkPenalty = this.dir == this.cameFrom.dir ? 0 : 2f;

            this.gCost = distanceMoved * (1f + reversePenalty) + this.cameFrom.gCost + jerkPenalty;
        }

        //Updates misc values of this node like its driving direction ..................
        public void updateValues(Vector3 newPose, float distanceMoved, int driveDir, float steerAngle)
        {
            this.pose = newPose;
            this.distMoved = this.cameFrom.distMoved + distanceMoved;
            this.dir = driveDir;
            this.steerAngle = steerAngle;
        }

        //Getter
        public float fCost
        {
            get { return hCost + gCost; }
        }

    }

    
}

