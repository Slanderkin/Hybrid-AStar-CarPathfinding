using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{

    public class Parameters
    {
        //World Parameters
        public const int cellWidth = 30;
        public const float worldSizeX = 3000;
        public const float worldSizeY = 1500;
       
        //Car Parameters
            //Position
        public const float carX0 = worldSizeX / 2;
        public const float carY0 = worldSizeY / 2;
        public const float carTheta0 = 0;
            //Others
        public const float speed = 35;
        public const float turnAngle = 45;
        public const float carSpriteScale = 30;
        public const float carLength = 150;
        public const float carWidth = 70.5f; //The choice to make its width a third of its length is somewhat arbitrary
    }
    

}

