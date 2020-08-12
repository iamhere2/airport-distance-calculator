# airport-distance-calculator
Calculates a distance between airports by their three-letter codes

* Logging: Serilog. TODO: Use apropriate centralazied logging sink, e.g. Serilog.Sinks.ElasticSearch or Serilog.Sinks.Seq 
* Rest API Doc: Swashbuckle
* Assemblies: just one for small service. Should be separated if service grow (structure is already marked up with top-level folders).
* A lot of unified infrastructure-related code should be moved to specialized libraries, if we have many services (as I hope).
* "Geolocation" library: fairly large community usage, looks simple and enough, the task is quite simple and there are no need to invent wheel here.  MIT License. If for any reason we have do give up this library, ti would be trivial to replace it with the same functionality and naming. But Coordinate is mutable struct :(
** "CoordinateSharp" - overcomplicated
** 
  


