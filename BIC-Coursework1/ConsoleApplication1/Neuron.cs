using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    public class Neuron {
        private int typeOfNeuron; // 1 -> Input, 2 -> Hidden/Output.
        private int numberOfInputs;
        private int[] inputs; 
        private double[] inputWeight;
        private bool[] inputAvailable;
        private Random numberGenerator;
        private FitnessChecker checker;
        private double threshold;
        private Chromosome output;
        private Chromosome[] pathInputs;

        public Neuron(FitnessChecker checker, int type) {
            typeOfNeuron = type;
            numberOfInputs = -1;  // -1 means unset.
            numberGenerator = new Random();
            this.checker = checker;
        }

        public void SetNumberOfInputs(int amount) {
            numberOfInputs = amount;
            SetupInputAndInputWieght();
        }

        private void SetupInputAndInputWieght() {
            if (typeOfNeuron == 1) {
                inputs = new int[numberOfInputs];
                inputAvailable = new bool[numberOfInputs];
            } else if (typeOfNeuron == 2) {
                pathInputs = new Chromosome[numberOfInputs];
            }

            inputWeight = new double[numberOfInputs];
        }

        public int NumberOfInputs() {
            return numberOfInputs;
        }

        /// <summary>
        /// Add a certain input to the neuron.
        /// </summary>
        /// <param name="whichInputNumber">Identify the input which is getting added.</param>
        /// <param name="input">For input neuron: city OR for hidden/output neuron: chromosome (path).</param>
        public void EnterInput(int whichInputNumber, Object input) {
            if(typeOfNeuron == 1) {
                EnterCityInput(whichInputNumber, (int)input);
            } else {
                EnterPathInput(whichInputNumber, (Chromosome)input);
            }
        }

        /// <summary>
        /// Add city as input to neuron, input layer only. 
        /// Note: Initial weight for input is random double between 0 and number of inputs.
        /// </summary>
        /// <param name="whichInputNumber">Input to assign value to.</param>
        /// <param name="city">The city index to apply as input value.</param>
        private void EnterCityInput(int whichInputNumber, int city) {
            inputs[whichInputNumber] = city;
            inputWeight[whichInputNumber] = numberGenerator.Next(numberOfInputs);
            inputAvailable[whichInputNumber] = true;
        }

        private void EnterPathInput(int whichInputNumber, Chromosome path) {
            pathInputs[whichInputNumber] = path;
            inputWeight[whichInputNumber] = numberGenerator.Next(numberOfInputs);
        }

        /// <summary>
        /// Once all the inputs have been entered, use this to prepare the output of the neuron.
        /// </summary>
        public void PrepareOutputForInputNeuron() {
            int lowestIndex = 0;
            double lowestWeight = inputWeight[0];
            
            // Get input with lowest weight.
            for(int startIndex = 1; startIndex < inputs.Count(); startIndex++) {
                if(inputWeight[startIndex] < lowestWeight) {
                    lowestWeight = inputWeight[startIndex];
                    lowestIndex = startIndex;
                }
            }

            // Set start/end point of journey.
            output.SetStartCity(inputs[lowestIndex]);
            inputAvailable[lowestIndex] = false;

            // Add the rest of the cities to output.
            int secondLowestIndex = -1;
            double secondLowestWeight = -1;

            while(output.NumberOfCitiesTravelled() < numberOfInputs) {
                for (int index = 1; index < inputs.Count(); index++) {
                    // Check second lowest weight has a value.
                    if (secondLowestWeight == -1 && inputAvailable[index] == true) {
                        secondLowestWeight = inputWeight[index];
                        secondLowestIndex = index;
                    // Change second lowest weight if lower value found (which is not -1)
                    } else if (inputAvailable[index] == true && inputWeight[index] < secondLowestWeight) {
                        secondLowestWeight = inputWeight[index];
                        secondLowestIndex = index;
                    }
                    // Found lowest value, end for loop.
                    if(lowestWeight == secondLowestWeight) {
                        index = inputs.Count() + 9001;
                    }
                }

                // Update lowest value with new current lowest values
                lowestIndex = secondLowestIndex;
                lowestWeight = secondLowestWeight;
                secondLowestIndex = -1;
                secondLowestWeight = -1;

                // Add to output
                output.AddCityToPath(inputs[lowestIndex]);
                inputAvailable[lowestIndex] = false;
            }

            // With output complete, get it's fitness
            output = checker.ProduceFitnessOfChromosome(output);

            // Set up Threshold value
            threshold = (checker.FitnessOfTwoCities(inputs[0], inputs[1]) * numberOfInputs);
        }

        public void PrepareOutputForHiddenOutputNeuron() {

        }

        public Chromosome ProduceOutput() {
            if(output.getFitness() > threshold) {
                // Alter output.
            }

            return output;
        }
    }
}
