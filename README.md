# airport-distance-calculator
Calculates a distance between airports by their three-letter codes

* Logging: Serilog. TODO: Use apropriate centralazied logging sink, e.g. Serilog.Sinks.ElasticSearch or Serilog.Sinks.Seq 
* Rest API Doc: Swashbuckle
* Assemblies: just one for small service. Should be separated if service grow (structure is already marked up with top-level folders).
* A lot of unified infrastructure-related code should be moved to specialized libraries, if we have many services (as I hope)
