#!/bin/bash

# Remove build folder
rm -rf "build"

# Remove build files in backend folder
rm -rf "backend/bin"
rm -rf "backend/obj"
rm -rf "backend/build"

# Build the project
dotnet publish backend/Backend.csproj /p:PublishProfile=backend/Properties/PublishProfiles/FolderProfile.pubxml --configuration Release

# Ensure we have the tools we need
dotnet tool restore

# Export the backend API
mkdir "frontend/api"
dotnet tool run swagger tofile --yaml --output frontend/api/openapi.yaml build/Server.dll v1

# Frontend
cd frontend
rm -rf "build"
rm -rf "src/Api/Generated"
npm install
npm run generate:api
npm run build:wasm
npm run build
cd ..

# Copy the frontend build to the build folder
rm -rf "build/wwwroot"
mv "frontend/build" "build/wwwroot"