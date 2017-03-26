namespace CoveoBCC.Core

type INameIndexBuilder =
    abstract Insert : string -> unit

type INameIndexQuerier =
    abstract Lookup : string -> string seq
