using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;

namespace CoveoBCC.Core
{
    public interface ICityRepository
    {
        IEnumerable<City> GetAllCities ();
    }

    public class FileCityRepository : ICityRepository
    {
        private readonly List<City> m_cities;

        public FileCityRepository ( [Dependency("CitiesTSVFile")] string filename )
        {
            m_cities = new List<City>();

            using ( var fileStream = System.IO.File.OpenRead( filename ) )
            {
                using ( var streamReader = new System.IO.StreamReader( fileStream ) )
                {
                    streamReader.ReadLine();

                    while ( !streamReader.EndOfStream )
                    {
                        var fields = streamReader.ReadLine().Split( '\t' );

                        m_cities.Add(
                            new City(
                                Name: fields[1],
                                Latitude: Double.Parse(fields[4]),
                                Longitude: Double.Parse(fields[5])
                            )
                        );
                    }
                }
            }
        }

        public IEnumerable<string> Select ( Func<object, object> p )
        {
            throw new NotImplementedException();
        }

        public IEnumerable<City> GetAllCities ()
        {
            return m_cities;
        }
    }
}
