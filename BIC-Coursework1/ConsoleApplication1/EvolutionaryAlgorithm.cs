using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    public class EvolutionaryAlgorithm {
        private FitnessChecker checker;
        private List<Chromosome> population;
        private LinkedList<Chromosome> newPopulation;
        private int bestFitness, worseFitness, rangeOfFitness;
        private double crossMuttionCutOff, mutationDivValue;
        private double crossMutationFactor, mutationFactor;

        public EvolutionaryAlgorithm(FitnessChecker checker) {
            this.checker = checker;
            newPopulation = new LinkedList<Chromosome>();

            crossMutationFactor = 0.6; // 60%
            mutationFactor = 0.1;  // 10%
        }

        public void ResetEvolustionaryFactors() {
            crossMutationFactor = 0.6; // 60%
            mutationFactor = 0.1;  // 10%
        }

        /// <summary>
        /// Evolve a population of chromosomes.
        /// </summary>
        /// <param name="population">Population of chromosomes to be evolved.</param>
        /// <param name="crossMutationFactor">Percentage from worse of population to apply cross-mutation to, i.e.: default is 0.6 (60%).</param>
        /// <param name="mutationFactor">The percentage of range from best fitness a cell is mutated for, i.e.: default 0.1 (10%).</param>
        /// <returns>New evolved population, same size as previous.</returns>
        public List<Chromosome> EvolvePopulation(List<Chromosome> population, double crossMutationFactor, double mutationFactor) {
            this.crossMutationFactor = crossMutationFactor;
            this.mutationFactor = mutationFactor;

            return EvolvePopulation(population);
        }

        /// <summary>
        /// Evolve a population of chromosomes.
        /// Note: Cross mutation 60% of population from worse and mutate a cell for every 10% of range from best fitness. 
        /// </summary>
        /// <param name="population">Population of chromosomes to be evolved.</param>
        /// <returns>New evolved population, same size as previous.</returns>
        public List<Chromosome> EvolvePopulation(List<Chromosome> population) {
            this.population = population;
            Chromosome travelPlan = new Chromosome();
            Chromosome secondTravelPlan = new Chromosome();

            // Find out range of population and workout required value for evolving.
            bestFitness = population.ElementAt(0).getFitness();
            worseFitness = population.ElementAt(population.Count - 1).getFitness();
            rangeOfFitness = worseFitness - bestFitness; // Best will be the lower value as want the shortest distance travelled.
            
            mutationDivValue = rangeOfFitness * mutationFactor;
            crossMuttionCutOff = worseFitness - (rangeOfFitness * (1 - crossMutationFactor)); // 1 - factor as want that percentage from the worse fitness. 

            // Variables needed for cell mutation.
            int cellsToMutate = 0;
            int preMutationFitness = 0;
            // Variable needed for cross-mutation.
            LinkedList<int> temp;
            int tempIndex = 0;
            int preCrossMutationFitnessOfSecond = 0;

            for (int count = population.Count - 1; count > -1; count--) {
                travelPlan = population.ElementAt(count);
                preMutationFitness = travelPlan.getFitness();
                // Determine if cross-mutation or individual cell mutation should be applied.
                if (preMutationFitness < crossMuttionCutOff) {
                    IndividualCellMutationOption(cellsToMutate, preMutationFitness, travelPlan);
                } else {
                    // Cross-mutation option.

                    // Set up second chromosome to cross mutate with.
                    secondTravelPlan = population.ElementAt(count - 1);
                    preCrossMutationFitnessOfSecond = secondTravelPlan.getFitness();
                    
                    // Set up cross point and get path segment from first chromosome.
                    temp = travelPlan.CrossMutationRequest();
                    tempIndex = travelPlan.CrossMutationIndexPoint();

                    // Apply cross-mutations and get new fitness values.
                    travelPlan.CrossMutationApplication(secondTravelPlan.CrossMutationRequest(tempIndex), tempIndex);
                    secondTravelPlan.CrossMutationApplication(temp, tempIndex);
                    travelPlan = checker.ProduceFitnessOfChromosome(travelPlan);
                    secondTravelPlan = checker.ProduceFitnessOfChromosome(secondTravelPlan);

                    // If either or both new fitness is higher than old, revert. 
                    if (travelPlan.getFitness() > preMutationFitness) {
                        travelPlan.RevertChromosome();
                    }
                    if (secondTravelPlan.getFitness() > preCrossMutationFitnessOfSecond) {
                        secondTravelPlan.RevertChromosome();
                    }

                    // Add best out of pre and post mutation to new population.
                    newPopulation.AddLast(travelPlan);
                    newPopulation.AddLast(secondTravelPlan);

                    // Adjust count as two chromosomes have changed, otherwise population will grow each evolution.
                    count--;
                }
            }

            return newPopulation.OrderBy(Chromosome => Chromosome.getFitness()).ToList();
        }

        private void IndividualCellMutationOption(int cellsToMutate, int preMutationFitness, Chromosome travelPlan) {
            // Number of cells to mutate is difference between it and best divided by mutationDivValue.
            cellsToMutate = (int)Math.Round((preMutationFitness - bestFitness) / mutationDivValue);

            // Make sure there is at least one mutation
            if (cellsToMutate == 0) {
                cellsToMutate = 1;
            }
            travelPlan.Mutation(cellsToMutate);

            // Calculate new fitness
            travelPlan = checker.ProduceFitnessOfChromosome(travelPlan);

            // If new fitness is higher than old, revert.
            if (travelPlan.getFitness() > preMutationFitness) {
                travelPlan.RevertChromosome();
            }

            // Add best out of pre and post mutation to new population.
            newPopulation.AddLast(travelPlan);
        }
    }
}
