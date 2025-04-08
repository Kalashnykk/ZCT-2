# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

# Copy the project file and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the code and publish
COPY . .
RUN dotnet publish -c Release -o /app/out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose HTTP port
EXPOSE 80

# Set environment variables for Azure Cognitive Services
ENV VISION_KEY=your_vision_key
ENV VISION_ENDPOINT=your_vision_endpoint

# Start the application
ENTRYPOINT ["dotnet", "server.dll"]
