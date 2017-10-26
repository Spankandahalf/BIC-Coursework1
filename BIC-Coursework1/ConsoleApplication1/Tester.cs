using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    class Tester {
        public Tester() {

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
            chrom.SetFitnessValue(5);

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
            chrom.SetFitnessValue(10);
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
            chrom.SetFitnessValue(20);
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
            chrom2.SetFitnessValue(5);

            Console.WriteLine("Second Chromosome create to test cross mutation.");
            Console.WriteLine(chrom2.ViewChromosome());
            Console.WriteLine("");

            LinkedList<int> temp = chrom.CrossMutationRequest();
            int tempIndex = chrom.CrossMutationIndexPoint();
            
            chrom.CrossMutationApplication(chrom2.CrossMutationRequest(tempIndex), tempIndex);
            chrom2.CrossMutationApplication(temp, tempIndex);

            Console.WriteLine("Cross mutation check.");
            Console.WriteLine(chrom.ViewChromosome());
            Console.WriteLine(chrom2.ViewChromosome());
            Console.WriteLine("");
        } 
    }
}
