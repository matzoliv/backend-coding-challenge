# Olivier Matz's Coveo Backend Coding Challenge
(inspired by https://github.com/busbud/coding-challenge-backend-c)

# Quick implementation overview

## Name search (CoveoBCC.NameSearch)
- The query is first sanitized by lowering the case and removing special characters
- Each word of the query is then assigned a stream of possible word candidates, each candidate having a distance score associated
    - The stream for the each word of the query except the last one is produced from a BKTree filled with all words in the database using the Levenshtein distance
    - The first part of the stream for the last word is produced from a trie all words in the database. The last part of the stream is using the same BKTree as the other words. Basically, I assume that the user is more interested in autocompleting the last word than trying to correct a typo on it.
- Then I generate a stream of candidate queries (series of words) in order of distance (the distance of the query being the sum of all the distance of each words in the query)
- Then I lookup each series of words in multiple "sentence tries" (tries built from the words of the full city name instead of each characters)
    - to be able to look for a series of word inside of each full city name, I build multiple tries for all suffixes.
      For example for the city "Mont Saint Hilaire", I insert "Mont Saint Hilaire" into trie 0, then "Saint Hilaire" into trie 1, then
      "Hilaire" into trie 2
