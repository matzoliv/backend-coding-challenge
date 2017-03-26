namespace CoveoBCC.Core

open System.Diagnostics
open System.Collections.Generic

/// Algorithm taken from http://blog.notdot.net/2007/4/Damn-Cool-Algorithms-Part-1-BK-Trees

module BKTree =
    type Node = {
        Key : string
        Children : Dictionary<int, Node>
    }
    with
        static member Empty (key: string) =
            {
                Key = key
                Children = new Dictionary<int, Node>()
            }

        member this.Insert (x: string) =
            if x <> this.Key then
                let distance = Levenshtein.ComputeDistance this.Key x
                if this.Children.ContainsKey distance then
                    this.Children.[distance].Insert x
                else
                    this.Children.Add(distance, Node.Empty x)

        member this.GetCloserThan (x: string) (n: int) =
            seq {
                let distance = Levenshtein.ComputeDistance this.Key x
                if distance <= n then
                    yield (distance, this.Key)
                for i in (distance - n)..(distance + n) do
                    match this.Children.TryGetValue i with
                    | (true, child) -> yield! child.GetCloserThan x n
                    | (false, _) -> ()
            }

    type Root = {
        mutable Root : Node option
        AllWords : HashSet<string>
    }
    with
        static member Empty = {
            Root = None
            AllWords = new HashSet<string>()
        }

        member this.Insert (x: string) =
            if this.AllWords.Add(x) then
                match this.Root with
                | None ->
                    this.Root <- Some <| Node.Empty x
                | Some root ->
                    root.Insert x

        member this.GetCloserThan (x: string) (n: int) =
            match this.Root with
            | None ->
                Seq.empty
            | Some root ->
                root.GetCloserThan x n
