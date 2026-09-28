FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.csproj ./
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

# Folder for the SQLite database (mounted as a Docker volume).
# The aspnet image runs as the non-root 'app' user, so it must own this folder.
USER root
RUN mkdir -p /app/Data && chown -R app:app /app/Data
USER app

ENTRYPOINT ["dotnet", "HeimevernetInnlevering1.dll"]
