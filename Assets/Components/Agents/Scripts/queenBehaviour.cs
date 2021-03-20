using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;

namespace Antymology.AgentScripts
{
    public class queenBehaviour : antManager
    {
        private AntHealth antHealth;

        // Start is called before the first frame update
        void Start()
        {
            antHealth = GetComponent<AntHealth>();
        }


        private void produceNestBlock()
        {
            int[] pos = getCurrentWorldXYZAnt();

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            WorldManager.Instance.SetBlock(x, y, z, new NestBlock());

            antHealth.costQueenHealth();
        }
    }
}

