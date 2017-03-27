using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoveoBCC.Interfaces;

namespace CoveoBCC.Core
{
    public class CityWithNameDistance
    {
        public City City { get; }
        public double Distance { get; }

        public CityWithNameDistance ( City City, double Distance )
        {
            this.City = City;
            this.Distance = Distance;
        }
    }

    public class CityWithFinalScore
    {
        public City City { get; }
        public double Score { get; }

        public CityWithFinalScore ( City City, double Score )
        {
            this.City = City;
            this.Score = Score;
        }
    }

    public static class ScoringStrategy
    {
        public static IEnumerable<CityWithFinalScore> ApplyScoring( this IEnumerable<CityWithNameDistance> cities, double? latitude = default( double? ), double? longitude = default( double? ) )
        {
            Func<Location, double> getDistance;

            if ( !latitude.HasValue && longitude.HasValue )
            {
                getDistance = ( location ) => Math.Abs( location.Longitude - longitude.Value );
            }
            else if ( latitude.HasValue && !longitude.HasValue )
            {
                getDistance = ( location ) => Math.Abs( location.Latitude - latitude.Value );
            }
            else if ( latitude.HasValue && longitude.HasValue )
            {
                getDistance = ( location ) => Compute2PointsDistance( location.Longitude, location.Latitude, longitude.Value, latitude.Value );
            }
            else
            {
                getDistance = _ => 0.0;
            }

            var scoredMatches =
                cities
                    .Select( x => new { City = x.City, Score = x.Distance + getDistance( new Location( x.City.Latitude, x.City.Longitude ) ) } );

            if ( scoredMatches.Any() )
            {
                var max = scoredMatches.Select( x => x.Score ).Max();
                var min = scoredMatches.Select( x => x.Score ).Min();

                if ( max > min )
                {
                    return
                        scoredMatches
                            .Select( x => new CityWithFinalScore( City: x.City, Score: 1.0 - ( ( x.Score - min ) / ( max - min ) ) ) );
                }
                else
                {
                    return
                        scoredMatches
                            .Select( x => new CityWithFinalScore( City: x.City, Score: 1.0 ) );
                }
            }
            else
            {
                return Enumerable.Empty<CityWithFinalScore>();
            }
        }

        private static double Compute2PointsDistance ( double ax, double ay, double bx, double by )
        {
            return Math.Sqrt( Math.Pow( ax + bx, 2 ) + Math.Pow( ay + by, 2 ) );
        }
    }
}
