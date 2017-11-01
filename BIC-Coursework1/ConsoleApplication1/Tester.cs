using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    class Tester {
        private FitnessChecker checker;
        private EvolutionaryAlgorithm ea;

        public Tester(FitnessChecker checker, EvolutionaryAlgorithm ea) {
            this.checker = checker;
            this.ea = ea;
        }

        public void ChromosomeTesting() {
            // Create
            Chromosome chrom = new Chromosome();
            for (int i = 0; i < 10; i++) {
                if (i == 0) {
                    chrom.SetStartCity(i);
                } else {
                    chrom.AddCityToPath(i);
                }
            }
            chrom = checker.ProduceFitnessOfChromosome(chrom);

            Console.WriteLine("Chromosome create, should see 0 - 9 followed by zero");
            Console.WriteLine(chrom.ViewChromosome());
            Console.WriteLine("");

            // Get first trip cities.
            Console.WriteLine("Get cities from first trip");
            Console.WriteLine("Start at: " + chrom.TripStartFrom(1) + " end at: " + chrom.TripDestination(1));
            Console.WriteLine("");

            // Get fourth trip cities.
            Console.WriteLine("Get cities from fourth trip");
            Console.WriteLine("Start at: " + chrom.TripStartFrom(4) + " end at: " + chrom.TripDestination(4));
            Console.WriteLine("");

            // Get last trip cities.
            Console.WriteLine("Get cities from last trip");
            Console.WriteLine("Start at: " + chrom.TripStartFrom(10) + " end at: " + chrom.TripDestination(10));
            Console.WriteLine("");

            // Single mutation.
            chrom.Mutation(1);
            chrom = checker.ProduceFitnessOfChromosome(chrom);
            Console.WriteLine("Chromosome has undergone a single mutation.");
            Console.WriteLine(chrom.ViewChromosome());
            Console.WriteLine("");

            // Reversion 
            chrom.RevertChromosome();
            Console.WriteLine("Revert Chromosome.");
            Console.WriteLine(chrom.ViewChromosome());
            Console.WriteLine("");

            // Nine mutations. (Watch in debug to see if any mutations are undone)
            chrom.Mutation(9);
            chrom = checker.ProduceFitnessOfChromosome(chrom);
            Console.WriteLine("Chromosome has undergone three mutations.");
            Console.WriteLine(chrom.ViewChromosome());

            // Make a second chromosome and test cross mutation.
            Chromosome chrom2 = new Chromosome();
            for (int i = 0; i < 10; i++) {
                if (i == 0) {
                    chrom2.SetStartCity(i);
                } else {
                    chrom2.AddCityToPath(i);
                }
            }
            chrom2 = checker.ProduceFitnessOfChromosome(chrom2);

            Console.WriteLine("Second Chromosome create to test cross mutation.");
            Console.WriteLine(chrom2.ViewChromosome());
            Console.WriteLine("");

            LinkedList<int> temp = chrom.CrossMutationRequest();
            int tempIndex = chrom.CrossMutationIndexPoint();
            
            chrom.CrossMutationApplication(chrom2.CrossMutationRequest(tempIndex), tempIndex);
            chrom2.CrossMutationApplication(temp, tempIndex);
            chrom = checker.ProduceFitnessOfChromosome(chrom);
            chrom2 = checker.ProduceFitnessOfChromosome(chrom2);

            Console.WriteLine("Cross mutation check using point: " + chrom.CrossMutationIndexPoint() + " giving: ");
            Console.WriteLine(chrom.ViewChromosome());
            Console.WriteLine(chrom2.ViewChromosome());
            Console.WriteLine("");

            // Make a third chromosome and test cross mutation.
            Chromosome chrom3 = new Chromosome();
            for (int i = 0; i < 10; i++) {
                if (i == 4) {
                    chrom3.SetStartCity(i);
                } else {
                    chrom3.AddCityToPath(i);
                }
            }
            chrom3 = checker.ProduceFitnessOfChromosome(chrom3);

            Console.WriteLine("Third Chromosome create to test cross mutation with different start/end point.");
            Console.WriteLine(chrom3.ViewChromosome());
            Console.WriteLine("Second current looks like: " + chrom2.ViewChromosome());
            Console.WriteLine("");

            temp = chrom2.CrossMutationRequest();
            tempIndex = chrom2.CrossMutationIndexPoint();

            chrom2.CrossMutationApplication(chrom3.CrossMutationRequest(tempIndex), tempIndex);
            chrom3.CrossMutationApplication(temp, tempIndex);
            chrom2 = checker.ProduceFitnessOfChromosome(chrom2);
            chrom3 = checker.ProduceFitnessOfChromosome(chrom3);

            Console.WriteLine("Cross mutation check using point: " + tempIndex + " giving: ");
            Console.WriteLine(chrom2.ViewChromosome());
            Console.WriteLine(chrom3.ViewChromosome());
            Console.WriteLine("");
        } 

        public void EvolvePopulationTesting(List<Chromosome> population) {
            int initialBestFitness, initialWorseFitness, evolvedBestFitness, evolvedWorseFitness;
            List<Chromosome> newPopulation = new List<Chromosome>();
            List<Chromosome> newPopulation2 = new List<Chromosome>();
            List<Chromosome> newPopulation3 = new List<Chromosome>();

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Evolving population Testing.");
            Console.WriteLine("Note: This will use best and worse fitness to show changes, but I will use debug break points to investigate further.");
            Console.WriteLine("");
            Console.WriteLine("");

            initialBestFitness = population.ElementAt(0).getFitness();
            initialWorseFitness = population.ElementAt(population.Count - 1).getFitness();
            Console.WriteLine("The populations current best fitness is: " + initialBestFitness);
            Console.WriteLine("With a worse of :" + initialWorseFitness);
            Console.WriteLine("");

            Console.WriteLine("Population evolved 5 times with default factors. Cross 60% & Mutation per 10% of range.");
            Console.WriteLine("");

            for (int i = 0; i < 5; i++) {
                if(i ==0) {
                    newPopulation = ea.EvolvePopulation(population);
                } else {
                    newPopulation = ea.EvolvePopulation(newPopulation);
                }

                evolvedBestFitness = newPopulation.ElementAt(0).getFitness();
                evolvedWorseFitness = newPopulation.ElementAt(newPopulation.Count - 1).getFitness();
                Console.WriteLine("New best fitness for generation " + (i + 1) + " is: " + evolvedBestFitness);
                Console.WriteLine("With a worse of :" + evolvedWorseFitness);
                Console.WriteLine("");
            }

            Console.WriteLine("Population evolved 5 times with default factors. Cross 40% & Mutation per 5% of range.");
            Console.WriteLine("");

            for (int i = 0; i < 5; i++) {
                if (i == 0) {
                    newPopulation2 = ea.EvolvePopulation(population, 0.4, 0.05);
                } else {
                    newPopulation2 = ea.EvolvePopulation(newPopulation2, 0.4, 0.05);
                }

                evolvedBestFitness = newPopulation2.ElementAt(0).getFitness();
                evolvedWorseFitness = newPopulation2.ElementAt(newPopulation2.Count - 1).getFitness();
                Console.WriteLine("New best fitness for generation " + (i + 1) + " is: " + evolvedBestFitness);
                Console.WriteLine("With a worse of :" + evolvedWorseFitness);
                Console.WriteLine("");
            }

            Console.WriteLine("Population evolved 5 times with default factors. Cross 70% & Mutation per 1% of range.");
            Console.WriteLine("");

            for (int i = 0; i < 5; i++) {
                if (i == 0) {
                    newPopulation3 = ea.EvolvePopulation(population, 0.7, 0.01);
                } else {
                    newPopulation3 = ea.EvolvePopulation(newPopulation3, 0.7, 0.01);
                }

                evolvedBestFitness = newPopulation3.ElementAt(0).getFitness();
                evolvedWorseFitness = newPopulation3.ElementAt(newPopulation3.Count - 1).getFitness();
                Console.WriteLine("New best fitness for generation " + (i + 1) + " is: " + evolvedBestFitness);
                Console.WriteLine("With a worse of :" + evolvedWorseFitness);
                Console.WriteLine("");
            }
        }
    }
}
