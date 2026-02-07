# syntax=docker/dockerfile:1

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS build
WORKDIR /src

# Copy solution and project files
COPY ["B3cBonsai.sln", "./"]
COPY ["B3cBonsaiWeb/B3cBonsaiWeb.csproj", "B3cBonsaiWeb/"]
COPY ["B3cBonsai.DataAccess/B3cBonsai.DataAccess.csproj", "B3cBonsai.DataAccess/"]
COPY ["B3cBonsai.Models/B3cBonsai.Models.csproj", "B3cBonsai.Models/"]
COPY ["B3cBonsai.Utility/B3cBonsai.Utility.csproj", "B3cBonsai.Utility/"]

# Restore dependencies
RUN dotnet restore

# Copy the rest of the source code
COPY . .

# Build and publish the application
WORKDIR "/src/B3cBonsaiWeb"
RUN dotnet publish "B3cBonsaiWeb.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage/image
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

COPY --from=build /app/publish .

# Ensure wwwroot exists and has correct permissions for the non-root user
RUN mkdir -p wwwroot && chown -R $APP_UID:$APP_UID wwwroot

# Switch to a non-privileged user (defined in the base image) that the app will run under.
# See https://docs.docker.com/go/dockerfile-user-best-practices/
USER $APP_UID

ENTRYPOINT ["dotnet", "B3cBonsaiWeb.dll"]
