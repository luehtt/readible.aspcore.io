# Readible

This project was generated with .NET Core.

## Development server
Run `dotnet watch run` for a dev server. Navigate to `https://localhost:5001/`. The app will automatically reload if you change any of the source files.

## Requirement

Visual Studio 2017 [(Download)](https://visualstudio.microsoft.com/downloads) (optional)  
MSBuild 15 [(Download)](https://docs.microsoft.com/en-us/visualstudio/msbuild/msbuild) (built-in Visual Studio)    
.NET core 2.2 [(Download)](https://dotnet.microsoft.com/download)  
PostgreSQL 11 [(Download)](https://www.postgresql.org/download)

## How to run

  1. create database `readible` in PostgreSQL  
  2. open `appsettings.json` and edit `Db`  
  3. cd to app folder 
  4. run `dotnet watch run` or `dotnet run` (make sure all NuGet package loaded)
  5. go to address `https://localhost:5001/api/migrate`
  6. go to address `https://localhost:5001/api/seed` (optional)

This app is written to target **readible.ng.io** as client so make sure it is running.

## NuGet package
  - [Microsoft.AspNetCore.App](https://dotnet.microsoft.com/apps/aspnet): ASP.NET Core  
  - [Microsoft.AspNetCore.Cors](https://github.com/aspnet/CORS): CORS Setting   
  - System.IdentityModel.Tokens.Jwt
  - Microsoft.AspNetCore.Authentication.JwtBearer
  - [Npgsql.EntityFrameworkCore.PostgreSQL 2.2.4](https://github.com/npgsql/Npgsql.EntityFrameworkCore.PostgreSQL): PostgreSQL ORM   
  - [SixLabors.ImageSharp](https://github.com/SixLabors/ImageSharp): Image Manipulator  
  - [BCrypt.Net-Next](https://github.com/BcryptNet/bcrypt.net): Password Encryptor  
  - [Bogus](https://github.com/bchavez/Bogus): Data Generator   
