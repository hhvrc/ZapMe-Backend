# ZapMe Backend

Prod
![Prod Build](https://github.com/hhvrc/ZapMe/actions/workflows/build.yml/badge.svg?branch=master)
![Prod Tests](https://github.com/hhvrc/ZapMe/actions/workflows/test.yml/badge.svg?branch=master)

Dev
![Dev Build](https://github.com/hhvrc/ZapMe/actions/workflows/build.yml/badge.svg?branch=dev)
![Dev Tests](https://github.com/hhvrc/ZapMe/actions/workflows/test.yml/badge.svg?branch=dev)

## Building

### Windows
```ps1
git clone --recurse-submodules -j8 https://github.com/hhvrc/ZapMe-Backend zapme
cd zapme/
powershell.exe -File .\build-windows.ps1 -ExecutionPolicy Bypass
```

### Ubuntu 22.04
```bash
sudo apt update
sudo apt install wget git -y

git clone --recurse-submodules -j8 https://github.com/hhvrc/ZapMe-Backend zapme
cd zapme/
chmod +x build-linux.sh
./build-linux.sh
```

# Contact

[contact@zapme.app](mailto:contact@zapme.app)