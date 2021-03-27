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
            {
                produceNestBlock();

                int[] queenCurrentPosition = getCurrentWorldXYZAnt();
                int queenX = queenCurrentPosition[0];
                int queenY = queenCurrentPosition[1];
                int queenZ = queenCurrentPosition[2];

                int yInfrontOfQueen = WorldManager.Instance.getHeightAt(queenX, queenZ);

                // If nothing greater or smaller in height diff of 2 infront of her
                // Then move forward
                if (Mathf.Abs(queenY - yInfrontOfQueen) < 2)
                {
                    moveQueenforward();
                }
            }
                
            
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

            // ASSUMING NO OBSTACLES (aka no block greater than 2 height diff)

            // Go forward until hit wall
            // Turn right
            // Turn right
            // Go forward until hit wall
            // Turn left
            // Turn left
            // Repeat
            // If wall on right
            // Move up one
            // Repeat
        }

        private void moveQueenforward()
        {
           
        }

        private void turnQueenRight()
        {

        }

        private void turnQueenLeft()
        {

        }

        private void moveQueenUpOne()
        {

        }

    }
}

