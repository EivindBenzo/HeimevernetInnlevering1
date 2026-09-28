# Byggesteg
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Heimevernet.Web/Heimevernet.Web.csproj Heimevernet.Web/
RUN dotnet restore Heimevernet.Web/Heimevernet.Web.csproj

COPY Heimevernet.Web/. Heimevernet.Web/
RUN dotnet publish Heimevernet.Web/Heimevernet.Web.csproj -c Release -o /app/publish /p:UseAppHost=false

# Kjøresteg
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "Heimevernet.Web.dll"]
