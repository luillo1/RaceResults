# RaceResults API

This is the backend service hosted at https://api.raceresults.run

## Requirements
[Azure CLI](https://docs.microsoft.com/en-us/cli/azure/)
[.NET CLI](https://docs.microsoft.com/en-us/dotnet/core/tools/)

## Getting started

RaceResults makes use of deployed sandbox resources in order to maintain close parity with the functionality of our production environment. To access these sandbox resources, run `az login` and login using your @raceresults.run credentials. If you don't have credentials, reach out to the development team and an account will be provisioned for you. After logging in, the API can be run locally by running `dotnet run` from the `scr/API` directory. 
