using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoveoBCC.Core;
using CoveoBCC.Interfaces;

namespace SuggestionsProvider.CommandLine
{
    class Program
    {
        static void Main ( string[] args )
        {
            var cityRepository = new FileCityRepository( @"C:\Users\matzoliv\dev\backend-coding-challenge\cities_canada-usa-big.tsv" );

            var suggestionProvider = new SuggestionProvider( cityRepository, new CoveoBCC.Core.Searcher.NameSearcherFactory() );

            var xs = suggestionProvider.GetSuggestions( "west c", 20 ).ToList();
        }
    }
}
 