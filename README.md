# airport-distance-calculator

## Purpose
Calculates a distance between airports by their three-letter codes

## Runtime dependencies
* "places" service

## API
* Provided SwaggerUI at `/swagger` path
* Provided Postman request collection in `postman` directory

## External libraries used

* Logging: Serilog. 
  * TODO: Use apropriate centralizied logging sink, e.g. Serilog.Sinks.ElasticSearch or Serilog.Sinks.Seq 
* Rest API Doc: Swashbuckle
* "Geolocation" library: fairly large community usage, looks simple and enough, the task is quite simple and there are no need to invent wheel here.  MIT License. If for any reason we have do give up this library, it would be trivial to replace it with the same functionality and naming. But their `Coordinate` type is mutable struct, unfortunately, so we'll hide the library behind our own types :(
  * Alternative "CoordinateSharp" - overcomplicated, and published under AGPL.
* Polly
* Macros.Json.Extensions

## Project structure
* Assemblies: just one for that small service. 
  * The solution should be separated into 5..7 assemblies if service grow.
  * The division structure is already marked up with top-level solution folders.
* A lot of unified infrastructure-related code should be moved to specialized libraries, if we have many services (as I hope).
  * Some generic utilities are placed in `CommonUtils` 

## Configuration
* The service reads `Config.json` file, which is not under version control
* See example with description in `Config.Example.json`

  


