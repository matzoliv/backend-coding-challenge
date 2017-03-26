namespace CoveoBCC.Core

/// Algorithm taken from http://people.cs.pitt.edu/~kirk/cs1501/Pruhs/Spring2006/assignments/editdistance/Levenshtein%20Distance.htm

module Levenshtein =
    let ComputeDistance (str1: string) (str2: string) = 
        let str1Length = str1.Length
        let str2Length = str2.Length
        let array =
            [|
                yield [| 0..str1Length |] 
                for i in 1..str2Length ->
                    [|
                        yield i
                        for _ in 1..str1Length -> 0
                    |]
            |]

        for i in 1..str2Length do
            for j in 1..str1Length do
                let cost = if str1.[j - 1] = str2.[i - 1] then 0 else 1
                array.[i].[j] <-
                    Seq.min <|
                        seq {
                            yield array.[i - 1].[j] + 1
                            yield array.[i].[j - 1] + 1
                            yield array.[i - 1].[j - 1] + cost
                        }

        array.[str2Length].[str1Length]
