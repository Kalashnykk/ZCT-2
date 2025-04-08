# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY server/*.csproj ./server/
WORKDIR /app/server
RUN dotnet restore

# Copy the remaining source code and build the project
COPY . .
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose the HTTP port
EXPOSE 80

# Set Azure environment variables (can be overridden in the hosting environment)
ENV VISION_KEY=your_vision_key
ENV VISION_ENDPOINT=your_vision_endpoint

# Start the application
ENTRYPOINT ["dotnet", "server.dll"]
