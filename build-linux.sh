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
function refresh_cargo_environment_if_required {
    if [[ $CARGO_ENV_REFRESH_REQUIRED = true ]]; then
        echo_blue "Refreshing cargo environment"
        source "$HOME/.cargo/env"
        source "$HOME/.cargo/bin"
        echo 'export PATH="$HOME/.cargo/env:$PATH"' >> ~/.bashrc
        CARGO_ENV_REFRESH_REQUIRED=false
        ENVIRONMENT_REFRESH_REQUIRED=true
    fi
}

# Set initial values
DOTNET_INSTALLED=false
DOTNET_6_INSTALLED=false
DOTNET_7_INSTALLED=false
CARGO_ENV_REFRESH_REQUIRED=false
ENVIRONMENT_REFRESH_REQUIRED=false

# Check if necessary compilers are installed
if ! hash cc 2>/dev/null; then
    COMPILER_MISSING=true
fi
if ! hash gcc 2>/dev/null; then
    COMPILER_MISSING=true
fi
if [[ $COMPILER_MISSING = true ]]; then
    echo_cyan "C/C++ compiler toolchain is not installed, please run the following command:"
    echo "sudo apt update && sudo apt install build-essential -y"
    exit 1
else
    echo_green "Found C/C++ compiler toolchain"
fi

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

    # Set environment variables
    echo 'export DOTNET_ROOT=$HOME/.dotnet' >> ~/.bashrc
    echo 'export PATH=$PATH:$DOTNET_ROOT:$DOTNET_ROOT/tools' >> ~/.bashrc

    echo_green "Dotnet installed"
    
    ENVIRONMENT_REFRESH_REQUIRED=true
else
    echo_green "Found dotnet runtime with SDKs 6 and 7"
fi

# Install node version manager if not already installed (This doesnt work specifically for checking if nvm is installed, but script is fast enough to not matter)
if ! [[ -d "${HOME}/.nvm/.git" ]]; then
    echo_cyan "Installing nvm"

    # Download and execute nvm install script
    wget -qO- https://raw.githubusercontent.com/nvm-sh/nvm/v0.39.3/install.sh | bash

    # Set environment variables
    export NVM_DIR="$([ -z "${XDG_CONFIG_HOME-}" ]] && printf %s "${HOME}/.nvm" || printf %s "${XDG_CONFIG_HOME}/nvm")"
    [[ -s "$NVM_DIR/nvm.sh" ]] && \. "$NVM_DIR/nvm.sh"

    echo_green "Nvm installed"
    
    ENVIRONMENT_REFRESH_REQUIRED=true
else
    echo_green "Found nvm"
fi

# Install rust if not already installed
if ! hash rustc 2>/dev/null; then
    echo_cyan "Installing rust"

    # Set environment refresh flag
    ENVIRONMENT_REFRESH_REQUIRED=true

    # Download rust install script
    wget https://sh.rustup.rs -O rustup.sh
    chmod +x ./rustup.sh

    # Install rust
    ./rustup.sh -y

    # Remove rust install script
    rm rustup.sh

    echo_green "Rust installed"

    CARGO_ENV_REFRESH_REQUIRED=true
else
    echo_green "Found rust"
fi

# Install wasm-pack if not already installed
if ! hash wasm-pack 2>/dev/null; then
    refresh_cargo_environment_if_required

    echo_cyan "Installing wasm-pack"

    # Set environment refresh flag
    ENVIRONMENT_REFRESH_REQUIRED=true

    # Download wasm-pack install script
    wget https://rustwasm.github.io/wasm-pack/installer/init.sh -O wasm-pack-init.sh
    chmod +x ./wasm-pack-init.sh

    # Install wasm-pack
    ./wasm-pack-init.sh -y

    # Remove wasm-pack install script
    rm wasm-pack-init.sh

    echo_green "wasm-pack installed"

    CARGO_ENV_REFRESH_REQUIRED=true
else
    echo_green "Found wasm-pack"
fi

refresh_environment_if_required

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
if [[ $INSTALL_NODE_18 = true ]]; then
    echo_cyan "Installing node 18"
    nvm install 18
    nvm use 18
    echo_green "Node 18 installed"
else
    echo_green "Found node 18"
fi

# Remove all build artifacts
rm -rf "build"
rm -rf "backend/bin"
rm -rf "backend/obj"
rm -rf "backend/build"

# Build the backend
echo_cyan "Building backend"
dotnet tool restore
dotnet publish backend/Backend.csproj /p:PublishProfile=backend/Properties/PublishProfiles/FolderProfile.pubxml -c Release
echo_green "Backend build complete"

# Build the frontend
echo_cyan "Building frontend"
cd frontend
npm run init
npm run build
cd ..
echo_green "Frontend build complete"

# Copy the frontend build to the backend
echo_cyan "Copying frontend build to backend"
rm -rf "build/wwwroot"
mkdir "build/wwwroot"
cp -r "frontend/dist/" "build/wwwroot"
echo_green "Frontend build copied!"