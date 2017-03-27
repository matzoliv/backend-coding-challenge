# Olivier Matz's Coveo Backend Coding Challenge
(inspired by https://github.com/busbud/coding-challenge-backend-c)

# Quick implementation overview

## Name search (CoveoBCC.NameSearch)
- The query is first sanitized by lowering case and removing special characters
- Each word of the query is then assigned a stream of possible word candidates, each candidate having a distance score associated
    - The stream for each word of the query _except the last one_ is produced from all words in the database with a Levenshtein distance lesser than 2. This is computed efficiently using a BKTree.
    - The first part of the stream _for the last word_ is composed of all words for which the last word of the query is a prefix. This is computed efficiently from a trie of all words in the database. The second part of the stream produced the same way as other words (levenshtein distance lesser that two). Basically, I assume that the user is more interested in autocompleting the last word than trying to correct a typo on it.
- Then I generate a stream of candidate queries (series of words) in order of distance (the distance of the query being the sum of all the distance of each words in the query)
- Then I lookup each series of words in multiple "sentence tries" (tries built from the words of the full city name instead of each characters)
    - To be able to look for a series of word inside of each full city name, I build multiple tries for all suffixes.
      For example for the city "Mont Saint Hilaire", I insert "Mont Saint Hilaire" into trie 0, then "Saint Hilaire" into trie 1, then
      "Hilaire" into trie 2

## Suggestion and scoring
- The city name database is held in memory along with the name search indexes
- I first do a lookup in the name search component and take the first 20 results along with their distances (the distance of the result is set to be the distance of the inferred candidate query during the name search process)
- If only a latitude is given, I add to the result distance the difference between the city's latitude and the given one
- If only a longitude is given, I add to the result distance the difference between the city's longitude and the given one
- If both a latitude and longitude are given, I add to the result distance the distance between the point formed by the city's (longitude, latitude) and the given (longitude, latitude)
- I map all the distances of the results to a value between 1.0 and 0.0 (lower is better a this point)
- Then I reverse the score to get the proper "higher is better" result

## Hosting
- I use Azure Cloud Service with OWIN to implement the HTTP API
- The database is read before handling the first request from an Azure storage account
