using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Antymology.Helpers;
using Antymology.Terrain;
using System.Linq; 

namespace Antymology.AgentScripts
{
    /* 
     * Manages ants actions
     * References at bottom of file
     */
    public class antManager : MonoBehaviour
    {
        private System.Random RNG;

        public AntHealth antHealth;

        private AntHealth otherAntHealth;
        //private antManager otherAntManager;

        private float digProbability = 0.25f;
        private float moveProbability = 0.90f;

        public float _timeToWaitInbetween;
        public float _waitTimer;

        private bool initialized = false;
        private NeuralNet net;

        [SerializeField] string currentBlock;

        private void Awake()
        {
            // Generate new random number generator
            RNG = new System.Random(ConfigurationManager.Instance.Seed);

            antHealth = GetComponent<AntHealth>();

            currentBlock = "airblock";
        }

        private void Start()
        {
            //checkWhatBlockAntIsOn();
        }

        private void Update()
        {
            if (initialized == true)
            {
                float[] inputs = new float[1];
                float m = (float)new System.Random((int)System.DateTime.Now.Ticks).NextDouble();
                inputs[0] = m;

                if (m > (1 - moveProbability))
                    moveAnt();

                float[] output = net.feedForward(inputs);

                m = output[0];

                if (m > (1 - moveProbability))
                    moveAnt();

                // This should be how many nest blocks are produced or maybe queen health
                // Maximize queen health -> more nest blocks
                net.addFitness((1f - Mathf.Abs(inputs[0])));

            }
        }

        public void Init(NeuralNet net)
        {
            this.net = net;
            initialized = true;
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
                int[][] possibleDirections = DirectionFinder.getPossibleDirections(x, y, z);

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
                    //transform.Rotate(new Vector3(x, y, z));

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

        

        #endregion


        #region INTERACT WITH BLOCKS

        private void checkWhatBlockAntIsOn()
        {
            int[] pos = getCurrentWorldXYZAnt();

            int x = pos[0];
            int y = pos[1];
            int z = pos[2];

            DirectionFinder.getPossibleDirections(x, y, z);

            AbstractBlock ab = WorldManager.Instance.GetBlock(x, y, z);

            currentBlock = ab.ToString();

            // Always changes
            // TODO: MAKE THIS PROBABILITY PART OF THE "GENOME" 
            float p = (float)new System.Random((int)System.DateTime.Now.Ticks).NextDouble();

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
                //transform.position = new Vector3(x, y + 1, z);
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
            if (other.CompareTag("ant") || other.CompareTag("queen"))
            {
                antHealth.canEat = false;

                Debug.Log("AN ANT COLLIDED");

                otherAntHealth = other.GetComponent<AntHealth>();

                // Ants may give some of their health to other ants occupying the same space (must be a zero-sum exchange)
                antHealth.shareHealthToAntWithLess(other.tag, otherAntHealth);
            }
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

