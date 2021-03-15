using System;
using UnityEngine;


namespace Antymology.Helpers
{
    public static class AntPosition
    {
        
        // Takes position of gameObject and returns the values in an array
        public static int[] getAntCurrentPosition(Vector3 position)
        {
            int[] AntPositionCoordinates = new int[3];


            position.x = CustomMath.fastfloor((double)position.x);
            position.y = CustomMath.fastfloor((double)position.y);
            position.z = CustomMath.fastfloor((double)position.z);

            AntPositionCoordinates[0] = (int) position.x;
            AntPositionCoordinates[1] = (int) position.y;
            AntPositionCoordinates[2] = (int) position.z;

            return AntPositionCoordinates;
        }
    }
}

