FROM microsoft/dotnet:sdk AS build
WORKDIR /app

# copy csproj and restore as distinct layers
COPY **.csproj ./
RUN dotnet restore

# copy everything else and build app
COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.1-aspnetcore-runtime AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "IdentityServer.dll"]