using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    public class PopulationCreator {
        private LinkedList<Chromosome> population;
        private FitnessChecker checker;
        private double populationFactor;
        private int populationSize, numberOfCites;

        public PopulationCreator(int numberOfCites, FitnessChecker checker) {
            population = new LinkedList<Chromosome>();
            this.numberOfCites = numberOfCites;
            this.checker = checker;
            populationFactor = 1.5;
            populationSize = (int)Math.Round((numberOfCites * populationFactor) * (numberOfCites * populationFactor));
        }

        /// <summary>
        /// Will create an initial population.
        /// Note: Population size default is number of cities times 1.5 then squared.
        /// </summary>
        /// <returns>Initial population.</returns>
        public List<Chromosome> CreateInitialPopulation() {
            Random selector = new Random();
            List<Chromosome> population2 = new List<Chromosome>();

            for (int c = 0; c < populationSize; c++) {
                population.AddLast(CreateChromosome(selector));
            }

            population2 = population.OrderBy(Chromosome => Chromosome.getFitness()).ToList();

            return population2;
        }

        /// <summary>
        /// Will create an initial population.
        /// Note: Population size will be number of cities times X then squared.
        /// </summary>
        /// <param name="newPopulationFactor">New factor to multiple against number of cities (X).</param>
        /// <returns>Initial population.</returns>
        public List<Chromosome> CreateInitialPopulation(double newPopulationFactor) {
            populationFactor = newPopulationFactor;
            populationSize = (int)Math.Round((numberOfCites * populationFactor) * (numberOfCites * populationFactor));

            return CreateInitialPopulation();
        }

        /// <summary>
        /// Creates a chromosome.
        /// Note: Had a Random inside the method at first, the quick repeat creation of Random caused repeat results.
        /// </summary>
        /// <param name="selector">A random number generator.</param>
        /// <returns>A randomly sequenced chromosome.</returns>
        private Chromosome CreateChromosome(Random selector) {
            Chromosome sequence = new Chromosome(selector);
            LinkedList<int> usedCities = new LinkedList<int>();
            int city = -1;

            // Select city to start from.
            city = selector.Next(numberOfCites);
            sequence.SetStartCity(city);
            usedCities.AddLast(city);

            // Form path through cities
            while (usedCities.Count < numberOfCites) {
                while(usedCities.Contains(city)) {
                    city = selector.Next(numberOfCites);
                }

                sequence.AddCityToPath(city);
                usedCities.AddLast(city);
            }

            sequence = checker.ProduceFitnessOfChromosome(sequence);

            return sequence;
        }

        public void ResetDefaultPopulationFactor() {
            populationFactor = 1.5;
            populationSize = (int)Math.Round((numberOfCites * populationFactor) * (numberOfCites * populationFactor));
        }
    }
}
