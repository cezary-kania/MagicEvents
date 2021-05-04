# Magic Events
***
## Description
Have you ever wanted to create event management system? You can treat this repositorium as an example or template! There are many predefined features that you can adjust to your requirements. You don't have to worry about Auth stuff etc. List of implemented function you can find below. This project, focus mainly on backend side of application. I've planned to add frontend layer (ASP.NET MVC project), so stay tuned for updates. For this moment, you have to add frontent client by yourself. 
### Implemented features
* ##### Events management
    * ###### Event host / co-organizer panel
    * ###### Registration on events
* ##### User accounts
    * ###### Profiles 
    * ###### Identity
    * ###### Activities
* ##### Complex Swagger documentation
* ##### JWT Authentication
### Used technologies
* .Net 5 / ASP.NET Core 5
* FluentValidation
* Automapper
* SixLabors.ImageSharp (for thumbnails and other image operations)
* Microsoft.AspNetCore.Authentication.JwtBearer
* MongoDB.Driver
* AspNetCore.HealthChecks.MongoDb
* xUnit
* Fluentassertions
* Moq
* WebApplicationFactory (for integration tests)
* Docker / docker-compose
### Implemented patterns
* REST Api
* DDD
* CQRS (Simlified version - without MediatR, handlers etc.)
* Repository pattern
* SOLID
***
## Requirements
* ##### **Docker (Tested on Docker Desktop - versions 20.10.2 and 19.03.13)** 
* ##### **Dotnet SDK (5.0.+)** *(required only if you want run unit and integration tests or build solution without docker)*
***
## Building and running solution

### Description:
Presented solution works on docker containers. After cloning this repository and running commands shown below, there's no need to set up anything. Building takes a while, but please be patient - docker has to download and build all images.

When the solution starting process is completed, in your docker desktop you should see something similar to:

![Docker desktop](https://i.imgur.com/C1nawem.png)

#### Default environment variables you can change:
```
MongoDbSettings__ConnectionString=mongodb://root:password@mongoDB:27017
JwtSettings__Secret=Super_secret_key_123!
MONGO_INITDB_ROOT_USERNAME=root
MONGO_INITDB_ROOT_PASSWORD=password
```

### To build solution
#### Generate https certificate
```
dotnet dev-certs https --clean
dotnet dev-certs https -ep %USERPROFILE%\.aspnet\https\aspnetapp.pfx -p Qwerty1234!
dotnet dev-certs https --trust
```
#### Build docker images
```
$ docker-compose build 
```
#### To run solution
```
$ docker-compose up
```

#### Optional
If you want to run solution without docker. You should set secrets:
```
$ dotnet user-secrets init
$ dotnet user-secrets set MongoDbSettings:ConnectionString mongodb://localhost:27017
$ dotnet user-secrets set JwtSettings:Secret super_secret_key_123!
```
## Tests
There are 97 integration tests and 76 unit tests. I'm still working on it. As soon as possible, I'll cover much more code with unit tests.


## Usage

### Description
##### Services are available on addresses:
* ###### **Api service** - *https://localhost:443*
Documentation of Api service, can be found on:
*https://localhost/swagger/index.html* 

![Api docs](https://imgur.com/EK5g2PD.png)
![Api docs](https://imgur.com/GKY0X4P.png)
![Api docs](https://imgur.com/B5hWFCn.png)
It's interactive and pretty simple in use, so there's no need to explain details.
After executing any command, you will see CURL examples.


#### WebUI
UI layer doesn't exist yet. This is what I want to add in the future. 

