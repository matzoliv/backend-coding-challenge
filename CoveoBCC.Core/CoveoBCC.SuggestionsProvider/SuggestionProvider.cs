using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoveoBCC.Interfaces;

namespace CoveoBCC.Core
{
    public class SuggestionProvider : ISuggestionsProvider
    {
        private readonly INameIndexQuerier m_nameIndexQuerier;
        private readonly Dictionary<string, List<City>> m_store;

        public SuggestionProvider (
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
            return
                m_nameIndexQuerier.Lookup( nameQuery )
                    .Where( x => m_store.ContainsKey( x.Result ) )
                    .SelectMany( x => m_store[ x.Result ].Select( c => new CityWithNameDistance( City: c, Distance: x.Distance ) ) )
                    .Take( limit )
                    .ApplyScoring( latitude, longitude )
                    .Select(
                        x => new Suggestion( Label: $"{x.City.Name} ({x.City.CountryCode})", Score: x.Score, Location: new Location( x.City.Latitude, x.City.Longitude ) )
                    )
                    .OrderByDescending( x => x.Score );
        }


    }
}
