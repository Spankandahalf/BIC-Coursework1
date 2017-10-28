using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    public class FitnessChecker {
        private CityLocations cities;

        public FitnessChecker(CityLocations cities) {
            this.cities = cities;
        }

        /// <summary>
        /// Will take a chromosome, work out the distances between each city in the path and then 
        /// give the chromosome a fitness value of the total distance travelled using path.
        /// </summary>
        /// <param name="chromosome">The chromosome you want a fitness to be added to.</param>
        /// <returns>The chromosome supplied with it's fitness value added to it.</returns>
        public Chromosome ProduceFitnessOfChromosome(Chromosome chromosome) {
            int fitness = 0;

            // Remember Trip methods in chromosome start from 1 not 0.
            for(int c = 1; c <= cities.NumberOfCities(); c++) {
                fitness += DistanceValue(chromosome.TripStartFrom(c), chromosome.TripDestination(c));
            }

            chromosome.SetFitnessValue(fitness);

            return chromosome;
        }

        private int DistanceValue(int startCity, int endCity) {
            int distance = 0;
            double latS, latD, longS, longD;
            double q1, q2, q3, q4, q5;

            latS = Math.PI * (cities.GetLatitude(startCity) / 180.00);
            latD = Math.PI * (cities.GetLatitude(endCity) / 180.00);
            longS = Math.PI * (cities.GetLongitude(startCity) / 180.00);
            longD = Math.PI * (cities.GetLongitude(endCity) / 180.00);

            q1 = Math.Cos(latD) * Math.Sin(longS - longD);
            q3 = Math.Sin((longS - longD) / 2.00);
            q4 = Math.Cos((longS - longD) / 2.00);
            q2 = Math.Sin(latS + latD) * q3 * q3 - Math.Sin(latS - latD) * q4 * q4;
            q5 = Math.Cos(latS - latD) * q4 * q4 - Math.Cos(latS + latD) * q3 * q3;

            distance = (int)(6378388.00 * Math.Atan2(Math.Sqrt(q1 * q1 + q2 * q2), q5) + 1.00);

            return distance;
        }
    }
}
