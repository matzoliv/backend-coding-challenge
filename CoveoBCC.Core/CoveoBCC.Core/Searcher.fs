namespace CoveoBCC.Core

open System.Text.RegularExpressions
open System.Diagnostics

module Searcher =
    let sanitizeRegex = new Regex(@"[()',.\-""]", RegexOptions.Compiled);
    let splitterRegex = new Regex(@"\s+",RegexOptions.Compiled);

    let sanitize (s: string) =
        sanitizeRegex.Replace(s.ToLower(), " ")

    let split (s: string) =
        splitterRegex.Split(s)

    let tokenize = sanitize >> split

    type Implementation = {
        WordsIndex : WordTrie.Root
        FuzzyIndex : BKTree.Root
        SentencesIndex : SentenceTrie.SuffixTree
    }
    with
        static member Empty = {
            WordsIndex = WordTrie.Root.Empty
            FuzzyIndex = BKTree.Root.Empty
            SentencesIndex = SentenceTrie.SuffixTree.Empty
        }

        member x.LoadFrom (streamReader: System.IO.StreamReader) =
            while not streamReader.EndOfStream do
                let line = streamReader.ReadLine()
                let tokens = tokenize line
                tokens |> Seq.iter x.WordsIndex.Insert 
                tokens |> Seq.iter x.FuzzyIndex.Insert
                x.SentencesIndex.Insert line tokens

        // Lazily iterates 
        member x.GetInnerWordCandidates (word: string) = 
            x.FuzzyIndex.GetCloserThan word 2
            |> Seq.sortBy fst
            |> Seq.map (fun (distance, word) -> (float distance, word))

        // Lazily iterates through candidates for the given word, beginning by
        // iterating all words for which the given string is a prefix.
        member x.GetLastWordCandidates (word: string) : (float * string) seq = 
            Seq.concat <|
                [
                    x.WordsIndex.GetWordsStartingWith word
                    |> Seq.map (fun (distance, word) -> ((float distance) / 0.33, word))

                    x.FuzzyIndex.GetCloserThan word 2
                    |> Seq.sortBy fst
                    |> Seq.map (fun (distance, word) -> ((float distance) , word))
                ]

        member x.GetPossibleQueries (s: string) =
            let tokens = tokenize s
            seq {
                let wordSequences = 
                    [|
                        yield (x.GetLastWordCandidates <| Array.last tokens)
                        for token in tokens.[0..(tokens.Length - 2)] do
                            yield (x.GetInnerWordCandidates token)
                    |]

                let enums = wordSequences |> Array.map (fun xs -> xs.GetEnumerator())
                let enums = enums |> Array.filter (fun e -> e.MoveNext()) |> Array.mapi (fun i e -> (i, e))

                let currentWords = ref (enums |> Array.map (fun (i, e) -> e.Current) |> Array.map snd)
                if (!currentWords).Length > 0 then
                    yield !currentWords

                let candidates = ref (enums |> Array.filter (fun (i, e) -> e.MoveNext()))

                while (!candidates).Length > 0 do
                    let (index, e) = !candidates |> Seq.minBy (fun (i, e) -> fst <| e.Current )
                    let newWords = Array.copy !currentWords
                    newWords.[index] <- e.Current |> snd
                    yield newWords
                    if not <| e.MoveNext() then
                        candidates := (!candidates) |> Array.filter (fun (_, thisE) -> e <> thisE)
            }
            // Put back the last word at the back of the result
            |> Seq.map ( fun words -> Array.append words.[1..(words.Length - 1)] [| words.[0] |]  )

        interface INameIndexQuerier with
            member x.Lookup (s: string) =
                seq {
                    for query in x.GetPossibleQueries s do
                        yield! x.SentencesIndex.GetMatching query
                }

