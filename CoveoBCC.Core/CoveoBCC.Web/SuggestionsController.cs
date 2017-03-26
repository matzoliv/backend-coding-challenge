using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using CoveoBCC.Interfaces;

namespace CoveoBCC.Web
{
    public class City
    {
        public string name { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public double score { get; set; }
    }

    public class Response
    {
        public IEnumerable<City> suggestions { get; set; }
    }

    public class SuggestionsController : ApiController
    {
        private readonly ISuggestionsProvider m_suggestionsProvider;
        
        public SuggestionsController( ISuggestionsProvider suggestionsProvider )
        {
            m_suggestionsProvider = suggestionsProvider;
        }

        public IHttpActionResult Get(string q, string latitude = null, string longitude = null)
        {
            double? latitudeDouble = null;
            double? longitudeDouble = null;

            {
                double temp;
                if ( Double.TryParse( latitude, out temp ) && temp >= 0.0 )
                {
                    latitudeDouble = temp;
                }
            }

            {
                double temp;
                if ( Double.TryParse( longitude, out temp ) && temp >= 0.0 )
                {
                    longitudeDouble = temp;
                }
            }

            var suggestions = m_suggestionsProvider.GetSuggestions( q, 20, latitude: latitudeDouble, longitude: longitudeDouble );

            return Json(
                new Response
                {
                    suggestions =
                        suggestions.Select(
                            x =>
                                new City
                                {
                                    name = x.Label,
                                    latitude = x.Location.Latitude,
                                    longitude = x.Location.Longitude,
                                    score = x.Score
                                }
                        )
                }
            );
        }
    }
}
