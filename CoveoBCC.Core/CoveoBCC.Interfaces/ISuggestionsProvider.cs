using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoveoBCC.Interfaces
{
    public class Location
    {
        public double Latitude { get; }
        public double Longitude { get; }

        public Location ( double Latitude, double Longitude )
        {
            this.Latitude = Latitude;
            this.Longitude = Longitude;
        }
    }

    public class Suggestion
    {
        public string Label { get; }
        public double Score { get; }
        public Location Location { get; }

        public Suggestion ( string Label, double Score, Location Location )
        {
            this.Label = Label;
            this.Score = Score;
            this.Location = Location;
        }
    }

    public interface ISuggestionsProvider
    {
        IEnumerable<Suggestion> GetSuggestions ( string nameQuery, int limit, double? latitude = null, double? longitude = null );
    }
}
