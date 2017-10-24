using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    class Program {
        static void Main(string[] args) {
            CityLocations cities;
            DataLoader loader = new DataLoader();

            cities = loader.Load(2);

            Console.WriteLine("Hello");
        }
    }
}
