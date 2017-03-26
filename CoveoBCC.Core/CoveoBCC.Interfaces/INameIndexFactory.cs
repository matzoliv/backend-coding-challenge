using System;
using System.Collections.Generic;

namespace CoveoBCC.Interfaces
{
    public interface INameIndexFactory
    {
        INameIndexQuerier BuildFrom ( IEnumerable<string> values );
    }
}
