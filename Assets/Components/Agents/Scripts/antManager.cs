using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;
using System.Linq; 

namespace Antymology.AgentScripts
{
    // TODO: RESTRICT ONLY ONE ANT ON A BLOCK AT A TIME

    /* 
     * Manages ants actions
     * References at bottom of file
     */
    public class antManager : MonoBehaviour
    {
        private System.Random RNG;

        private AntHealth antHealth;

        private AntHealth otherAntHealth;



        private float digProbability = 0.25f;
        private float moveProbability = 0.25f;

        public float _timeToWaitInbetween;
        public float _waitTimer;

        private void Awake()
        {
            // Generate new random number generator
            RNG = new System.Random(ConfigurationManager.Instance.Seed);

            antHealth = GetComponent<AntHealth>();

            // Will get updated very quickly if it should be true
            checkIfOnAcidicBlock();
           
            otherAntHealth = GetComponent<AntHealth>();

        }

        void Update()
        {
            float m = (float)new System.Random((int)System.DateTime.Now.Ticks).NextDouble() * 100;
            if (m > (1 - moveProbability))
                moveAnt();

        }

        public int[] getCurrentWorldXYZAnt()
        {
            int[] currentXYZWorldAntCoord = AntPosition.getAntCurrentPosition(transform.position);

            return currentXYZWorldAntCoord;
        }

        #region MOVEMENT


        // Only moves ants every X seconds, set in Inspector
        public void moveAnt()
        {

            if (_waitTimer >= _timeToWaitInbetween)
            {
                int[] pos = getCurrentWorldXYZAnt();

                int x = pos[0];
                int y = pos[1];
                int z = pos[2];

                // Get possible directions to move in based on current position
                int[][] possibleDirections = getPossibleDirections(x, y, z);

                // Use the direction variable to select an array from the possibleDirections jagged array to move to
                int direction = 0;

                // If there is actually a place to move
                if (possibleDirections != null)
                {
                    // If there is multiple places to move, not just one
                    if (possibleDirections.Length > 1)
                        // Gets a random direction from options to move in
                        direction = RNG.Next(0, possibleDirections.Length - 1);

                    x = possibleDirections[direction][0];

                    y = possibleDirections[direction][1];

                    z = possibleDirections[direction][2];

                    // TODO: ADD IN ROTATION/DIRECTION OF ANTS MOVEMENT

                    // Select a random direction to go to out of possible options
                    transform.position = new Vector3(x, y, z);

                    // Everytime ant moves, check where he is
                    checkWhatBlockAntIsOn();
                }
                 _waitTimer = 0f;
            }
            else { _waitTimer += 1 * Time.deltaTime; } 
        }

        public void moveAntUpOne()
        {
            int[] pos = getCurrentWorldXYZAnt();

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];
            transform.position = new Vector3(x, y + 1, z);
        }

        public void moveAntDownOne()
        {
            int[] pos = getCurrentWorldXYZAnt();

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];
            transform.position = new Vector3(x, y - 1, z);
        }

        // Some code learned from office hours w/Cooper
        // When moving from one black to another, ants are not allowed to move to a
        // block that is greater than 2 units in height difference
        // Could have also used 2D arrays instead of jagged
        private int[][] getPossibleDirections(int xCoord, int yCoord, int zCoord)
        {
            int[] neighbourYCoords = new int[8];

            int[][] neighbourXZCoords = new int[][]
            {
                new int[] { xCoord - 1, zCoord + 1 },
                new int[] { xCoord, zCoord + 1 },
                new int[] { xCoord + 1, zCoord+1 },
                new int[] { xCoord - 1, zCoord },
                new int[] { xCoord + 1, zCoord },
                new int[] { xCoord - 1, zCoord - 1 },
                new int[] { xCoord, zCoord - 1 },
                new int[] { xCoord + 1, zCoord - 1 }
            };

            // Max 8 directions, 3 int in each direction
            // When initializing an array in c#, all values will be 0. So we will need to remove those later.
            int[][] possibleDirections = new int[8][];

            for (int i = 0; i < 8; i++)
            {
                possibleDirections[i] = new int[3];
            }


            // Find y coordinate of each neighbour
            for (int i = 0; i < 8; i++)
            {
                neighbourYCoords[i] = WorldManager.Instance.getHeightAt(neighbourXZCoords[i][0], neighbourXZCoords[i][1]);
            }

            // Check all the neighbour y coordinates
            for (int i = 0; i < neighbourYCoords.Length; i++)
            {
                // If the difference between the neighbouring block and current block is less than 2 units
                // save the coordinates as a possible direction to move in
                if (Mathf.Abs(neighbourYCoords[i] - yCoord) < 2)
                {
                    // x coord of the neighbour
                    possibleDirections[i][0] = neighbourXZCoords[i][0];
                    // y coord of the neighbour
                    possibleDirections[i][1] = neighbourYCoords[i];
                    // z coord of the neighbour
                    possibleDirections[i][2] = neighbourXZCoords[i][1];
                }
            }

            // Now we have a jagged array of all the possible moves ant can make.
            // If array not full of moves, than the "empty" spots are occupied by zeros

            // Default where to "slice" the jagged array
            int whereToSlice = 8;

            // Set whereToSlice to the first all zero sub array in jagged array
            for (int i = 0; i < possibleDirections.Length; i++)
            {
                if (possibleDirections[i].Sum() == 0)
                {
                    whereToSlice = i;
                    break;
                }
            }

            // Deal with case where there is nowhere to move
            if (whereToSlice == 0)
            {
                return null;
            }
            // whereToSlice can = 1 through 8
            else {
                int[][] possibleDirectionsNoZeros = new int[whereToSlice][];
                
                for (int i = 0; i < possibleDirectionsNoZeros.Length; i++)
                {
                    possibleDirectionsNoZeros[i] = possibleDirections[i];
                }
                return possibleDirectionsNoZeros;
            }
        }

        #endregion


        #region INTERACT WITH BLOCKS

        private void checkWhatBlockAntIsOn()
        {
            int[] pos = getCurrentWorldXYZAnt();

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            getPossibleDirections(x, y, z);

            AbstractBlock ab = WorldManager.Instance.GetBlock(x, y, z);

            // Always changes
            // TODO: MAKE THIS PROBABILITY PART OF THE "GENOME" 
            float p = (float)new System.Random((int)System.DateTime.Now.Ticks).NextDouble() * 100;

            // 20% probability to dig the block
            if (p > (1 - digProbability))
            {
                digBlock(ab, x, y, z);
                antHealth.standingOnAcidicBlock = false;
            }
            else if (ab.GetType().Equals(typeof(Antymology.Terrain.MulchBlock)))
            {
                antHealth.standingOnAcidicBlock = false;
                consumeMulch(x, y, z);
            }
            else if (ab.GetType().Equals(typeof(Antymology.Terrain.AcidicBlock)))
            {
                antHealth.standingOnAcidicBlock = true;
            }
            else
            {
                antHealth.standingOnAcidicBlock = false;
                transform.position = new Vector3(x, y + 1, z);
            }
        }

        private void checkIfOnAcidicBlock()
        {
            int[] pos = getCurrentWorldXYZAnt();

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            getPossibleDirections(x, y, z);

            AbstractBlock ab = WorldManager.Instance.GetBlock(x, y, z);

            if (!ab.GetType().Equals(typeof(Antymology.Terrain.AcidicBlock)))
            {
                antHealth.standingOnAcidicBlock = false;
            }
            else
            {
                antHealth.standingOnAcidicBlock = true;
            }
            
        }


        private void consumeMulch(int xMulchBlock, int yMulchBlock, int zMulchBlock)
        {
            antHealth.eatMulchGainHealth();

            // Replace mulch block with airblock
            WorldManager.Instance.SetBlock(xMulchBlock, yMulchBlock, zMulchBlock, new AirBlock());
            //moveAntDownOne();

        }

        // Remove block from world by digging it
        private void digBlock(AbstractBlock currentBlock, int xBlockToDig, int yBlockToDig, int zBlockToDig)
        {
            if (!currentBlock.GetType().Equals(typeof(Antymology.Terrain.ContainerBlock)))
            {
                WorldManager.Instance.SetBlock(xBlockToDig, yBlockToDig, zBlockToDig, new AirBlock());
                //moveAntDownOne();
            }
        }

        #endregion

        #region COLLISIONS

        private void OnTriggerEnter(Collider other)
        {
            Debug.Log("SOMETHING COLLIDED");
            // Ants may give some of their health to other ants occupying the same space (must be a zero-sum exchange)
            if (other.CompareTag("ant"))
            {
                Debug.Log("AN ANT COLLIDED");
                antHealth.canEat = false;

                // Need reference to antBehaviour script on other ant, pass as argument
                otherAntHealth.health += antHealth.shareHealth(otherAntHealth.health);

            }
        }

        private void OnTriggerExit(Collider other)
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

