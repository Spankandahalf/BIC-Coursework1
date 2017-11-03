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
        private bool setThreshold;
        private Chromosome output;
        private Chromosome[] pathInputs;

        public Neuron(FitnessChecker checker, int type) {
            typeOfNeuron = type;
            numberOfInputs = -1;  // -1 means unset.
            numberGenerator = new Random();
            this.checker = checker;
            setThreshold = true;
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

        public double GetWeightForInput(int inputIndex) {
            return inputWeight[inputIndex];
        }

        /// <summary>
        /// After initial use, an ANN updates it's weights to improve them and lead to better results. 
        /// </summary>
        /// <param name="updateWith">What will be added to the current weight value.</param>
        /// <param name="whichWeight">Which input weight to update.</param>
        public void UpdateInputWeight(double updateWith, int whichWeight) {
            double weight = inputWeight[whichWeight];
            weight += updateWith;
            inputWeight[whichWeight] = weight;
        }

        public double ThresholdValue() {
            return threshold;
        }

        /// <summary>
        /// After initial use, an ANN updates a neuron's threshold to improve them and lead to better results. 
        /// </summary>
        /// <param name="updateWith">What to add to current threshold to improve it.</param>
        public void UpdateThreshold(double updateWith) {
            threshold += updateWith;
        }

        /// <summary>
        /// Once all the inputs have been entered, use this to prepare the output of the neuron.
        /// </summary>
        public void PrepareOutputForInputNeuron() {
            int lowestIndex = 0;
            double lowestWeight = inputWeight[0];
            ResetAvailabilities();
            
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

            // Set up Threshold value if needed.
            if (setThreshold) {
                threshold = (checker.FitnessOfTwoCities(inputs[0], inputs[1]) * numberOfInputs);
                setThreshold = false;
            }
        }

        private void ResetAvailabilities() {
            for(int i = 0; i < inputAvailable.Count(); i++) {
                inputAvailable[i] = false;
            }
        }

        public void PrepareOutputForHiddenOutputNeuron() {
            double totalWeight = 0.00;

            // Workout the total weight of all inputs.
            foreach(double weight in inputWeight) {
                totalWeight += weight;
            }

            double inputIndexRef = 0;
            double changeRef = -1;
            // Using weights to produce percentage of input to add to output. i.e. first input works out at 50% -> first half of the path comes from it.
            for(int inputCount = 0; inputCount < pathInputs.Count(); inputCount++) {
                // Work out where to end taking from, work out the weight percentage out of all inputs then multiple by number of cities in path.
                // Add that to start point to find end for that path.
                changeRef = Math.Round(inputIndexRef + ((inputWeight[inputCount] / totalWeight) * pathInputs[inputCount].NumberOfCitiesTravelled()));

                // Add start city if starting loop.
                if(inputCount == 0) {
                    output.SetStartCity(pathInputs[0].GetStartCity());
                    // Reduce end index by one to account for the addition of start city.
                    changeRef--;
                // Last input, the method of taking from inputs may not be 100% each time so will make sure all cities are added.
                } else if ((inputCount + 1) == pathInputs.Count()) {
                    changeRef = pathInputs[inputCount].NumberOfCitiesTravelled() - 1; // Cities minus start city (as stored separately).
                }

                // Add cities to path of output up to it's weighted allotment.
                for(int chromeosomeIndex = (int)inputIndexRef; chromeosomeIndex < changeRef; chromeosomeIndex++) {
                    output.AddCityToPath(pathInputs[inputCount].GetCityFromPath(chromeosomeIndex));
                }

                // Prepare for next input.
                inputIndexRef = changeRef;
            }

            // Set up Threshold value if needed.
            if (setThreshold) {
                threshold = (checker.FitnessOfTwoCities(inputs[0], inputs[1]) * numberOfInputs);
                setThreshold = false;
            }
        }

        public Chromosome ProduceOutput() {
            if(output.getFitness() > threshold) {
                output.AlterForNegativeThreshold();
            }

            return output;
        }
    }
}
