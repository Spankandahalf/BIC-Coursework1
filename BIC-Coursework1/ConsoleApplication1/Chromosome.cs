using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    /// <summary>
    /// A chromosome has the ability for mutation and cross-mutation with another chromosome.
    /// A chromosome is designed that it will always have a city as its start and end with one of 
    /// each other city forming a path from start to end.
    /// </summary>
    public class Chromosome : IComparable<Chromosome> {
        private int startPoint;
        private LinkedList<int> path, previousPath;
        private LinkedList<int> alter, alterWith;
        private Random randomNumberGenerator;
        private int fitnessValue, previousFitnessValue;
        private int mutationIndexPoint;
        
        public Chromosome(Random randomNumberGenerator) {
            startPoint = -1;
            path = new LinkedList<int>();
            previousPath = new LinkedList<int>();
            alter = new LinkedList<int>();
            alterWith = new LinkedList<int>();
            this.randomNumberGenerator = randomNumberGenerator;
            fitnessValue = -1;
            previousFitnessValue = -1;
            mutationIndexPoint = -1;
        }

        public void SetStartCity(int startPoint) {
            this.startPoint = startPoint;
        }

        public int GetStartCity() {
            return startPoint;
        }

        public void AddCityToPath(int city) {
            path.AddLast(city);
        }

        /// <summary>
        /// Gives a city from the path. 
        /// Note: First available city is the destination from the start city.
        /// </summary>
        /// <param name="pathIndex">Index of city in path. Note: Starts at 0 and you exclude the start and end city.</param>
        /// <returns>Number of a city.</returns>
        public int GetCityFromPath(int pathIndex) {
            return path.ElementAt(pathIndex);
        }

        /// <summary>
        /// A trip is from one city to another. 
        /// </summary>
        /// <param name="tripNumber">Which trip you want the start city for. 
        /// Note: Starts from 1 not 0.</param>
        /// <returns>City a trip starts from.</returns>
        public int TripStartFrom(int tripNumber) {
            if(tripNumber == 1) {
                return startPoint;
            } 
            
            return path.ElementAt((tripNumber - 2));
        }

        /// <summary>
        /// A trip is from one city to another. 
        /// </summary>
        /// <param name="tripNumber">Which trip you want the destination city for. 
        /// Note: Starts from 1 not 0.</param>
        /// <returns>City a trip ends at.</returns>
        public int TripDestination(int tripNumber) {
            if (tripNumber == (path.Count + 1)) {
                return startPoint;
            }

            return path.ElementAt((tripNumber - 1));
        }

        public void SetFitnessValue(int fitness) {
            fitnessValue = fitness;
        }

        public int getFitness() {
            return fitnessValue;
        }

        public int NumberOfCitiesTravelled() {
            return path.Count + 1;
        }

        /// <summary>
        /// Gives city indexes of a path from one city, 
        /// through all the others and ending at the starting city.
        /// </summary>
        /// <returns>List of city indexes, arranged in path through them. 
        /// Note: If it starts with -1 -> start city not set and 
        /// just shows same number repeated -> path through city not added.</returns>
        public string ViewChromosome() {
            string startToEnd;

            startToEnd = startPoint + ", ";
            foreach(int city in path) {
                startToEnd += city + ", ";
            }
            startToEnd += startPoint;

            return startToEnd;
        }

        /// <summary>
        /// Will alter X number of cells in the chromosome.
        /// </summary>
        /// <param name="numberOfMutations">How many cells to mutate.</param>
        public void Mutation(int numberOfMutations) {
            StoreCurrentConfiguration();

            for (int i = 0; i < numberOfMutations; i++) {
                // Get indexes for alter elements.
                alter.AddLast(randomNumberGenerator.Next(path.Count()));
                alterWith.AddLast(randomNumberGenerator.Next(path.Count()));

                // Check these are not the same value
                // Note: Value of i equals last in both alter and alterWith.
                while(alter.ElementAt(i) == alterWith.ElementAt(i)) {
                    alterWith.RemoveLast();
                    alterWith.AddLast(randomNumberGenerator.Next(path.Count()));
                }

                // Check not undoing a previous mutation.
                if(i > 0) {
                    for(int c = i - 1; c > -1; c--) {
                        if(alter.ElementAt(c) == alter.ElementAt(i)) {
                            while(alterWith.ElementAt(c) == alterWith.ElementAt(i)) {
                                alterWith.RemoveLast();
                                alterWith.AddLast(randomNumberGenerator.Next(path.Count()));
                            }
                            if(alterWith.ElementAt(c) != alterWith.ElementAt(i)) {
                                // This will add to a previous mutation so stop check.
                                c = -1;
                            }
                        }
                    }
                }

                Mutate(alter.ElementAt(i), alterWith.ElementAt(i));
            }

            // Clear, ready to use on next mutation.
            alter.Clear();
            alterWith.Clear();
        }

        /// <summary>
        /// Swap two chromosome cells around to mutate chromosome.
        /// Note: Pre-check inputs are different.
        /// </summary>
        /// <param name="alterIndex">Path index of one cell</param>
        /// <param name="withIndex">Path index of another cell</param>
        private void Mutate(int alterIndex, int withIndex) {
            int alter = path.ElementAt(alterIndex);
            int with = path.ElementAt(withIndex);

            if(alterIndex < withIndex) {
                path.Find(with).Value = alter;
                path.Find(alter).Value = with;
            } else {
                path.Find(alter).Value = with;
                path.Find(with).Value = alter;
            }
        }

        /// <summary>
        /// Cross over mutation is applied to this chromosome, then repairs chromosome to remove 
        /// duplicate cities.
        /// </summary>
        /// <param name="pathSegment">New section of path to replace counterpart of this chromosome.</param>
        /// <param name="mutationPoint">Index point of path where element is to be added at.</param>
        public void CrossMutationApplication(LinkedList<int> pathSegment, int mutationPoint) {
            LinkedList<int> newPath = new LinkedList<int>();
            LinkedList<int> cityCheckList = new LinkedList<int>();

            // Create new path of old path to mutation point then segment from another chromosome.
            for(int i = 0; i < mutationPoint; i++) {
                newPath.AddLast(path.ElementAt(i));
            }
            foreach(int city in pathSegment) {
                newPath.AddLast(city);
            }

            // Form check list to of cities getting of replaced cities to prevent duplicate cities.
            for(int c = mutationPoint; c < path.Count; c++) {
                cityCheckList.AddLast(path.ElementAt(c));
            }

            // Filter out cities that will not have a duplicate in new path.
            for(int di = (cityCheckList.Count - 1); di > -1; di--) {
                foreach(int city in pathSegment) {
                    if(cityCheckList.ElementAt(di) == city) {
                        // No remove at method so change value to -1 to remove city.
                        cityCheckList.Find(city).Value = -1;
                    }
                }
            }

            // Now check section before mutation point for duplicate cities and replace with cities left in check list.
            for(int ai = 0; ai < mutationPoint; ai++) {
                // Check against new cities add after mutation point.
                foreach(int newCity in pathSegment) {
                    // Detect duplicate in path.
                    if(newPath.ElementAt(ai) == newCity) {
                        // Go through city list
                        for(int cli = 0; cli < cityCheckList.Count; cli++) {
                            // Found first available city to replace duplicate.
                            if(cityCheckList.ElementAt(cli) != -1) {
                                // Replace and remove from city list.
                                int changeThis = newPath.ElementAt(ai);
                                int removeThis = cityCheckList.ElementAt(cli);

                                newPath.Find(changeThis).Value = removeThis;
                                cityCheckList.Find(removeThis).Value = -1;

                                // After a change has been made, end the for loop otherwise will replace with all missing values.
                                cli = cityCheckList.Count;
                            }
                        }
                    }
                }
            }

            // Deal with start/end point of this chromosome in pathSegent passed in.
            if(pathSegment.Contains(startPoint)) {
                for (int fci = 0; fci < cityCheckList.Count; fci++) {
                    // Find last available city to replace this duplicate.
                    if (cityCheckList.ElementAt(fci) != -1) {
                        newPath.Find(startPoint).Value = cityCheckList.ElementAt(fci);

                        fci = cityCheckList.Count;
                    }
                }
            }

            StoreCurrentConfiguration();

            // Replace path with new cross over mutation path.
            path.Clear();
            foreach(int city in newPath) {
                path.AddLast(city);
            }
        }

        /// <summary>
        /// Request a section from this chromosome to cross mutate onto another chromosome.
        /// </summary>
        /// <returns>Section of the list of cities forming a path for this chromosome.</returns>
        public LinkedList<int> CrossMutationRequest() {
            StoreCurrentConfiguration();

            LinkedList<int> crossSection = new LinkedList<int>();
            
            // If true, no cross section wait to be swapped into place.
            if (mutationIndexPoint == -1) {
                mutationIndexPoint = randomNumberGenerator.Next(path.Count);

                // I want a decent size section, if I did all but first or just the last then
                // you end up with the same affects of the mutation method on both chromosomes.
                int crossIndexFactor = path.Count / 5;
                while (mutationIndexPoint <= crossIndexFactor || mutationIndexPoint >= (path.Count - crossIndexFactor)) {
                    mutationIndexPoint = randomNumberGenerator.Next(path.Count);
                }
            }

            // Form section to give to another chromosome.
            for(int i = mutationIndexPoint; i < path.Count; i++) {
                crossSection.AddLast(path.ElementAt(i));
            }

            return crossSection;
        }

        /// <summary>
        /// Get a section from this chromosome to match the size of a section from another chromosome.
        /// </summary>
        /// <param name="mutationIndexPoint">Mutation point from other chromosome.</param>
        /// <returns>Matching section from this chromosome for other chromosome.</returns>
        public LinkedList<int> CrossMutationRequest(int mutationIndexPoint) {
            this.mutationIndexPoint = mutationIndexPoint;
            return CrossMutationRequest();
        }

        /// <summary>
        /// The path index point where the cross mutation section was removed from.
        /// </summary>
        /// <returns>Path index point. Note: -1 indicates no cross section has been requested.</returns>
        public int CrossMutationIndexPoint() {
            return mutationIndexPoint;
        }

        /// <summary>
        /// Reset mutationIndexPoint to -1. 
        /// </summary>
        public void ClearMutationIndexPoint() {
            mutationIndexPoint = -1;
        }

        /// <summary>
        /// Stores current path and fitness, this is to allow the addition of the better chromosome, 
        /// pre or post mutation, to the population.
        /// </summary>
        private void StoreCurrentConfiguration() {
            if (fitnessValue != -1) {
                previousFitnessValue = fitnessValue;
                previousPath.Clear();
                foreach (int city in path) {
                    previousPath.AddLast(city);
                }
            }
        }

        /// <summary>
        /// If the mutation applied produces a weaker fitness, this method is used so that the fitness 
        /// chromosome, from pre and post mutation, is add to the population.
        /// </summary>
        public void RevertChromosome() {
            if (previousFitnessValue != -1) {
                fitnessValue = previousFitnessValue;
                path.Clear();
                foreach (int city in previousPath) {
                    path.AddLast(city);
                }
            }
        }
        
        public int CompareTo(Chromosome other) {
            if (this.fitnessValue > other.getFitness())
                return -1;

            if (this.fitnessValue == other.getFitness())
                return 0;

            return 1;
        }

        /// <summary>
        /// As I am using the chromosomes as outputs for neurons, this is added for use by them alone.
        /// </summary>
        public void AlterForNegativeThreshold() {
            StoreCurrentConfiguration();

            int cityOneValue = -1;
            int cityTwoValue = -1;
            for (int swapIndex = 0; swapIndex < path.Count; swapIndex++) {
                if(cityOneValue == 0) {
                    // Swap start point with first destination.
                    cityOneValue = startPoint;
                    cityTwoValue = path.ElementAt(swapIndex);
                    path.Find(cityTwoValue).Value = cityOneValue;
                    startPoint = cityTwoValue;
                } else if(swapIndex + 1 != path.Count) {
                    // Swap city with next. 
                    cityOneValue = path.ElementAt(swapIndex);
                    cityTwoValue = path.ElementAt(swapIndex + 1);
                    path.Find(cityTwoValue).Value = cityOneValue;
                    path.Find(cityOneValue).Value = cityTwoValue;
                    swapIndex++;
                } else {
                    /// Swap city with previous as last city in path and it has not been swapped.
                    cityOneValue = path.ElementAt(swapIndex);
                    cityTwoValue = path.ElementAt(swapIndex - 1);
                    path.Find(cityTwoValue).Value = cityOneValue;
                    path.Find(cityOneValue).Value = cityTwoValue;
                }
            }
        }
    }
}
