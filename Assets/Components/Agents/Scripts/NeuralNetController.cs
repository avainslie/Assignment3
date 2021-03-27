using UnityEngine;
using System.Collections;
using Antymology.Helpers;


namespace Antymology.AgentScripts
{
    public class NeuralNetController : Singleton<NeuralNetController>
    {


        // Get inputs from ants
        // Run nn
        // Compare nn
        // Keep & discard nn
        // Mutate nn
        // Send outputs back to ants

        public string decision = "moveF";

        private string[] possibleDecisions = { "moveF", "moveB", "moveR", "moveL", "nothing", "dig", "eat" };

        private float[] outputs;

        public float _timeToWaitInbetween = 10f;
        public float _waitTimer = 0f;

        // INPUTS
        public float distToQueen;
        public float xCoord;
        public float yCoord;
        public float zCoord;
        public float currentHealth;
        public float queensHealth;

        private bool initialized = false; 
        private NeuralNet net;

        // Use this for initialization
        void Awake()
        {
            // TODO: HARD CODE ONLY FOR NOW, WILL CHANGE WITH ACTUAL NN
            outputs = new float[7] { 0f, 1f, 2f, 3f, 4f, 5f, 6f };

        }

        // Update is called once per frame
        public void Update()
        {
            if (_waitTimer >= _timeToWaitInbetween)
            {
                


                float r = Random.Range(0f, 6f);
                int rr = CustomMath.fastfloor((double)r);

                decision = possibleDecisions[rr];

                // NN will give back an output with a larger weight.
                //float highestProbability = outputs[0];

                //for (int i = 0; i < outputs.Length; i++)
                //{
                //    if (outputs[i] > highestProbability)
                //    {
                //        highestProbability = outputs[i];
                //        decision = possibleDecisions[i];
                //    }
                //}

                _waitTimer = 0f;
            }
            else { _waitTimer += 1 * Time.deltaTime; }


        }
    }
}

