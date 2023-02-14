#!/bin/bash

# Remove old build files
rm -rf "build"
rm -rf "backend/bin"
rm -rf "backend/obj"
rm -rf "backend/build"

# Build the project
dotnet publish backend/Backend.csproj /p:PublishProfile=backend/Properties/PublishProfiles/FolderProfile.pubxml --configuration Release

# Frontend
cd frontend
npm run init
npm run build
cd ..

# Copy the frontend build to the build folder
rm -rf "build/wwwroot"
mv "frontend/dist" "build/wwwroot"