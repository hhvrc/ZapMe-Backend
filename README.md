# ZapMe Backend

Prod
![Prod Build](https://github.com/hhvrc/ZapMe/actions/workflows/build.yml/badge.svg?branch=master)
![Prod Tests](https://github.com/hhvrc/ZapMe/actions/workflows/test.yml/badge.svg?branch=master)

Dev
![Dev Build](https://github.com/hhvrc/ZapMe/actions/workflows/build.yml/badge.svg?branch=dev)
![Dev Tests](https://github.com/hhvrc/ZapMe/actions/workflows/test.yml/badge.svg?branch=dev)

## Building

```bash
sudo apt update
sudo apt install wget git build-essential -y

git clone --recurse-submodules -j8 https://github.com/hhvrc/ZapMe
cd ZapMe/
./build-linux.sh
```
