using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;
using Antymology.UI;
using TMPro;

namespace Antymology.AgentScripts
{
    public class queenBehaviour : antManager
    {
        private AntHealth antHealth;

        private System.Random RNG;

        private void Awake()
        {
            RNG = new System.Random();

        }

        private void Update()
        {
            int r = CustomMath.fastfloor(RNG.NextDouble() * 100);
            if (r > 80f)
            {
                produceNestBlock();
            }
        }

        private void produceNestBlock()
        {
            int[] pos = getCurrentWorldXYZAnt();
           
            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            WorldManager.Instance.SetBlock(x, y, z, new NestBlock());

            NestUI.Instance.addNestBlockToCount(); 

            antHealth.costQueenHealth();
        }
    }
}

