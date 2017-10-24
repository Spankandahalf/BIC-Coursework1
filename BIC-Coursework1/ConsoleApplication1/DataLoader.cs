using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;

namespace TravellingSalesmanOfIreland {
    /// <summary>
    /// Custom data loader for travelling salesman of Ireland.
    /// </summary>
    public class DataLoader {
        private string smallTest, fullSet;
        private CityLocations loadedData;

        public DataLoader() {
            smallTest = "TravellingSalesmanOfIreland.Data.ei8246TestS10.txt";
            fullSet = "TravellingSalesmanOfIreland.Data.ei8246.txt";

            loadedData = new CityLocations();
        }

        /// <summary>
        /// Get travelling salesman city locations from a file in Data folder.
        /// </summary>
        /// <param name="option">Option of 1: full set of points, 2: set of 10 points.</param>
        /// <returns>List of city locations(latitude & longitude).</returns>
        public CityLocations Load(int option) {
            switch (option) {
                case 1:LoadFileInDataFolder(fullSet);
                    break;
                case 2: LoadFileInDataFolder(smallTest);
                    break;
                default: Console.WriteLine("Option error on Load(" + option + ")");
                    break;
            }
            return loadedData;
        }

        private void LoadFileInDataFolder(string resourceName) {
            var assembly = Assembly.GetExecutingAssembly();

            try {
                Stream stream = assembly.GetManifestResourceStream(resourceName);
                StreamReader reader = new StreamReader(stream);

                Boolean processData = false;
                string lineOfData = reader.ReadLine();
                double latitude, longitude;

                while (!lineOfData.Equals("EOF", StringComparison.Ordinal)) {
                    if (processData) {
                        // Split string and get double values for latitude and longitude of each city in file.
                        // Note: first value is a number to idea the city so ignoring lineValues[0].
                        string[] lineValues = lineOfData.Split(new char[0]);
                        latitude = Convert.ToDouble(lineValues[1]);
                        longitude = Convert.ToDouble(lineValues[2]);
                        loadedData.AddCity(latitude, longitude);
                    } else if (lineOfData.Equals("NODE_COORD_SECTION", StringComparison.Ordinal)) {
                        processData = true;
                    }

                    lineOfData = reader.ReadLine();
                }
                
            } catch {
                Console.WriteLine("Error loading: " + resourceName);
            }
        }
    }
}
