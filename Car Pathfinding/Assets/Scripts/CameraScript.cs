using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class CameraScript : MonoBehaviour
{
    private Camera cameraObject;
    private GameObject world;

    public Transform parentObject;
    public float zoomGoal;
    public float sensitivity = 150;
    public float zoomSpeed = 5000;
    public float scrollSpeed = 1;
    public float maxZoom = 1600;
    public float minZoom = 100;
    float zoomAmount = Parameters.worldSizeY - 400;

    // Start is called before the first frame update
    void Start()
    {

        cameraObject = gameObject.GetComponent<Camera>();
        cameraObject.orthographicSize = zoomAmount;
        zoomGoal = zoomAmount;
        transform.position = new Vector3(Parameters.worldSizeX/2f, Parameters.worldSizeY/2f,-10);
    }

    // Update is called once per frame
    void Update()
    {
        //Always check for zoom changes
        zoomGoal -= Input.mouseScrollDelta.y * sensitivity;
        zoomGoal = Mathf.Clamp(zoomGoal, minZoom, maxZoom);
        zoomAmount = Mathf.MoveTowards(zoomAmount, zoomGoal, zoomSpeed * Time.deltaTime);
        cameraObject.orthographicSize = zoomAmount;

        //Only move if one of the movement keys are down (stops an "ice-like" gliding after releasing a movement key)
        if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            parentObject.transform.Translate(System.Math.Sign(Input.GetAxis("Horizontal")) * zoomGoal * scrollSpeed * Time.deltaTime, System.Math.Sign(Input.GetAxis("Vertical")) * zoomGoal * scrollSpeed * Time.deltaTime, 0);
        }


    }
}
