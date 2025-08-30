migrations (PS):
$ cd api\Data
$ dotnet ef migrations add Initial -c ApplicationDbContext -s "../JwtAuthentication.csproj"
$ dotnet ef database update -c ApplicationDbContext -s "../JwtAuthentication.csproj"

remove last migartion (PS):
$ dotnet ef migrations remove -c ApplicationDbContext -s "../JwtAuthentication.csproj"

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