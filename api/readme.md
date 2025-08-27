migrations (PS):
$ cd api\Data
$ dotnet ef migrations add Initial -c ApplicationDbContext -s "../JwtAuthentication.csproj"
$ dotnet ef database update -c ApplicationDbContext -s "../JwtAuthentication.csproj"

remove last migartion (PS):
$ dotnet ef migrations remove -c ApplicationDbContext -s "../JwtAuthentication.csproj"