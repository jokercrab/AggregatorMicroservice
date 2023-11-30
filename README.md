# Anime Series Aggregator Microservice
Aggregates series from two popular anime sites (anilibra,animevost), and returns a list of [Anime](DataStructs/Anime.cs) objects in json format.
Service functions in two different modes:
1. Straight forward fetch - aggregates the result from sites upon calling (takes some time to finish, depends on internet connection speed)
2. Background working - saves result locally, runs background task and updates the data every 1h.
In background mode data is loaded to local mongodb instance and could be requested on demand. MongoDB connection string and collection names can be modified at AnimeDB section of [appsettings](/appsettings.json).