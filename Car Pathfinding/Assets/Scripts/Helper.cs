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

        //TODO
        public void checkCollision()
        {

        }

        public void setUpObstacleCollider(Obstacle obstacle)
        {
            obstacle.obstacleGameobject.transform.position = obstacle.center;
            BoxCollider2D tmpCollider = obstacle.obstacleGameobject.GetComponent<BoxCollider2D>();

            tmpCollider.offset = new Vector2(0, 0);
            tmpCollider.size = new Vector2(obstacle.length, obstacle.width);
            tmpCollider.transform.rotation = Quaternion.Euler(0,0,obstacle.theta*Mathf.Rad2Deg);

            obstacle.obstacleCollider = tmpCollider;

        }

        public void setupCarCollider()
        {

        }



    }


}

