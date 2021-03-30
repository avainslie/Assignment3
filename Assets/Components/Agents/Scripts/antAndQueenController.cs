using System;
using UnityEngine;
using Antymology.Helpers;

namespace Antymology.AgentScripts
{

    /// <summary>
    /// Methods used by both common ants and the Queen ant
    /// </summary>
    public static class antAndQueenController
    {
        // Moves ant up one world unit
        public static void moveAntUpOne(GameObject go)
        {
            int[] pos = getCurrentWorldXYZAnt(go);

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];
            go.transform.position = new Vector3(x, y + 1, z);
        }

        // Moves ant down one world unit
        public static void moveAntDownOne(GameObject go)
        {
            int[] pos = getCurrentWorldXYZAnt(go);

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];
            go.transform.position = new Vector3(x, y - 1, z);
        }

        // Gives the current coordinates of an ant
        public static int[] getCurrentWorldXYZAnt(GameObject go)
        {
            int[] currentXYZWorldAntCoord = AntPosition.getAntCurrentPosition(go.transform.position);

            return currentXYZWorldAntCoord;
        }

    }
}

