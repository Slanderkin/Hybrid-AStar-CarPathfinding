using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Pathfinding
{

    public class ArrowScript : MonoBehaviour
    {
        public GameObject child;
        private Vector3 startPoint;
        private Vector3 endPoint;
        private Vector3 directionVector;
        private float vectorAngle;
        public Vector3 pose;

        private float arrowOffset = 0.2f;
        public bool startedPlacingGoal = false;
        public bool finishedPlacingGoal = false;

        public float arrowScale = 23f;

        private float width;
        private float height;

        private Helper helper;

        public PathFinding pathFinding;
        public WorldScript worldScript;

        public GameObject arrowCollisionDummy;
        public BoxCollider2D arrowCollisionDummyCollider;

        // Start is called before the first frame update
        void Start()
        {
            helper = new Helper();


            transform.position = new Vector3(0, 0, 0);
            child.transform.position = new Vector3(0, 0, -100);

            width = Parameters.worldSizeX;
            height = Parameters.worldSizeY;

            arrowCollisionDummy.transform.localScale = new Vector3(30, 30, 1);
        }

        // Update is called once per frame
        void Update()
        {
            if (!startedPlacingGoal)
                preSelectHover();
            if (!helper.isMouseOverUi())
                checkMouse();
            pose = new Vector3(transform.position.x, transform.position.y, vectorAngle);
        }

        //Checks if the mouse has been set
        private void checkMouse()
        {
            //0 L, 1 R, 2 M
            if (Input.GetMouseButton(0) && !finishedPlacingGoal)
            {
                if (!startedPlacingGoal)
                {

                    startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    startPoint[2] = 0.0f;
                    if (startPoint[0] < width && startPoint[0] >= 0 && startPoint[1] < height && startPoint[1] >= 0)
                    {
                        transform.position = startPoint;
                        startedPlacingGoal = true;

                    }

                }
                else if (startedPlacingGoal && !finishedPlacingGoal)
                {
                    
                    endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    directionVector = endPoint - startPoint;
                    directionVector = directionVector.normalized;
                    vectorAngle = Mathf.Atan2(directionVector[1], directionVector[0]);
                    child.transform.rotation = Quaternion.Euler(0, 0, vectorAngle*Mathf.Rad2Deg);
                    child.transform.localPosition = new Vector3(Mathf.Sqrt(arrowOffset) * Mathf.Cos(vectorAngle), Mathf.Sqrt(arrowOffset) * Mathf.Sin(vectorAngle), 0);

                }

            }
            else if (!Input.GetMouseButton(0) && startedPlacingGoal && !finishedPlacingGoal)
            {
                finishedPlacingGoal = true;
                directionVector = endPoint - startPoint;
                directionVector = directionVector.normalized;
                vectorAngle = Mathf.Atan2(directionVector[1], directionVector[0]);
                child.transform.rotation = Quaternion.Euler(0, 0, vectorAngle * Mathf.Rad2Deg);
                child.transform.localPosition = new Vector3(Mathf.Sqrt(arrowOffset) * Mathf.Cos(vectorAngle), Mathf.Sqrt(arrowOffset) * Mathf.Sin(vectorAngle), 0);

                if (willCollide(new Vector3(transform.position[0], transform.position[1], vectorAngle)))
                    resetArrowScript();
            }

        }

        //Before a final position is selected have the arrow follow the mouse cursor
        private void preSelectHover()
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos[2] = 0;//Make sure the arrow is not set into -z making it not visible
            transform.localPosition = mousePos;
            child.transform.localPosition = new Vector3(0, 0, 0);
        }


        public void resetArrowScript()
        {
            startedPlacingGoal = false;
            finishedPlacingGoal = false;
        }

        
        //Detects if being in the proposed pose would cause a collision with an obstacle or the "world border" (This is not efficient)
        public bool willCollide(Vector3 arrowPose)
        {
            arrowCollisionDummyCollider.transform.rotation = Quaternion.Euler(0, 0, arrowPose[2] * Mathf.Rad2Deg);
            arrowCollisionDummyCollider.attachedRigidbody.position = (Vector2)arrowPose;
            arrowCollisionDummyCollider.attachedRigidbody.transform.position = (Vector2)arrowPose;


            //Detect if this new pose would cause a wall coliision
            //if (maxPoints[0] > Parameters.worldSizeX || maxPoints[1] > Parameters.worldSizeY || minPoints[0] < 0 || minPoints[1] < 0)
            //return true;
            if (arrowCollisionDummyCollider.Distance(worldScript.borderCollider).isOverlapped)
                return true;

            //Loop through all of the obstacle colliders
            //Do .distance from the ghost to each obstacle
            //If any is overlapped, return as a collision occured and this is not a valid pose
            foreach (Obstacle obstacle in worldScript.obstacleList)
            {
                if (arrowCollisionDummyCollider.Distance(obstacle.obstacleCollider).isOverlapped) { 
                    return true;
                }                
            }

            return false;
        }
    }
}
