using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Pathfinding
{
    public class Helper{



        //Takes the current position as floats and returns the discretized position
        public Vector2Int worldPos2Cell(float posX, float posY)
        {
            return new Vector2Int(Mathf.FloorToInt(posX) / Parameters.cellWidth, Mathf.FloorToInt(posY) / Parameters.cellWidth);
        }

        public bool isMouseOverUi()
        {
            return EventSystem.current.IsPointerOverGameObject();
        }

        //Closes the application
        public void exitApp()
        {
            Application.Quit();
        }





    }


}

