FROM microsoft/dotnet:sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY **.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . ./
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
COPY --from=build /app/aspnetapp/out ./
ENTRYPOINT ["dotnet", "IdentityServerAspNetIdentity.dll"]