# Readible

This project was generated with .NET Core 2.2 then upgrade to 5.0. Some of the code might look suspicious.  
Note that the DB in this sample project is NOT indexed at all.  
Some of NuGet package might get updated but not updated in the README.md  

## Development server
Run `dotnet watch run` for a dev server. Navigate to `https://localhost:5001/`. The app will automatically reload if you change any of the source files.

## Requirement

Visual Studio 2019 [(Download)](https://visualstudio.microsoft.com/downloads) (optional)  
MSBuild 15 [(Download)](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild) (built-in Visual Studio)    
.NET Core 5 [(Download)](https://dotnet.microsoft.com/download/dotnet/5.0)  
PostgreSQL 13 [(Download)](https://www.postgresql.org/download)

## How to run

  1. create database `readible` in PostgreSQL  
  2. open `appsettings.json` and edit `Db`  
  3. cd to app folder 
  4. run `dotnet watch run` or `dotnet run` (make sure all NuGet package loaded)
  5. go to address `https://localhost:5001/api/migrate`
  6. go to address `https://localhost:5001/api/seed` (optional)

This app is written to target [(**readible.ng.edu**)](https://github.com/luehtt/readible.ng.edu) as client so make sure it is running.

## NuGet package
  - [Microsoft.AspNetCore.App](https://dotnet.microsoft.com/apps/aspnet): ASP.NET Core  
  - [Microsoft.AspNetCore.Cors](https://github.com/aspnet/CORS): CORS Setting   
  - System.IdentityModel.Tokens.Jwt
  - Microsoft.AspNetCore.Authentication.JwtBearer
  - [Npgsql.EntityFrameworkCore.PostgreSQL](https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL): PostgreSQL ORM   
  - [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp): Image Manipulator  
  - [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net): Password Encryptor  
  - [Bogus](https://github.com/bchavez/Bogus): Data Generator   
