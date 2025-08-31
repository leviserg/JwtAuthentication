migrations (PS):
$ cd api\Data
$ dotnet ef migrations add Initial -c ApplicationDbContext -s "../JwtAuthentication.csproj"
$ dotnet ef database update -c ApplicationDbContext -s "../JwtAuthentication.csproj"

remove last migartion (PS):
$ dotnet ef migrations remove -c ApplicationDbContext -s "../JwtAuthentication.csproj"

Linux notes:
install SDK & tools & runtime:
// sudo add-apt-repository ppa:dotnet/backports
// sudo apt-get update && sudo apt-get install -y dotnet-sdk-9.0
// sudo dotnet tool install --global dotnet-ef
// dotnet tool list --global    { check the lists of available tools before, locally & globally }
// cdcd ../JwtAuthentication/api
// dotnet restore
// dotnet build

install docker & login & set up db
// docker login
// cd ../JwtAuthentication
// export SERVER_PWD="" && export DEVELOPER_LOGIN="" &&  export DEVELOPER_PWD="" && export DEVELOPER_DB=""
// sudo -E docker-compose -f docker-compose.yml up

// apply migration :
// cd ../JwtAuthentication/api/Data
// dotnet ef database update -c ApplicationDbContext -s "../JwtAuthentication.csproj" 

// run app:
// cd ../JwtAuthentication/api && dotnet run
// http://localhost:5057/scalar/v1 || https://localhost:7246/scalar/v1

// clean up docker resources:
// sudo docker ps         - check available containers
// sudo docker images     - check available images
// sudo docker volume ls  - check avalable volumes
// cd ../JwtAuthentication
// sudo docker-compose -f docker-compose.yml down --volumes --rmi all

```
json:
local.settings.json:
{
  "ConnectionStrings": {
    "AppDb": "YourConnectionStringHere",
  },
  "AuthSettings": {
    "TokenKey": "YourSecretLongKeyHere",
    "Audience": "local.app.server", // who should verify token - your BE app
    "Issuer": "local.auth.server" // who creates and signs token - your auth server or microsoft.login..
  }
}
```