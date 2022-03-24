using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pathfinding;

public class UIScript : MonoBehaviour
{
   
    Toggle visiblityToggle;
    Toggle gridToggle;
    Toggle debugToggle;
    Toggle pathfindingLinesToggle;

    private Text iterNum;


    public SpriteRenderer carRenderer;
    public SpriteRenderer arrowRenderer;

    public MeshRenderer meshRenderer;
    public Canvas debugMenu;

    public PathFinding pathfinding;
    public DrawManager drawManager;
    public ArrowScript arrowScript;

    // Start is called before the first frame update
    void Start()
    {
        visiblityToggle = this.transform.Find("Sprite Visibility Toggle").GetComponentInChildren<Toggle>();

        visiblityToggle.onValueChanged.AddListener(delegate {
            updateToggle(visiblityToggle);
        });

        gridToggle = this.transform.Find("Grid Visibility Toggle").GetComponentInChildren<Toggle>();

        gridToggle.onValueChanged.AddListener(delegate {
            updateToggle(gridToggle);
        });

        debugToggle = this.transform.Find("Debug Menu Toggle").GetComponentInChildren<Toggle>();

        debugToggle.onValueChanged.AddListener(delegate {
            updateToggle(debugToggle);
        });

        pathfindingLinesToggle = this.transform.Find("Pathfinding Line Toggle").GetComponentInChildren<Toggle>();

        pathfindingLinesToggle.onValueChanged.AddListener(delegate {
            updateToggle(pathfindingLinesToggle);
        });
        
        debugMenu.enabled = false;

        iterNum = debugMenu.transform.Find("Iteration Number").GetComponent<Text>();
        

    }


    //Use this to update the iteration # regardless of whether we are in "debug mode" or not  
    private void Update()
    {
        iterNum.text = "Iteration #: " + pathfinding.getIerationNum();

    }

    //Updates the visibility of the car and the arrow
    void updateToggle(Toggle toToggle )
    {
        if(toToggle.name == "Grid Visibility Toggle")
        {
            meshRenderer.enabled = toToggle.isOn;
        }
        else if (toToggle.name == "Sprite Visibility Toggle")
        {
            carRenderer.enabled = toToggle.isOn;
            if(arrowRenderer.GetComponentInChildren<Transform>().position.z > -10f)
            {
                arrowRenderer.enabled = carRenderer.enabled;
            }

        }
        else if (toToggle.name == "Debug Menu Toggle")
        {

            debugMenu.enabled = toToggle.isOn;

        }
        else if (toToggle.name == "Pathfinding Line Toggle")
        {
            drawManager.doDrawPathfindingTree = toToggle.isOn;
        }

    }

    public void simNode()
    {
        //Don't run this if we haven't set a goal yet
        if (!pathfinding.arrowScript.finishedPlacingGoal)
            return;
        //Don't run this si we are done pathfinding
        else if (pathfinding.donePathfinding)
            return;

        pathfinding.simNextNode();
        Node latestNode = pathfinding.getLatestNode();

        //latestNode.pose[2] *= 180 / Mathf.PI;

        Text nodePose = debugMenu.transform.Find("Node Pose").GetComponent<Text>();
        nodePose.text = "Node Pose: " + latestNode.pose.ToString();

        Text FCost = debugMenu.transform.Find("FCost").GetComponent<Text>();
        FCost.text = "F Cost: " + latestNode.fCost.ToString();

        Text HCost = debugMenu.transform.Find("GCost").GetComponent<Text>();
        HCost.text = "H Cost: " + latestNode.hCost.ToString();

        Text GCost = debugMenu.transform.Find("HCost").GetComponent<Text>();
        GCost.text = "G Cost: " + latestNode.gCost.ToString();

    }

    public void simPath()
    {
        //Don't run this if we haven't set a goal yet
        if (!pathfinding.arrowScript.finishedPlacingGoal)
            return;
        //Don't run this si we are done pathfinding
        else if (pathfinding.donePathfinding)
            return;

        pathfinding.simNextPath();
        Node latestNode = pathfinding.getLatestNode();

        //latestNode.pose[2] *= 180 / Mathf.PI;

        Text nodePose = debugMenu.transform.Find("Node Pose").GetComponent<Text>();
        nodePose.text = "Node Pose: " + latestNode.pose.ToString();

        Text FCost = debugMenu.transform.Find("FCost").GetComponent<Text>();
        FCost.text = "F Cost: " + latestNode.fCost.ToString();

        Text HCost = debugMenu.transform.Find("GCost").GetComponent<Text>();
        HCost.text = "H Cost: " + latestNode.hCost.ToString();

        Text GCost = debugMenu.transform.Find("HCost").GetComponent<Text>();
        GCost.text = "G Cost: " + latestNode.gCost.ToString();

    }

    //return the state of the debug toggler
    public bool getDebugToggleEnabled()
    {
        return debugToggle.isOn;
    }

    //Closes the application
    public void exitApp()
    {
        Application.Quit();
    }

    //Resets the vars in the misc scripts
    public void resetScripts()
    {
        pathfinding.resetPathfinding();
        arrowScript.resetArrowScript();
        drawManager.resetDrawManager();
    }

}
