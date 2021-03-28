using System;
using System.Collections.Generic;

// Reference: https://youtu.be/Yq0SfuiOVYE
// Unsupervised neural network
namespace Antymology.AgentScripts
{
    public class NeuralNet : IComparable<NeuralNet>
    {

        private int[] layers;
        private float[][] neurons;
        private float[][][] weights;
        private float fitness;

        #region INITIALIZATION

        // To create neurons and weights we need to initialize the layers
        public NeuralNet(int[] layers)
        {
            this.layers = new int[layers.Length];
            for (int i = 0; i < layers.Length; i++)
                this.layers[i] = layers[i];

            initializeNeurons();
            initializeWeights();
        }

        // Deep copy - copy everything that can be referenced so original is not affected
        public NeuralNet(NeuralNet copyNeuralNet)
        {
            this.layers = new int[copyNeuralNet.layers.Length];
            for (int i = 0; i < copyNeuralNet.layers.Length; i++)
                this.layers[i] = copyNeuralNet.layers[i];

            initializeNeurons();
            initializeWeights();
            copyWeights(copyNeuralNet.weights);
        }

        private void copyWeights(float[][][] copyWeights)
        {
            for (int i = 0; i< weights.Length; i++)
            {
                for (int j=0; j<weights[i].Length;j++)
                {
                    for (int k = 0; k < weights[i][j].Length; k++)
                        weights[i][j][k] = copyWeights[i][j][k];
                }
            }
        }

        public void addFitness(float fitnessToAdd)
        {
            fitness += fitnessToAdd;
        }

        public void setFitness(float fitness)
        {
            this.fitness = fitness;
        }

        public float getFitness()
        {
            return fitness;
        }

        // Neurons are a jagged array, float[][] neurons = new float [layers.length][];
        private void initializeNeurons()
        {
            List<float[]> neuronsList = new List<float[]>();

            // for each layer in layers, add layer to neuron list
            for (int i = 0; i < layers.Length; i++)
                neuronsList.Add(new float[layers[i]]);
            neurons = neuronsList.ToArray();
        }

        private void initializeWeights()
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

                    // For each of those connections, give them a random weight
                    // between -0.5 and 0.5
                    for (int k = 0; k < neuronsInPreviousLayer; k++)
                        neuronWeights[k] = UnityEngine.Random.Range(-0.5f,0.5f);

                    layerWeightsList.Add(neuronWeights);
                }
                weightsList.Add(layerWeightsList.ToArray());
            }
            weights = weightsList.ToArray(); // Create jagged array from our list
        }

        #endregion


        #region ACTIONS
        public float[] feedForward(float[] inputs)
        {
            // Put inputs into input layer in neuron matrix
            for (int i = 0; i < inputs.Length; i++)
                neurons[0][i] = inputs[i];

            // Iterate over every layer starting from 2nd layer
            for (int i =1 ; i < layers.Length; i++)
            {
                // Go through every neuron in layers
                for (int j = 0; j < neurons[i].Length; j++)
                {
                    float sumOfAllNeuronWeights = 0.25f; // Constant bias of 0.25

                    // Go through all neurons in previous layer and get the sum
                    // of all weight connections of current neuron and their values in the previous layer
                    for (int k = 0; k < neurons[i - 1].Length; k++)
                        // weights[i-1] since we start at 2nd layer, j = current
                        // neuron, k = previous neuron * value at previous neuron
                        sumOfAllNeuronWeights += weights[i - 1][j][k] * neurons[i - 1][k];

                    // Apply an activation, convert sumOfPreviousNeuronsWeights
                    // between -1 and 1. Gives us the new value of the current neuron
                    // This is the feedforward value
                    neurons[i][j] = (float) Math.Tanh(sumOfAllNeuronWeights);
                }
            }

            // Return the output layer, aka the last layer
            return neurons[neurons.Length - 1];
        }


        public void mutateWeightsInMatrix()
        {
            // Go through all layers
            for (int i = 0; i < weights.Length; i++)
            {
                // Go through all neurons per layer
                for (int j = 0; j < weights[i].Length; j++)
                {
                    // Go through all connections neuron is connected to in previous layer
                    for (int k = 0; k < weights[i][j].Length; k++)
                    {
                        float weight = weights[i][j][k];

                        //mutate weight value 
                        float randomNumber = UnityEngine.Random.Range(0f, 100f);

                        // 40% chance a mutation will occur
                        // Each mutation has a 10% chance

                        if (randomNumber <= 10f)
                        { 
                          //flip sign of weight
                            weight *= -1f;
                        }
                        else if (randomNumber <= 20f)
                        { 
                          //pick random weight between -0.5 and 0.5
                            weight = UnityEngine.Random.Range(-0.5f, 0.5f);
                        }
                        else if (randomNumber <= 30f)
                        { 
                          //randomly increase by 0% to 100%
                            float factor = UnityEngine.Random.Range(0f, 1f) + 1f;
                            weight *= factor;
                        }
                        else if (randomNumber <= 40f)
                        { 
                          //randomly decrease by 0% to 100%
                            float factor = UnityEngine.Random.Range(0f, 1f);
                            weight *= factor;
                        }

                        weights[i][j][k] = weight;
                    }
                }
            }
        }

        // Helps to sort neural nets by their fitness in ascending fitness order
        public int CompareTo(NeuralNet otherNeuralNet)
        {
            if (otherNeuralNet == null) return 1;

            if (fitness > otherNeuralNet.fitness) return 1;

            else if (fitness < otherNeuralNet.fitness) return -1;
            
            else return 0;
        }

        #endregion

    }
}