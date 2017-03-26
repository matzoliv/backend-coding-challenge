using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoveoBCC.Core.Tests
{
    [TestClass]
    public class Levenshtein
    {
        [TestMethod]
        public void Levenshtein1 ()
        {
            Assert.AreEqual( 2, CoveoBCC.Core.Levenshtein.ComputeDistance( "GUMBO", "GAMBOL" ) );
            Assert.AreEqual( 6, CoveoBCC.Core.Levenshtein.ComputeDistance( "GuMbO", "gAmBoL" ) );
        }

        [TestMethod]
        public void LevenshteinBaseCase()
        {
            Assert.AreEqual( 0, CoveoBCC.Core.Levenshtein.ComputeDistance( "", "" ) );
            Assert.AreEqual( 3, CoveoBCC.Core.Levenshtein.ComputeDistance( "foo", "" ) );
            Assert.AreEqual( 3, CoveoBCC.Core.Levenshtein.ComputeDistance( "", "bar" ) );
        }
    }
}
