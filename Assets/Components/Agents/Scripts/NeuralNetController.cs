using UnityEngine;
using System.Collections;
using Antymology.Helpers;
using System.Collections.Generic;
using System.IO;
using System;

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

        string fpath = @"D:\TestUnityAssingment3IAMHERE.txt";

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

        // Always pass in current net as n1 and most recent previous net as n2
        private NeuralNet pickTheBestNet(NeuralNet n1, NeuralNet n2)
        {
            if (n1.CompareTo(n2) == 1)
            {
                nets.Remove(nets[nets.Count - 2]);
                return n1;
            }  
            else if (n1.CompareTo(n2) == -1)
            {
                nets.Remove(nets[nets.Count - 1]);
                return n2;
            }
            else
            {
                nets.Remove(nets[nets.Count - 2]);
                return n1;
            }
                
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


        public void WriteToFile(NeuralNet toWrite)
        {

            if (File.Exists(fpath))

            {

                File.Delete(fpath);

            }


            BinaryWriter writer = new BinaryWriter(File.Open(fpath, FileMode.Create));

            

            //writer.Write(each neuron of each layer);
        }

        public void ReadFile()
        {
            try
            {

            }

            catch (Exception e)
            {
                Debug.Log("file didn't exist" + e.StackTrace);
            }
        }



    }
}

