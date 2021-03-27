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
        public float _timeToWaitInbetween = 10f;
        public float _waitTimer = 0f;


        private void Awake()
        {
            antHealth = GetComponent<AntHealth>();
        }

        private void LateUpdate()
        {
            checkQueenHealth();
        }

        private void checkQueenHealth()
        {
            if (antHealth.health > antHealth.maxHealth / 3)
                produceNestBlock();
        }

        private void produceNestBlock()
        {
            int[] pos = getCurrentWorldXYZAnt();

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            WorldManager.Instance.SetBlock(x, y, z, new NestBlock());
            moveAntUpOne();

            NestUI.Instance.addNestBlockToCount();
                
            antHealth.costQueenHealth();
        }

        private void moveQueen()
        {
            // MOVE QUEEN IN A SMART WAY TO MAXIMIZE NEST BLOCK PRODUCTION 
        }

    }
}

