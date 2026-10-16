# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build

WORKDIR /app

# Copy solution and project files
COPY ["Obilet.JourneySearch.slnx", "./"]
COPY ["src/Obilet.JourneySearch.Web/Obilet.JourneySearch.Web.csproj", "src/Obilet.JourneySearch.Web/"]
COPY ["tests/Obilet.JourneySearch.Tests/Obilet.JourneySearch.Tests.csproj", "tests/Obilet.JourneySearch.Tests/"]

# Restore dependencies
RUN dotnet restore "Obilet.JourneySearch.slnx"

# Copy entire source code
COPY . .

# Build the application
RUN dotnet build "src/Obilet.JourneySearch.Web/Obilet.JourneySearch.Web.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "src/Obilet.JourneySearch.Web/Obilet.JourneySearch.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime

WORKDIR /app

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=5s --retries=3 \
	CMD curl -f http://localhost:8080/ || exit 1

# Entry point
ENTRYPOINT ["dotnet", "Obilet.JourneySearch.Web.dll"]
