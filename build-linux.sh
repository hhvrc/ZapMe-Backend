#!/bin/bash

function echo_blue {
    echo -e "\e[34m$1\e[0m"
}
function echo_cyan {
    echo -e "\e[36m$1\e[0m"
}
function echo_green {
    echo -e "\e[32m$1\e[0m"
}
function refresh_environment_if_required {
    if [[ $ENVIRONMENT_REFRESH_REQUIRED = true ]]; then
        echo_blue "Refreshing environment variables"
        source ~/.bashrc
        ENVIRONMENT_REFRESH_REQUIRED=false
    fi
}

# Set initial values
DOTNET_INSTALLED=false
DOTNET_6_INSTALLED=false
DOTNET_7_INSTALLED=false
ENVIRONMENT_REFRESH_REQUIRED=false

# Check if dotnet is installed
if hash dotnet 2>/dev/null; then
    DOTNET_INSTALLED=true
    DOTNET_VERSIONS=$(dotnet --list-sdks)

    # Check if dotnet 6 is installed
    if [[ $DOTNET_VERSIONS == *"6.0.309"* ]]; then
        DOTNET_6_INSTALLED=true
    fi

    # Check if dotnet 7 is installed
    if [[ $DOTNET_VERSIONS == *"7.0.102"* ]]; then
        DOTNET_7_INSTALLED=true
    fi
fi

# Download dotnet install script
if [[ $DOTNET_INSTALLED = false ]] || [[ $DOTNET_6_INSTALLED = false ]] || [[ $DOTNET_7_INSTALLED = false ]]; then
    if [[ $DOTNET_INSTALLED = false ]]; then
        echo_cyan "Installing dotnet, and SDKs 6 and 7"
    elif [[ $DOTNET_6_INSTALLED = false ]] && [[ $DOTNET_7_INSTALLED = false ]]; then
        echo_cyan "Updating dotnet: Installing 6 and 7 SDKs"
    elif [[ $DOTNET_6_INSTALLED = false ]]; then
        echo_cyan "Updating dotnet: Installing dotnet 6 SDK"
    elif [[ $DOTNET_7_INSTALLED = false ]]; then
        echo_cyan "Updating dotnet: Installing dotnet 7 SDK"
    fi

    # Download dotnet install script
    wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
    chmod +x ./dotnet-install.sh

    # Install dotnet 6 if not already installed
    if [[ $DOTNET_6_INSTALLED = false ]]; then
        ./dotnet-install.sh --version 6.0.309
    fi

    # Install dotnet 7 if not already installed
    if [[ $DOTNET_7_INSTALLED = false ]]; then
        ./dotnet-install.sh --version 7.0.102
    fi

    # Remove dotnet install script
    rm dotnet-install.sh

    # Set environment variables and add them to .bashrc to persist
    export DOTNET_ROOT="$HOME/.dotnet"
    export PATH="$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools"
    echo 'export DOTNET_ROOT="$HOME/.dotnet"' >> ~/.bashrc
    echo 'export PATH="$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools"' >> ~/.bashrc

    echo_green "Dotnet installed"
    
    ENVIRONMENT_REFRESH_REQUIRED=true
else
    echo_green "Found dotnet runtime with SDKs 6 and 7"
fi

# Check if flatc is installed
if hash flatc 2>/dev/null; then
    echo_green "Found flatc"
else
    echo_cyan "Installing flatc"
    git clone https://github.com/google/flatbuffers.git tmp-flatbuffers
    cd tmp-flatbuffers
    cmake -G "Unix Makefiles"
    make -j$(nproc)
    mkdir ~/.flatbuffers
    cp flatc ~/.flatbuffers/
    cd ..
    rm -rf tmp-flatbuffers
    export PATH="$PATH:$HOME/.flatbuffers"
    echo 'export PATH="$PATH:$HOME/.flatbuffers"' >> ~/.bashrc
fi

# Remove all build artifacts
rm -rf "build"
rm -rf "backend/bin"
rm -rf "backend/obj"
rm -rf "backend/build"

# Build the backend
echo_cyan "Building backend"
dotnet tool restore
dotnet publish backend/Backend.csproj /p:PublishProfile=backend/Properties/PublishProfiles/Linux-x64.pubxml -c Release
echo_green "Backend build complete"
