using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;
using System.Linq; 

namespace Antymology.AgentScripts
{
    /// <summary>
    /// Manages common ants actions
    /// References at bottom of file
    /// </summary>
    public class antManager : MonoBehaviour
    {
        private System.Random RNG;

        public AntHealth antHealth;

        private AntHealth otherAntHealth;

        private GameObject queen;

        private queenBehaviour queenBehaviour;

        // INPUTS
        public float[] inputs;
        public float distToQueen;
        public float xCoord;
        public float yCoord;
        public float zCoord;
        public float currentHealth;
        public float queensHealth;

        public float antTimeToWaitInbetween = 10f;
        public float antWaitTimer = 0f;

        private void Awake()
        {
            // Generate new random number generator
            RNG = new System.Random(ConfigurationManager.Instance.Seed);

            antHealth = GetComponent<AntHealth>();

            // Get queen's gameobject and script
            queen = GameObject.FindGameObjectWithTag("queen");
            queenBehaviour = queen.GetComponent<queenBehaviour>();
        }

        private void Update()
        {
            checkIfOnAcidicBlock();

            if (antWaitTimer >= antTimeToWaitInbetween)
            {
                // Get inputs
                int[] pos = AntPosition.getAntCurrentPosition(transform.position);

                xCoord = pos[0];
                yCoord = pos[1];
                zCoord = pos[2];

                distToQueen = calculateDistToQueen();

                currentHealth = antHealth.health;
                
                queensHealth = queenBehaviour.queenAntHealth.health;

                inputs = new float[6] { distToQueen, xCoord, yCoord, zCoord, currentHealth, queensHealth };

                // Get output from the current neural net based on inputs
                string decision = NeuralNetController.Instance.runNeuralNet(inputs);

                // Do something based on highest probability output
                switch (decision)
                {
                    case "moveF":
                        moveAnt("F");
                        Debug.Log("MOVEF");
                        break;

                    case "moveB":
                        moveAnt("B");
                        Debug.Log("MOVEB");
                        break;

                    case "moveR":
                        // Rotate then move forward
                        transform.Rotate(new Vector3(0, 90, 0));
                        moveAnt("F");
                        Debug.Log("MOVER");
                        
                        break;

                    case "moveL":
                        // Rotate then move forward
                        transform.Rotate(new Vector3(0, -90, 0));
                        moveAnt("F");
                        Debug.Log("MOVEL");
                        
                        break;

                    case "nothing":
                        Debug.Log("NOTHING");
                        break;

                    case "dig":
                        Debug.Log("DIG");
                        digBlock((int) xCoord, (int) yCoord - 1 , (int) zCoord);
                        break;

                    case "eat":
                        Debug.Log("EAT");
                        consumeMulch((int) xCoord, (int) yCoord - 1, (int) zCoord);
                        break;
                }

                checkIfOnAcidicBlock();
                antWaitTimer = 0f;
            }
            else { antWaitTimer += 1 * Time.deltaTime; }

        }

        #region MOVEMENT

        public void checkIfOnAcidicBlock()
        {
            int[] pos = AntPosition.getAntCurrentPosition(transform.position);

            xCoord = pos[0];
            yCoord = pos[1];
            zCoord = pos[2];

            if ((WorldManager.Instance.GetBlock((int)xCoord, (int)yCoord - 1, (int)zCoord) as AcidicBlock ) != null)
                antHealth.standingOnAcidicBlock = true;
            else
                antHealth.standingOnAcidicBlock = false;
        }

       
        public void moveAnt(string move)
        {
            int[] pos = antAndQueenController.getCurrentWorldXYZAnt(gameObject);

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            int direction = 0;

            if (move.Equals("F") || move.Equals("B"))
            {
                int[][] possibleDirections = DirectionFinder.getPossibleForwardORBackwardDirections(x, y, z, move);

                if (possibleDirections != null)
                {
                    if (possibleDirections.Length > 1)
                        direction = RNG.Next(0, possibleDirections.Length - 1);

                    x = possibleDirections[direction][0];

                    y = possibleDirections[direction][1];

                    z = possibleDirections[direction][2];

                    // Apply movement
                    transform.position = new Vector3(x, y, z);

                    // Turn ant around if moved backwards
                    if (move.Equals("B"))
                        transform.Rotate(new Vector3(0, 180, 0));
                }
                
            }  
        }

        #endregion

        

        #region INTERACT WITH BLOCKS

        // Remove block from world by digging it
        private void digBlock(int xBlockToDig, int yBlockToDig, int zBlockToDig)
        {
            // Only digs non-container blocks
            if ((WorldManager.Instance.GetBlock(xBlockToDig, yBlockToDig, zBlockToDig) as ContainerBlock) == null)
            {
                WorldManager.Instance.SetBlock(xBlockToDig, yBlockToDig, zBlockToDig, new AirBlock());
                transform.position = new Vector3(xBlockToDig, yBlockToDig, zBlockToDig);
            }
        }


        private void consumeMulch(int xBlockToEat, int yBlockToEat, int zBlockToEat)
        {
            // Only eats mulch blocks
            if ((WorldManager.Instance.GetBlock(xBlockToEat, yBlockToEat, zBlockToEat) as MulchBlock) != null)
            {
                antHealth.standingOnAcidicBlock = false;
                antHealth.eatMulchGainHealth();
                // Replace mulch block with airblock
                WorldManager.Instance.SetBlock(xBlockToEat, yBlockToEat, zBlockToEat, new AirBlock());
                transform.position = new Vector3(xBlockToEat, yBlockToEat, zBlockToEat);
            }
        }

        #endregion

        #region QUEEN KNOWLEDGE

        private float calculateDistToQueen()
        {
            float dist = Vector3.Distance(gameObject.transform.position, queen.transform.position);
            return dist;
        }

        #endregion

        #region COLLISIONS

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("ant") || other.CompareTag("queen"))
            {
                Debug.Log("ANT OR QUEEN COLLISION");

                // Ants cannot eat when in the same space
                antHealth.canEat = false;

                otherAntHealth = other.GetComponent<AntHealth>();

                // Ants may give some of their health to other ants occupying
                // the same space (must be a zero-sum exchange)
                if (otherAntHealth != null)
                    antHealth.shareHealthToAntWithLess(other.tag, otherAntHealth);

                Invoke("adjustHeight", 1f);
            }
        }

        private void adjustHeight()
        {
            int[] pos = AntPosition.getAntCurrentPosition(transform.position);
            int x = pos[0];
            int z = pos[2];
            int y = WorldManager.Instance.getHeightAt(x, z);
            transform.position = new Vector3(x, y, z);
        }


        private void OnTriggerExit()
        {
            antHealth.canEat = true;
        }


        #endregion
    }



    /* REFERENCES
     * 
     * InvokeRepeating:
     * https://answers.unity.com/questions/17131/execute-code-every-x-seconds-with-update.html
     * 
     * Wait timer:
     * https://youtu.be/j-28BbzvgGk
     * 
     * 
     * 
     * 
     */


}

