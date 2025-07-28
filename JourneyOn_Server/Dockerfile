# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy the solution file
COPY JourneyOn_Server.sln .


# Copy project files for better caching
COPY API/API.csproj API/
COPY Application/Application.csproj Application/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Tests/Tests.csproj Tests/

# Restore dependencies
RUN dotnet restore JourneyOn_Server.sln

# Copy the source code
COPY API/ API/
COPY Application/ Application/
COPY Infrastructure/ Infrastructure/

# Build the application
RUN dotnet publish API/API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy published application
COPY --from=build /app/publish .

# Configure environment
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Production

# Expose port
EXPOSE 80

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:80/weatherforecast || exit 1

# Entry point
ENTRYPOINT ["dotnet", "API.dll"]