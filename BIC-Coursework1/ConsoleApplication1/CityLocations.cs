using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TravellingSalesmanOfIreland {
    /// <summary>
    /// Store the latitude and longitude of each City.
    /// </summary>
    public class CityLocations {
        private LinkedList<Double> latitude, longitude;

        public CityLocations() {
            latitude = new LinkedList<double>();
            longitude = new LinkedList<double>();
        }

        public void AddCity(double latitude, double longitude) {
            this.latitude.AddLast(latitude);
            this.longitude.AddLast(longitude);
        }

        public double GetLatitude(int cityIndex) {
            return latitude.ElementAt(cityIndex);
        }

        public double GetLongitude(int cityIndex) {
            return longitude.ElementAt(cityIndex);
        }

        public void ClearCities() {
            latitude.Clear();
            longitude.Clear();
        }

        public int NumberOfCities() {
            return latitude.Count();
        }

        public bool HasEntries() {
            return latitude.Count() > 0;
        }
    }
}
