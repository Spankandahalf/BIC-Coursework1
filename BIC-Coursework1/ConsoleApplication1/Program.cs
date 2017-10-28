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

            cities = loader.Load(2);
            checker = new FitnessChecker(cities);
            codeChecking = new Tester(checker);
            populationGenerator = new PopulationCreator(cities.NumberOfCities(), checker);

            population = populationGenerator.CreateInitialPopulation();

            codeChecking.ChromosomeTesting();
            
            Console.WriteLine("Hello");
        }
    }
}
