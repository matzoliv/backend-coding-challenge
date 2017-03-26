using System;

namespace CoveoBCC.Interfaces
{
    public interface ISuggestionsProviderFactory
    {
        ISuggestionsProvider BuildFrom ();
    }
}
