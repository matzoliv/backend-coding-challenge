namespace CoveoBCC.Core

open System.Collections.Generic

module SentenceTrie =
    type Node = {
        Word                    : string
        Children                : Dictionary<string, Node>
        Sentences               : HashSet<string>
        mutable Parent          : Node option
    }
    with
        static member Empty (w: string) =
            {
                Word = w
                Children = new Dictionary<string, Node>()
                Sentences = new HashSet<string>()
                Parent = None
            }

        member this.Insert (sentence: string) (xs: string list) =
            match xs with
            | x::xs ->
                match this.Children.TryGetValue x with
                | (true, child) ->
                    child.Insert sentence xs
                | (false, _) ->
                    let newNode = Node.Empty x
                    newNode.Parent <- Some this
                    newNode.Insert sentence xs
                    this.Children.Add (x, newNode)
            | [] ->
                this.Sentences.Add(sentence) |> ignore

        member this.GetStartingWith =
            let queue = Queue<Node>()
            queue.Enqueue(this)
            seq {
                while queue.Count > 0 do
                    let current = queue.Dequeue()
                    if current.Sentences.Count > 0 then
                        yield! current.Sentences

                    for child in current.Children.Values do
                        queue.Enqueue(child)
            }

    type Root = {
        Children : Dictionary<string, Node>
    }
    with
        static member Empty =
            {
                Children = new Dictionary<string, Node>()
            }

        member this.TryGetNodeAt (words: string seq) =
            let rec loop (node: Node) (words: string list) =
                match words with
                | x::xs ->
                    match node.Children.TryGetValue x with
                    | (true, child) ->
                        loop child xs
                    | (false, _) ->
                        None
                | [] ->
                    Some node

            match Seq.toList words with
            | x::xs ->
                match this.Children.TryGetValue x with
                | (true, node) ->
                    loop node xs
                | (false, _) ->
                    None
            | [] -> None

        member this.Insert (sentence: string) (words: string seq) =
            match words |> Seq.toList with
            | x::xs ->
                match this.Children.TryGetValue x with
                | (true, child) ->
                    child.Insert sentence xs
                | (false, _) ->
                    let newNode = Node.Empty x
                    newNode.Insert sentence xs
                    this.Children.Add (x, newNode)
            | [] ->
                ()

    type SuffixTree = {
        Trees : SortedDictionary<int, Root>
    }
    with
        static member Empty = {
            Trees = new SortedDictionary<int, Root>()
        }

        member this.Insert (sentence: string) (words: string seq) =
            let rec loop (i: int) (words: string list) =
                match words with
                | _::rest ->
                    match this.Trees.TryGetValue(i) with
                    | (true, root) ->
                        root.Insert sentence words
                    | (false, _) ->
                        let newRoot = Root.Empty
                        newRoot.Insert sentence words
                        this.Trees.Add(i, newRoot) |> ignore
                    loop (i + 1) rest
                | [] ->
                    ()
            loop 0 (Seq.toList words)

        member this.GetMatching (words: string seq) =
            seq {
                for i in this.Trees.Keys do
                    match this.Trees.[i].TryGetNodeAt words with
                    | Some node ->
                        yield! node.GetStartingWith
                    | None ->
                        ()
            }

