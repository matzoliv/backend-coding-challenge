namespace CoveoBCC.Core

open System.Collections.Generic

module WordTrie =
    type Node = {
        Character           : char
        Children            : Dictionary<char, Node>
        mutable IsEndOfWord : bool
        mutable Parent      : Node option // allows to build the word from the last character
    }
    with
        static member Empty (c: char) =
            {
                Character = c
                Children = new Dictionary<char, Node>()
                IsEndOfWord = false
                Parent = None
            }

        member this.Insert (xs: char list) =
            match xs with
            | x::xs ->
                match this.Children.TryGetValue x with
                | (true, child) ->
                    child.Insert xs
                | (false, _) ->
                    let newNode = Node.Empty x
                    newNode.Parent <- Some this
                    newNode.Insert xs
                    this.Children.Add (x, newNode)
            | [] ->
                this.IsEndOfWord <- true

        member this.CurrentWord =
            let rec loop (current: Node) (chars: char list) =
                match current.Parent with
                | Some parentNode ->
                    loop parentNode <| current.Character::chars
                | None ->
                    System.String (List.toArray <| current.Character::chars)
            loop this []

        // Breadth first search too look for complete words starting with given prefix.
        member this.GetWords () =
            let queue = Queue<Node>()
            queue.Enqueue(this)
            seq {
                while queue.Count > 0 do
                    let current = queue.Dequeue()
                    if current.IsEndOfWord then
                        yield current.CurrentWord

                    for child in current.Children.Values do
                        queue.Enqueue(child)
            }

        member this.SearchNode (chars: char list) =
            match chars with
            | x::xs ->
                match this.Children.TryGetValue x with
                | (true, child) ->
                    child.SearchNode xs
                | (false, _) ->
                    None
            | [] ->
                Some this

    type Root = {
        Children : Dictionary<char, Node>
    }
    with
        static member Empty =
            {
                Children = new Dictionary<char, Node>()
            }

        member this.Insert (word: string) =
            match word.ToCharArray() |> Array.toList with
            | x::xs ->
                match this.Children.TryGetValue x with
                | (true, child) ->
                    child.Insert xs
                | (false, _) ->
                    let newNode = Node.Empty x
                    newNode.Insert xs
                    this.Children.Add (x, newNode)
            | [] ->
                ()

        member this.GetWords () =
            seq {
                for child in this.Children.Values do
                    yield! child.GetWords()
            }

        member this.GetWordsStartingWith (prefix: string) =
            let chars = prefix.ToCharArray() |> Array.toList
            let maybeNode =
                match chars with
                | x::xs ->
                    match this.Children.TryGetValue x with
                    | (true, child) ->
                        child.SearchNode xs
                    | (false, _) ->
                        None
                | [] ->
                    None
            match maybeNode with
            | Some node -> node.GetWords()
            | None -> Seq.empty
            |> Seq.map (
                fun w -> (w.Length - prefix.Length, w)
            )