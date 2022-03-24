using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class Car : MonoBehaviour
{

    public WorldScript world;

    public GameObject carObj;
    public GameObject carGhostObj; //The previous car pose will have a faded sprite (the "ghost")
    
    public SpriteRenderer carSprite;
    public SpriteRenderer ghostSprite;

    public BoxCollider2D carCollider;

    public Vector3 currPose;
    public Vector3 prevPose;
    public float speed;
    public float turnAngle;
    public float carSpriteScale;
    public float carLength;
    public float carWidth;

    private void Start()
    {
        prevPose = new Vector3(-1, -1, 0); //This sprite will be disabled until the pose is changed from -1
        currPose = new Vector3(Parameters.carX0, Parameters.carY0, Parameters.carTheta0);
        speed = Parameters.speed;
        turnAngle = Parameters.turnAngle;
        carSpriteScale = Parameters.carSpriteScale;
        carLength = Parameters.carLength;
        carWidth = Parameters.carWidth;

    }
    
    

    private void Update()
    {
        
    }

    //Takes the pose that was ended at with the previous A* iteration and updates the ghost's pose
    public void updatePose(Vector3 newPose)
    {
        prevPose = currPose;
        currPose = newPose;
    }


    /*
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detected collision between " + gameObject.name + "and " + other.GetComponent<Collider>().name);
    }*/

    /*
    public float carScale = 30f;
    public float speed = 1.5f;
    public float turnAngle = 30f;
    public float carLength = 6f;
    float currX;

    float currY;
    float currHeading; //rad

    private GameObject world;
    public WorldScript worldScript;
    private EdgeCollider2D worldBorderCollider;


    //This is for the car itself
    private GameObject carChild;
    public BoxCollider2D carBoxCollider;

    //This is for when testing is done with theoretical movements (determines out of bound collisions)
    private GameObject carChildGhost;
    public BoxCollider2D carGhostBoxCollider;

    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        //Set up world object in the context of this script (used almost like an import or include statement)
        world = GameObject.Find("World");
        worldScript = world.GetComponent<WorldScript>();
        worldBorderCollider = worldScript.borderCollider;

        //Modify parameters for the car
        carLength *= carScale;
        currX = Parameters.worldSizeX / 2;
        currY = Parameters.worldSizeY / 2;
        currHeading = 0;

        //Set up the sprite renderer object in the context of this script (used almost like an import or include statement)
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.transform.localScale = new Vector3(carScale, carScale, 1);
        spriteRenderer.transform.position = new Vector3(currX, currY, 1);

        carChild = GameObject.Find("Car");
        carBoxCollider = carChild.GetComponent<BoxCollider2D>();

        carChildGhost = GameObject.Find("CarChildGhost");
        carGhostBoxCollider = carChildGhost.GetComponent<BoxCollider2D>();

        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (Input.GetKey("w"))
        {
            if (Input.GetKey("a"))
            {
                move(-1,1);
            }
            else if (Input.GetKey("d"))
            {
                move(1,1);
            }
            else
            {
                move(0,1);
            }
             
        }
        else if (Input.GetKey("s"))
        {
            if (Input.GetKey("a"))
            {
                move(-1, -1);
            }
            else if (Input.GetKey("d"))
            {
                move(1, -1);
            }
            else
            {
                move(0, -1);
            }

        }
    }

    //Determines if the car's movement would collide with the world border
    public bool doesCollide(BoxCollider2D carCollider, EdgeCollider2D worldBorder)
    {
        return carCollider.Distance(worldBorder).distance < 0;
    }

    //Basic car motion that takes a turn direction (-1 left, 0 straight, 1 right) and a drive direction (1 fwd, -1 back)
    public void move(int turnDir,int driveDir)
    {
        float beta = speed/ carLength * Mathf.Tan(turnAngle*turnDir);
        float cx;
        float cy;
        if(turnDir != 0)
        {
            float R = speed / beta;
            beta *= Time.deltaTime;
            cx = currX - Mathf.Sin(currHeading) * R;
            cy = currY + Mathf.Cos(currHeading) * R;
            currX = cx + Mathf.Sin(currHeading + driveDir*beta) * R;
            currY = cy - Mathf.Cos(currHeading + driveDir*beta) * R;
            currHeading = (currHeading + driveDir*beta + 2 * Mathf.PI) % (2*Mathf.PI); //Adding 2 pi makes sure this value is never negative
        }
        else
        {
            float dist = speed * Time.deltaTime;
            currX += driveDir*dist*Mathf.Cos(currHeading);
            currY += driveDir * dist *Mathf.Sin(currHeading);
            currHeading = (currHeading + driveDir * beta) % (2 * Mathf.PI);
        }
        updateSprite();
    }

    //Moves the sprite to match the current pose of the car
    private void updateSprite()
    {
        spriteRenderer.transform.position = new Vector3(currX, currY, 1);
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, currHeading * 180 / Mathf.PI);
    }

    //Returns the car pose in (x,y,heading)
    public Vector3 getPose()
    {
        return new Vector3(currX,currY,currHeading);
    }*/
}
