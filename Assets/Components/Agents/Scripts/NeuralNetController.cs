﻿using UnityEngine;
using System.Collections;
using Antymology.Helpers;
using System.Collections.Generic;
using System.IO;
using System;
using System.Runtime.Serialization.Formatters.Binary;

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

        private float[][] neurons;

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

        public NeuralNet InitializeNeuralNetFromFile()
        {
            float[][][] weights = ReadFile();
            net = new NeuralNet(layers, weights);

            nets = new List<NeuralNet>();
            Debug.Log("NET INITIALIZED from file");
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
                if (nets.Count > 2)
                    nets.Remove(nets[nets.Count - 2]);
                return n1;
            }  
            else if (n1.CompareTo(n2) == -1)
            {
                if (nets.Count > 2)
                    nets.Remove(nets[nets.Count - 1]);
                return n2;
            }
            else
            {
                if (nets.Count > 2)
                    nets.Remove(nets[nets.Count - 2]);
                return n1;
            }
                
        }

        // Compare the current and most recent previous nets
        public void compareNetsAndMutateBest()
        {
            WriteToFile(net);

            // Second last element
            // https://stackoverflow.com/questions/22857137/how-to-find-second-last-element-from-a-list/22857667
            NeuralNet mostRecentPreviousNet = null;
            if (nets.Count > 2)
                mostRecentPreviousNet = nets[nets.Count - 2];

            net = pickTheBestNet(net, mostRecentPreviousNet);

            net.mutateWeightsInMatrix();

            Debug.Log("compareNetsAndMutateBest");
        }

        // https://www.tutlane.com/tutorial/csharp/csharp-binarywriter
        // https://answers.unity.com/questions/1300019/how-do-you-save-write-and-load-from-a-file.html
        public void WriteToFile(NeuralNet toWrite)
        {

            using (BinaryWriter writer = new BinaryWriter(File.Create(fpath)))
            {
                // Write each weight of each neuron in all layers

                for (int i = 0; i < toWrite.weights.Length; i++)
                {
                    for (int j = 0; j < toWrite.weights[i].Length; j++)
                    {
                        for (int k = 0; k < toWrite.weights[i][j].Length; k++)
                            writer.Write(toWrite.weights[i][j][k]);
                    }
                }

            }
        }

        private float[][] initNeurons()
        {
            List<float[]> neuronsList = new List<float[]>();

            // for each layer in layers, add layer to neuron list
            for (int i = 0; i < layers.Length; i++)
                neuronsList.Add(new float[layers[i]]);
            neurons = neuronsList.ToArray();

            return neurons;
        }

        public float[][][] ReadFile()
        {
            float[][] neurons = initNeurons();
            float[][][] weights = new float[layers.Length][][];

            if (File.Exists(fpath))
            { 
                using (BinaryReader reader = new BinaryReader(File.Open(fpath, FileMode.Open)))
                {

                    List<float[][]> weightsList = new List<float[][]>();

                    // Each hidden layer will need a weight matrix
                    for (int i = 1; i < layers.Length; i++)
                    {
                        List<float[]> layerWeightsList = new List<float[]>();

                        int neuronsInPreviousLayer = layers[i - 1];

                        // For each neuron in the current layer
                        for (int j = 0; j < neurons[i].Length; j++)
                        {
                            // All the connections of the current neuron
                            float[] neuronWeights = new float[neuronsInPreviousLayer];

                            for (int k = 0; k < neuronsInPreviousLayer; k++)
                                neuronWeights[k] = reader.ReadByte();

                            layerWeightsList.Add(neuronWeights);
                        }
                        weightsList.Add(layerWeightsList.ToArray());
                    }
                    weights = weightsList.ToArray(); // Create jagged array from our list
                }


                return weights;
            }
            else
                Debug.Log("FILE NOT FOUND");
            

            return null;

        }



    }
}

