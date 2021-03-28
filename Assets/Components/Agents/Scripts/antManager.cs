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

        // OUTPUTS
        private float moveF;
        private float moveB;
        private float moveR;
        private float moveL;
        private float nothing;
        private float dig;
        private float eat;
        // shareHealth

        [SerializeField] string currentBlock;

        public float antTimeToWaitInbetween = 10f;
        public float antWaitTimer = 0f;

        private void Awake()
        {
            // Generate new random number generator
            RNG = new System.Random(ConfigurationManager.Instance.Seed);

            antHealth = GetComponent<AntHealth>();

            queen = GameObject.FindGameObjectWithTag("queen");
            queenBehaviour = queen.GetComponent<queenBehaviour>();
        }

        private void Update()
        {

            if (antWaitTimer >= antTimeToWaitInbetween)
            {
                int[] pos = antAndQueenController.Instance.getCurrentWorldXYZAnt(gameObject);

                int xCoord = pos[0];
                int yCoord = pos[1];
                int zCoord = pos[2];

                AbstractBlock ab = WorldManager.Instance.GetBlock(xCoord, yCoord, zCoord);

                currentBlock = ab.ToString();

                distToQueen = calculateDistToQueen();

                currentHealth = antHealth.health;
                
                queensHealth = queenBehaviour.queenAntHealth.health;

                inputs = new float[6] { distToQueen, xCoord, yCoord, zCoord, currentHealth, queensHealth };

                // Get output from the current neural net
                string decision = NeuralNetController.Instance.runNeuralNet(inputs);

                switch (decision)
                {
                    case "moveF":
                        moveAnt("F");
                        Debug.Log("MOVEF");
                        
                        break;

                    case "moveB":
                        moveAnt("B");
                        Debug.Log("MOVEB");
                        transform.Rotate(new Vector3(0, 180, 0));
                        break;

                    case "moveR":
                        moveAnt("R");
                        Debug.Log("MOVER");
                        transform.Rotate(new Vector3(0, 90, 0));
                        break;

                    case "moveL":
                        moveAnt("L");
                        Debug.Log("MOVEL");
                        transform.Rotate(new Vector3(0, -90, 0));
                        break;

                    case "nothing":
                        Debug.Log("NOTHING");
                        break;

                    case "dig":
                        Debug.Log("DIG");
                        digBlock(ab, xCoord, yCoord - 1 ,zCoord);
                        break;

                    case "eat":
                        Debug.Log("EAT");
                        consumeMulch(xCoord, yCoord, zCoord);
                        break;
                }
                    antWaitTimer = 0f;
            }
            else { antWaitTimer += 1 * Time.deltaTime; }

        }

        #region MOVEMENT

        public void moveAnt(string move)
        {
            int[] pos = antAndQueenController.Instance.getCurrentWorldXYZAnt(gameObject);

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
                }
                
            }

            else
            {
                switch (move)
                {
                    case "R":
                        if (DirectionFinder.validXNeighbourHeight(x + 1, y, z))
                            transform.position = new Vector3(x + 1, WorldManager.Instance.getHeightAt(x + 1, z) , z);
                        break;
                    case "L":
                        if (DirectionFinder.validXNeighbourHeight(x - 1, y, z))
                            transform.position = new Vector3(x - 1, WorldManager.Instance.getHeightAt(x + 1, z), z);
                        break;
                }
            }

            
        }

        #endregion

        

        #region INTERACT WITH BLOCKS

        // Remove block from world by digging it
        private void digBlock(AbstractBlock currentBlock, int xBlockToDig, int yBlockToDig, int zBlockToDig)
        {
            if ((currentBlock as ContainerBlock) == null)
            {
                WorldManager.Instance.SetBlock(xBlockToDig, yBlockToDig, zBlockToDig, new AirBlock());
                //moveAntDownOne();
            }
        }


        private void consumeMulch(int x, int y, int z)
        {
            if ((WorldManager.Instance.Blocks[x, y - 1, z] as MulchBlock) != null)
            {
                antHealth.standingOnAcidicBlock = false;
                antHealth.eatMulchGainHealth();
                // Replace mulch block with airblock
                WorldManager.Instance.SetBlock(x, y, z, new AirBlock());
                //moveAntDownOne();
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
                antHealth.canEat = false;

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

