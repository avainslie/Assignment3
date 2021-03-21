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
        private System.Random RNG;

        private float nestProbability = 0.90f;

        private void Awake()
        {
            RNG = new System.Random();
            antHealth = GetComponent<AntHealth>();
        }

        private void LateUpdate()
        {

            int r = CustomMath.fastfloor(RNG.NextDouble());
            if (r > (1 - nestProbability))
            {
                produceNestBlock();
            }
        }

        private void produceNestBlock()
        {
            if (_waitTimer >= _timeToWaitInbetween + 5)
            {
                int[] pos = getCurrentWorldXYZAnt();

                int x = pos[0];
                int y = pos[1];
                int z = pos[2];

                WorldManager.Instance.SetBlock(x, y, z, new NestBlock());
                moveAntUpOne();

                NestUI.Instance.addNestBlockToCount();
                
                antHealth.costQueenHealth();
                _waitTimer = 0f;
            }
            else { _waitTimer += 1 * Time.deltaTime; }
        }
    }
}

