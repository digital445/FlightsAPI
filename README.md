# FlightsAPI

## Purpose
The project represents an API that should aggregate information about available flights from 2 sources. In addition it should give the client an opportunity to book the selected flight offer.
## Stack
.NET 8, PostgreSQL
## Sources
Amadeus GDS test API https://developers.amadeus.com/self-service

Demo Flight PostgreSQL database https://edu.postgrespro.com/demo-small-en.zip   
[Entity-Relationship Diagram](https://github.com/digital445/FlightsAPI/blob/main/FlightDb_ERD.png)
### Limitations
* By default FlightDb has information about flights in range from 2017-07-16 to 2017-09-14.  
* Working with the db, the API supports only 1 segment routes.


## Plans
* Write Swagger API documentation  
* Add more tests including functional  
* ~~Implement request caching~~  
* Add authentication and authorization  
* Create simple web app 
* Containerization of API, application and database with Docker  
