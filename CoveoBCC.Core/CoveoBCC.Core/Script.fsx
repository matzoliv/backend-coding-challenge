// Learn more about F# at http://fsharp.org. See the 'F# Tutorial' project
// for more guidance on F# programming.

#load "Library1.fs"
open CoveoBCC.Core

// Define your library scripting code here

let fileStream = System.IO.File.OpenRead(@"C:\Users\matzoliv\Desktop\cities-small.txt");;
let streamReader = new System.IO.StreamReader( fileStream );;
let searcher = CoveoBCC.Core.Searcher.Implementation.Empty;;
searcher.LoadFrom(streamReader);;
