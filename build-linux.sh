#!/bin/bash

# Check if dotnet is installed
if hash dotnet 2>/dev/null; then
    DOTNET_INSTALLED=true
    DOTNET_VERSIONS=$(dotnet --list-sdks)

    # Check if dotnet 6 is installed
    if [[ $DOTNET_VERSIONS == *"6.0.309"* ]]
    then
        echo "Found dotnet 6"
        DOTNET_6_INSTALLED=true
    else
        echo "Preparing to install dotnet 6"
        DOTNET_6_INSTALLED=false
    fi

    # Check if dotnet 7 is installed
    if [[ $DOTNET_VERSIONS == *"7.0.102"* ]]
    then
        echo "Found dotnet 7"
        DOTNET_7_INSTALLED=true
    else
        echo "Preparing to install dotnet 7"
        DOTNET_7_INSTALLED=false
    fi
else
    echo "Preparing to install dotnet"
    DOTNET_INSTALLED=false
    DOTNET_6_INSTALLED=false
    DOTNET_7_INSTALLED=false
fi

# Download dotnet install script
if [ $DOTNET_INSTALLED = false ] || [ $DOTNET_6_INSTALLED = false ] || [ $DOTNET_7_INSTALLED = false ]; then
    echo "Installing dotnet"

    # Set environment refresh flag
    ENVIRONMENT_REFRESH_REQUIRED=true

    # Download dotnet install script
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
    chmod +x ./dotnet-install.sh

    # Install dotnet 6 if not already installed
    if [ $DOTNET_6_INSTALLED = false ]
    then
        ./dotnet-install.sh --version 6.0.309
    fi

    # Install dotnet 7 if not already installed
    if [ $DOTNET_7_INSTALLED = false ]
    then
        ./dotnet-install.sh --version 7.0.102
    fi

    # Remove dotnet install script
    rm dotnet-install.sh

    # Set environment variables
    echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
    echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools' >> ~/.bashrc
else
    ENVIRONMENT_REFRESH_REQUIRED=false
fi

# Install node version manager if not already installed (This doesnt work specifically for checking if nvm is installed, but script is fast enough to not matter)
if hash nvm 2>/dev/null; then
    echo "Found nvm"
else
    echo "Installing nvm"

    # Set environment refresh flag
    ENVIRONMENT_REFRESH_REQUIRED=true

    # Download and execute nvm install script
    wget -qO- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.3/install.sh | bash

    # Set environment variables
    export NVM_DIR="$([ -z "${XDG_CONFIG_HOME-}" ] && printf %s "${HOME}/.nvm" || printf %s "${XDG_CONFIG_HOME}/nvm")"
    [ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"
fi

# Refresh environment
if [ $ENVIRONMENT_REFRESH_REQUIRED = true ]; then
    echo "Refreshing environment"
    source ~/.bashrc
fi

# Check if node 18 is installed
if hash node 2>/dev/null; then
    node_version=$(node -v)
    if [[ $node_version != *"v18"* ]]; then
        INSTALL_NODE_18=true
    else
        INSTALL_NODE_18=false
    fi
else
    INSTALL_NODE_18=true
fi

# Install node 18 if not already installed
if [ $INSTALL_NODE_18 = true ]; then
    echo "Installing node 18"
    nvm install 18
    nvm use 18
else
    echo "Found node 18"
fi

# Remove old build files
rm -rf "build"
rm -rf "backend/bin"
rm -rf "backend/obj"
rm -rf "backend/build"

# Build the project
dotnet tool restore
dotnet publish backend/Backend.csproj /p:PublishProfile=backend/Properties/PublishProfiles/FolderProfile.pubxml --configuration Release

# Frontend
cd frontend
npm run init
npm run build
cd ..

# Copy the frontend build to the build folder
rm -rf "build/wwwroot"
mv "frontend/dist" "build/wwwroot"