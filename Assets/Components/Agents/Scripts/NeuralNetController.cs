﻿using UnityEngine;
using System.Collections;
using Antymology.Helpers;


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

        public static readonly NeuralNetController Instance = new NeuralNetController();

        private NeuralNetController() { }

        // Use this for initialization
        public NeuralNet InitializeFirstNeuralNet()
        {
            // TODO: HARD CODE ONLY FOR NOW, WILL CHANGE WITH ACTUAL NN
            //outputs = new float[7] { 0f, 1f, 2f, 3f, 4f, 5f, 6f };

            NeuralNet net = new NeuralNet(layers);

            return net;
        }

        // Update is called once per frame
        public string runNeuralNet(float[] inputs)
        {
            outputs = net.feedForward(inputs);

            for (int i = 0; i < outputs.Length; i++)
                Debug.Log(outputs[i]);

            float r = Random.Range(0f, 6f);
            int rr = CustomMath.fastfloor((double)r);

            decision = possibleDecisions[rr];

            // NN will give back an output with a larger weight.
            float highestProbability = outputs[0];

            //for (int i = 0; i < outputs.Length; i++)
            //{
            //    if (outputs[i] > highestProbability)
            //    {
            //        highestProbability = outputs[i];
            //        decision = possibleDecisions[i];
            //    }
            //}


            return decision;

        }
    }
}

