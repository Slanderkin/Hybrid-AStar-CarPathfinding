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
        private float arrowOffset = 0.2f;
        private bool startedPlacingGoal = false;
        public bool finishedPlacingGoal = false;

        public float arrowScale = 15f;

        private float width;
        private float height;

        private Helper helper;

        public SpriteRenderer spriteRenderer;
        // Start is called before the first frame update
        void Start()
        {
            helper = new Helper();


            transform.position = new Vector3(0, 0, 0);
            child.transform.position = new Vector3(0, 0, -100);
            spriteRenderer = child.GetComponent<SpriteRenderer>();
            spriteRenderer.transform.localScale = new Vector3(arrowScale, arrowScale, 1);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f); //Can't just change the color.a field :(

            width = Parameters.worldSizeX;
            height = Parameters.worldSizeY;
        }

        // Update is called once per frame
        void Update()
        {
            if (!startedPlacingGoal)
                preSelectHover();
            if (!helper.isMouseOverUi())
                checkMouse();

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
                    spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1f);
                    endPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    directionVector = endPoint - startPoint;
                    directionVector = directionVector.normalized;
                    vectorAngle = Mathf.Atan2(directionVector[1], directionVector[0]);
                    child.transform.rotation = Quaternion.Euler(0, 0, vectorAngle * 180 / Mathf.PI);
                    child.transform.localPosition = new Vector3(Mathf.Sqrt(arrowOffset) * Mathf.Cos(vectorAngle), Mathf.Sqrt(arrowOffset) * Mathf.Sin(vectorAngle), 0);

                }

            }
            else if (!Input.GetMouseButton(0) && startedPlacingGoal && !finishedPlacingGoal)
            {
                finishedPlacingGoal = true;
                directionVector = endPoint - startPoint;
                directionVector = directionVector.normalized;
                vectorAngle = Mathf.Atan2(directionVector[1], directionVector[0]);
                child.transform.rotation = Quaternion.Euler(0, 0, vectorAngle * 180 / Mathf.PI);
                child.transform.localPosition = new Vector3(Mathf.Sqrt(arrowOffset) * Mathf.Cos(vectorAngle), Mathf.Sqrt(arrowOffset) * Mathf.Sin(vectorAngle), 0);
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
    }
}
