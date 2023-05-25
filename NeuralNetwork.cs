using System;
public class NeuralNetwork
{
    //Neuron in a layer
    private class Neuron
    {
        //Current value of the layer, can be modified
        public double value { get; set; } = 0;
        //Weights connected to other neurons
        private Weight[] weights { get; set; } = new Weight[0];
        //Method used when initializing a zeored neural net, connect this neuron to each neuron in passed layer 
        public void CreateWeightsFromSelfToEachNeuronInNextLayer(Neuron[] nextLayer)
        {
            //Create weights
            int neuronCountInNextLayer = nextLayer.Length;
            weights = new Weight[neuronCountInNextLayer];
            
            //Apply values
            for(int neuronIndexInNextLayer = 0; neuronIndexInNextLayer < neuronCountInNextLayer; neuronIndexInNextLayer++)
            {
                Weight newWeight = new Weight(this, nextLayer[neuronIndexInNextLayer]);
                weights[neuronIndexInNextLayer] = newWeight;
            }
        }

        //Add self value * weight value to each connected neuron
        public void UpdateConnectedNeuronsValues()
        {
            foreach(Weight weight in weights)
            {
                weight.connectedTo.value += value*weight.value;
            }
        }
    }

    //Weight is connected from to another neuron
    private class Weight
    {
        public double value { get; set; } = 0;
        public Neuron connectedTo { get; set; }
        public Neuron connectedFrom { get; set; }
        //We always use this constructor, connectedTo and connectedFrom can not be null. 
        public Weight(Neuron from, Neuron to)
        {
            if (from == null || to == null)
                throw new NullReferenceException("From or to neuron can not be null! Weight must be connected to two neurons.");
            connectedFrom = from;
            connectedTo = to;
        }
    }
    
    //The neural network
    public class NeuralNet
    {
        //Net's layers, layers[layerIndex][neuronIndex]
        private Neuron[][] layers;
        
        //This constructor initializes a zeroed neural net (all neurons and weighs values are set to 0)
        public NeuralNet(int[] neuronLayers)
        {
            //Exception checks
            if(Array.Exists(neuronLayers, element => element <= 0))
                throw new Exception("Layer can not have less than one neuron!");
            if (neuronLayers.Length <= 1)
                throw new Exception("NeuralNet must have more than one layer! At least input and output layers.");

            //Create layers
            layers = new Neuron[neuronLayers.Length][];

            //Resize layers and populate them with empty neurons
            for(int layerIndex = 0; layerIndex < layers.Length; layerIndex++)
            {
                //Resizing layer
                Neuron[] neuronsInLayer = new Neuron[neuronLayers[layerIndex]];
                layers[layerIndex] = neuronsInLayer;

                //Populating with empty neurons
                for(int neuronIndexInLayer = 0; neuronIndexInLayer < neuronsInLayer.Length; neuronIndexInLayer++) 
                {
                    neuronsInLayer[neuronIndexInLayer] = new Neuron();
                }
            }

            //Create weights between neurons
            for(int layerIndex = 0; layerIndex < layers.Length - 1; layerIndex++)
            {
                for(int neuronIndexInLayer = 0; neuronIndexInLayer < layers[layerIndex].Length; neuronIndexInLayer++)
                {
                    layers[layerIndex][neuronIndexInLayer].CreateWeightsFromSelfToEachNeuronInNextLayer(layers[layerIndex + 1]);
                }
            }
        }

        //Updates the input layer, values length must be the same as input layer's length
        public void UpdateInputLayerValues(double[] values)
        {
            Neuron[] inputLayer = layers[0];
            //Exception check
            if(values.Length != inputLayer.Length)
            {
                throw new Exception("Length of 'values' array must match the input layer's neurons count!");
            }
            //Apply values to input layer
            for(int neuronIndex = 0; neuronIndex < inputLayer.Length; neuronIndex++)
            {
                inputLayer[neuronIndex].value = values[neuronIndex];
            }
        }
        //Provides a double array of output layer neuron's values
        public double[] GetOutputLayerValues()
        {
            Neuron[] outputLayer = layers[layers.Length - 1];
            double[] outputLayerValues = new double[outputLayer.Length];
            for(int neuronIndex = 0; neuronIndex < outputLayer.Length; neuronIndex++)
            {
                outputLayerValues[neuronIndex] = outputLayer[neuronIndex].value;
            }
            return outputLayerValues;
        }
        //Updates the network using Neuron.UpdateConnectedNeuronsValues
        public void UpdateNeuronsValues()
        {
            for(int layerIndex = 0; layerIndex < layers.Length - 1; layerIndex++)
            {
                for(int neuronIndexInLayer = 0; neuronIndexInLayer < layers[layerIndex].Length; neuronIndexInLayer++)
                {
                    layers[layerIndex][neuronIndexInLayer].UpdateConnectedNeuronsValues();
                }
            }
        }
    }
}
