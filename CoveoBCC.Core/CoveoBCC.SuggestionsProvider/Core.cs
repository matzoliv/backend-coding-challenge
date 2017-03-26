using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoveoBCC.Interfaces;

namespace CoveoBCC.Core
{
    public class Core : ISuggestionsProvider
    {
        private readonly INameIndexQuerier m_nameIndexQuerier;
        private readonly Dictionary<string, List<City>> m_store;

        public Core (
            ICityRepository cityRepository,
            INameIndexFactory nameIndexFactory
        )
        {
            m_nameIndexQuerier = nameIndexFactory.BuildFrom( cityRepository.GetAllCities().Select( x => x.Name ) );
            m_store = new Dictionary<string, List<City>>();
            foreach ( var city in cityRepository.GetAllCities() )
            {
                List<City> citiesOfName;

                if ( !m_store.TryGetValue( city.Name, out citiesOfName ) )
                {
                    citiesOfName = new List<City>();
                    m_store.Add( city.Name, citiesOfName );
                }
                citiesOfName.Add( city );
            }
        }

        public IEnumerable<Suggestion> GetSuggestions ( string nameQuery, int limit, double? latitude = default( double? ), double? longitude = default( double? ) )
        {
            var matches =
                m_nameIndexQuerier.Lookup( nameQuery )
                    .Where( x => m_store.ContainsKey( x.Result ) )
                    .SelectMany( x => m_store[ x.Result ] )
                    .Take( limit )
                    .ToList();

            return matches.Select(
                x => new Suggestion( Label: x.Name, Score: 1.0, Location: new Location( x.Latitude, x.Longitude ) )
            );

        }
    }
}
