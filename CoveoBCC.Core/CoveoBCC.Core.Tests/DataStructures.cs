using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CoveoBCC.Core.Tests
{
    [TestClass]
    public class DataStructures
    {
        [TestMethod]
        public void WordTrie ()
        {
            var trie = CoveoBCC.Core.WordTrie.Root.Empty;

            foreach ( var w in new[] { "london", "londonerry", "lake", "mitchell" } )
            {
                trie.Insert( w );
            }

            {
                var matches =
                    trie.GetWordsStartingWith( "londo" )
                        .Select( x => x.Item2 )
                        .ToList();

                Assert.IsTrue( matches.Contains( "london" ) );
                Assert.IsTrue( matches.Contains( "londonerry" ) );
                Assert.IsFalse( matches.Contains( "mitchell" ) );
            }

            {
                var matches =
                    trie.GetWordsStartingWith( "mitch" )
                        .Select( x => x.Item2 )
                        .ToList();

                Assert.IsFalse( matches.Contains( "london" ) );
                Assert.IsFalse( matches.Contains( "londonerry" ) );
                Assert.IsTrue( matches.Contains( "mitchell" ) );
            }

            {
                var matches =
                    trie.GetWordsStartingWith( "" )
                        .Select( x => x.Item2 )
                        .ToList();

                Assert.IsFalse( matches.Any() );
            }
        }

        [TestMethod]
        public void BKTree ()
        {
            var bktree = CoveoBCC.Core.BKTree.Root.Empty;

            foreach ( var w in new[] { "london", "londonerry", "lake", "mitchell" } )
            {
                bktree.Insert( w );
            }

            {
                var matches = bktree.GetCloserThan( "londin", 1 ).ToList();

                Assert.AreEqual( matches.Count, 1 );
                Assert.AreEqual( matches[ 0 ], Tuple.Create( 1, "london" ) );
            }

            {
                var matches = bktree.GetCloserThan( "londoneruj", 2 ).ToList();

                Assert.AreEqual( matches.Count, 1 );
                Assert.AreEqual( matches[ 0 ], Tuple.Create( 2, "londonerry" ) );
            }

            {
                var matches = bktree.GetCloserThan( "londoner", 1 ).ToList();

                Assert.AreEqual( matches.Count, 0 );
            }

            {
                var matches = bktree.GetCloserThan( "", 2 ).ToList();

                Assert.AreEqual( matches.Count, 0 );
            }

            {
                var matches = bktree.GetCloserThan( ";qjxk.,lu", 5 ).ToList();

                Assert.AreEqual( matches.Count, 0 );
            }
        }

        [TestMethod]
        public void SentenceTrie ()
        {
        }
    }
}
