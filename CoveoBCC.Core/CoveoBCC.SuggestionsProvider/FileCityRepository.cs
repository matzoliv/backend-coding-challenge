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
                        m_cities.Add( City.FromTabbedSeparatedLine( streamReader.ReadLine() ) );
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
