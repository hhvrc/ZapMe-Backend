:: Remove build folder
rmdir "build" /s /q

:: Remove build files in backend folder
rmdir "backend\bin"
rmdir "backend\obj"
rmdir "backend\build"

:: Build the backend
dotnet publish backend/Backend.csproj /p:PublishProfile=backend\Properties\PublishProfiles\FolderProfile.pubxml --configuration Release

:: Ensure we have the tools we need
dotnet tool restore

:: Export the backend API
mkdir "frontend/api"
dotnet tool run swagger tofile --yaml --output frontend/api/openapi.yaml build/Server.dll v1

:: Frontend
cd frontend
rmdir "build" /s /q
rmdir "src/Api/Generated" /s /q
call npm install
call npm run generate:api
call npm run build:wasm
call npm run build
cd ..

:: Copy the frontend build to the build folder
rmdir "build\wwwroot" /s /q
move "frontend\build" "build\wwwroot"