using System;
using UnityEngine;
using Antymology.Helpers;

namespace Antymology.AgentScripts
{

    /// <summary>
    /// Methods used by both common ants and the Queen ant
    /// </summary>
    public sealed class antAndQueenController
    {

        public static readonly antAndQueenController Instance = new antAndQueenController();

        private antAndQueenController() { }


        public void moveAntUpOne(GameObject go)
        {
            int[] pos = getCurrentWorldXYZAnt(go);

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];
            go.transform.position = new Vector3(x, y + 1, z);
        }

        public void moveAntDownOne(GameObject go)
        {
            int[] pos = getCurrentWorldXYZAnt(go);

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];
            go.transform.position = new Vector3(x, y - 1, z);
        }

        public int[] getCurrentWorldXYZAnt(GameObject go)
        {
            int[] currentXYZWorldAntCoord = AntPosition.getAntCurrentPosition(go.transform.position);

            return currentXYZWorldAntCoord;
        }

    }
}

