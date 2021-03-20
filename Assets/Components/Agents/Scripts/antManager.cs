using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;

namespace Antymology.AgentScripts
{
    // TODO: RESTRICT ONLY ONE ANT ON A BLOCK AT A TIME

    /* 
     * Manages ants actions
     * References at bottom of file
     */
    public class antManager : MonoBehaviour
    {

        private Rigidbody rigidbody;

        private System.Random RNG;

        private AntHealth antHealth;

        [SerializeField] float _timeToWaitInbetween;
        private float _waitTimer;


        private float digProbability;

        private void Awake()
        {
            // Generate new random number generator
            RNG = new System.Random(ConfigurationManager.Instance.Seed);

            antHealth = GetComponent<AntHealth>();

            // Will get updated very quickly if it should be true
            antHealth.standingOnAcidicBlock = false;            

        }

        // Start is called before the first frame update
        void Start()
        {
            rigidbody = GetComponent<Rigidbody>();
            
        }


        void Update()
        {

            checkWhatBlockAntIsOn();
            
        }

        #region MOVEMENT

        // Only moves ants every X seconds, set in Inspector
        private void moveAnt()
        {

            if (_waitTimer >= _timeToWaitInbetween)
            {
                // TEMPORARY CODE TO MOVE ANTS AROUND AND BUILD OTHER METHODS
                double r = RNG.NextDouble() * 100;

                if (r >= 60)
                {
                    transform.Translate(Vector3.forward, Space.World);
                }
                else if (r < 60 && r >= 5)
                {
                    //transform.Rotate(0, 90, 0);
                    transform.Translate(Vector3.forward, Space.World);
                }
                else
                {
                    //transform.Rotate(0, 180, 0);
                    transform.Translate(Vector3.forward, Space.World);
                }


                _waitTimer = 0f;

            }
            else
            {
                _waitTimer += 1 * Time.deltaTime;
            }


            // Everytime ant moves, check where he is
            checkWhatBlockAntIsOn();
        }

        // Some code learned from office hours w/Cooper
        // When moving from one black to another, ants are not allowed to move to a
        // block that is greater than 2 units in height difference
        private int[][] checkBlocksHeight(int xCoord, int yCoord, int zCoord)
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
            // TODO: NULL CHECK WHEN PICKING A DIRECTION FROM THIS ARRAY FOR A DIRECTION TO MOVE IN
            int[][] possibleDirections = new int[8][];

            possibleDirections[0] = new int[3];
            possibleDirections[1] = new int[3];
            possibleDirections[2] = new int[3];
            possibleDirections[3] = new int[3];
            possibleDirections[4] = new int[3];
            possibleDirections[5] = new int[3];
            possibleDirections[6] = new int[3];
            possibleDirections[7] = new int[3];

            // Find y coordinate of each neighbour
            for (int i = 0; i < 8; i++)
            {
                neighbourYCoords[i] = WorldManager.Instance.getHeightAt(neighbourXZCoords[i][0], neighbourXZCoords[i][1]);
            }
      
            // Check all the neighbour y coordinates
            for (int i = 0; i < neighbourYCoords.Length; i ++)
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

                    Debug.Log("Ant can move to the block");
                }
                    
                else if (Mathf.Abs(neighbourYCoords[i] - yCoord) > 2)
                    Debug.Log("NO MOVEMENT");
            }

            return possibleDirections;

        }
        #endregion


        #region INTERACT WITH BLOCKS

        private void checkWhatBlockAntIsOn()
        {
            int[] pos = AntPosition.getAntCurrentPosition(transform.position);

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            checkBlocksHeight(x, y, z);

            //Debug.Log("x:" + x + "y"+ y+"z"+z);

            AbstractBlock ab = WorldManager.Instance.GetBlock(x, y, z);

            //Debug.Log("The type of block is " + ab.GetType()); 

            // Always changes
            // TODO: MAKE THIS PROBABILITY PART OF THE "GENOME" 
            digProbability = (float)new System.Random((int)System.DateTime.Now.Ticks).NextDouble() * 100;

            // 20% probability to dig the block
            if (digProbability > 80f)
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
            else { antHealth.standingOnAcidicBlock = false; }


        }


        private void consumeMulch(int xMulchBlock, int yMulchBlock, int zMulchBlock)
        {

            antHealth.eatMulchGainHealth();
            
            
            Debug.Log("ON A MULCH");

            // Replace mulch block with airblock
            WorldManager.Instance.SetBlock(xMulchBlock, yMulchBlock, zMulchBlock, new AirBlock());

        }

        // Remove block from world by digging it
        private void digBlock(AbstractBlock currentBlock, int xBlockToDig, int yBlockToDig, int zBlockToDig)
        {
            if (!currentBlock.GetType().Equals(typeof(Antymology.Terrain.ContainerBlock)))
            {
                WorldManager.Instance.SetBlock(xBlockToDig, yBlockToDig, zBlockToDig, new AirBlock());
            }
        }

        private void checkNeighbours()
        {
            int[] neighbourYCoords = new int[9];

        
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

                // Share health
                // Need reference to antBehaviour script on other ant

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

