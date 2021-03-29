using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;
using Antymology.UI;
using TMPro;

namespace Antymology.AgentScripts
{
    public class queenBehaviour : MonoBehaviour
    {
        public float queenTimeToWaitInbetween = 2f;
        public float queenWaitTimer = 0f;

        public AntHealth queenAntHealth;

        private void Awake()
        {
            queenAntHealth = GetComponent<AntHealth>();
        }

        private void Update()
        {
            checkQueenHealthAndMove();
        }

        private void checkQueenHealthAndMove()
        {
            if (queenWaitTimer >= queenTimeToWaitInbetween)
            {
                if (queenAntHealth.health > (1000 / 3))
            {
                produceNestBlock();

                    Invoke("moveQueen", 1f);

            }
                queenWaitTimer = 0f;
            }
            else { queenWaitTimer += 1 * Time.deltaTime; }
        }

        private void moveQueen()
        {
            int[] queenCurrentPosition = antAndQueenController.getCurrentWorldXYZAnt(gameObject);
            int queenX = queenCurrentPosition[0];
            int queenY = queenCurrentPosition[1];
            int queenZ = queenCurrentPosition[2];

            int yInfrontOfQueen = WorldManager.Instance.getHeightAt(queenX, queenZ + 1);
            int yToRightOfQueen = WorldManager.Instance.getHeightAt(queenX + 1, queenZ);
            int yToLeftOfQueen = WorldManager.Instance.getHeightAt(queenX - 1, queenZ);


            // If nothing greater or smaller in height diff of 2 infront of her
            // Then move forward
            if (Mathf.Abs(queenY - yInfrontOfQueen) <= 2)
            {
                transform.position = new Vector3(queenX, yInfrontOfQueen, queenZ + 1);
            }

            else if (Mathf.Abs(queenY - yToLeftOfQueen) <= 2 && (WorldManager.
                Instance.GetBlock(queenX - 1, yToLeftOfQueen, queenZ) as NestBlock) == null)
            {
                transform.position = new Vector3(queenX - 1, yToLeftOfQueen, queenZ);
                transform.Rotate(new Vector3(0, -90, 0));
            }

            else if (Mathf.Abs(queenY - yToRightOfQueen) <= 2)
            {
                transform.position = new Vector3(queenX + 1, yToRightOfQueen, queenZ);
                transform.Rotate(new Vector3(0, 90, 0));
            }

        }

        private void produceNestBlock()
        {
            int[] pos = antAndQueenController.getCurrentWorldXYZAnt(gameObject);

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            if (queenAntHealth.health >= (1000 / 3))
            {
                WorldManager.Instance.SetBlock(x, y, z, new NestBlock());
                antAndQueenController.moveAntUpOne(gameObject);

                NestUI.Instance.addNestBlockToCount();

                queenAntHealth.costQueenHealth();
            }
            
        }

    }
}

