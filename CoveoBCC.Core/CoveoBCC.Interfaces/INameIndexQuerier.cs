using System;
using System.Collections.Generic;

namespace CoveoBCC.Interfaces
{
    public class LookupResult
    {
        public string Result { get; }
        public float Distance { get; }

        public LookupResult ( string Result, float Distance )
        {
            this.Result = Result;
            this.Distance = Distance;
        }
    }

    public interface INameIndexQuerier
    {
        IEnumerable<LookupResult> Lookup ( string query );
    }
}
