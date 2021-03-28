using UnityEngine;
using System.Collections;
using Antymology.Helpers;
using System.Collections.Generic;

namespace Antymology.AgentScripts
{
    // https://answers.unity.com/questions/1408574/destroying-and-recreating-a-singleton.html 
    public sealed class NeuralNetController 
    {

        /// <summary>
        /// 6 inputs, 1 hidden layer w/6 neurons, and 7 outputs. 
        /// https://stats.stackexchange.com/questions/181/how-to-choose-the-number-of-hidden-layers-and-nodes-in-a-feedforward-neural-netw
        /// </summary>
        private int[] layers = new int[] { 6, 6, 7 };

        public bool isTraining = false;

        // Get inputs from ants
        // Run nn
        // Compare nn
        // Keep & discard nn
        // Mutate nn
        // Send outputs back to ants

        public string decision = "moveF";

        private string[] possibleDecisions = { "moveF", "moveB", "moveR", "moveL", "nothing", "dig", "eat" };

        private float[] outputs = new float[7];

        private float[] inputs;

        // INPUTS
        public float distToQueen;
        public float xCoord;
        public float yCoord;
        public float zCoord;
        public float currentHealth;
        public float queensHealth;

        private bool initialized = false;

        // Used by ants
        public NeuralNet net;

        public List<NeuralNet> nets;

        public static readonly NeuralNetController Instance = new NeuralNetController();

        private NeuralNetController() { }

        // Use this for initialization
        public NeuralNet InitializeFirstNeuralNet()
        {
            net = new NeuralNet(layers);
            nets = new List<NeuralNet>();
            Debug.Log("FIRST NET INITIALIZED");

            return net;
        }

        // Update is called once per frame
        public string runNeuralNet(float[] inputs)
        {
            outputs = net.feedForward(inputs);
            // TODO: SOME OUTPUTS ARE NEGATIVE, IS THIS OK???

            // NN will give back an output with a larger weight.
            float highestProbability = outputs[0];

            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > highestProbability)
                {
                    highestProbability = outputs[i];
                    decision = possibleDecisions[i];
                }
            }
            return decision;
        }

        private NeuralNet pickTheBestNet(NeuralNet n1, NeuralNet n2)
        {
            if (n1.CompareTo(n2) == 1)
                return n1;
            else if (n1.CompareTo(n2) == -1)
                return n2;
            else
                return n1;
        }

        // Compare the current and most recent previous nets
        public void compareNetsAndMutateBest()
        {
            // Second last element
            // https://stackoverflow.com/questions/22857137/how-to-find-second-last-element-from-a-list/22857667
            NeuralNet mostRecentPreviousNet = null;
            if (nets.Count > 2)
                mostRecentPreviousNet = nets[nets.Count - 2];

            net = pickTheBestNet(net, mostRecentPreviousNet);

            net.mutateWeightsInMatrix();

            Debug.Log("compareNetsAndMutateBest");
        }



    }
}

