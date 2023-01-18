:: Remove old build files
rmdir "build" /s /q
rmdir "backend\bin" /s /q
rmdir "backend\obj" /s /q
rmdir "backend\build" /s /q
rmdir "frontend\build" /s /q
rmdir "frontend\src\Api\Generated" /s /q

:: Build the backend
dotnet publish backend/Backend.csproj /p:PublishProfile=backend\Properties\PublishProfiles\FolderProfile.pubxml --configuration Release

:: Frontend
cd frontend
npm install
npm run generate:api
npm run build:wasm
npm run build
cd ..

:: Copy the frontend build to the build folder
rmdir "build\wwwroot" /s /q
move "frontend\build" "build\wwwroot"