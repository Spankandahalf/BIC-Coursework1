using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    class Program {
        static void Main(string[] args) {
            CityLocations cities;
            FitnessChecker checker;
            DataLoader loader = new DataLoader();
            Tester codeChecking;
            PopulationCreator populationGenerator;
            List<Chromosome> population = new List<Chromosome>();
            EvolutionaryAlgorithm ea;
            Random randomNumberGenerator = new Random();

            cities = loader.Load(2);
            checker = new FitnessChecker(cities);
            ea = new EvolutionaryAlgorithm(checker, randomNumberGenerator);
            codeChecking = new Tester(checker, ea);
            populationGenerator = new PopulationCreator(cities.NumberOfCities(), checker);

            population = populationGenerator.CreateInitialPopulation();

            //codeChecking.ChromosomeTesting(randomNumberGenerator);
            //codeChecking.EvolvePopulationTesting(population);
            codeChecking.NeuronTesting(randomNumberGenerator);
            
            Console.WriteLine("Hello");
        }
    }
}
