Installation
============


#  1. System requirements

The following system requirements are needed, split by dev or production.
Where dev is used for development, testing, debugging etc.
Where production is used to simply run.

## 1.1 Requirements

The following requirements are for both production as dev, all platforms

* dotnet7
* nodejs 18 (lts/hydrogen)

__Ubuntu amd64__

```bash
# Debian based
apt install dotnet7
apt install nodejs
```

__Raspberry Pi4__

```bash
https://raw.githubusercontent.com/pjgpetecodes/dotnet7pi/main/install.sh
```

## Linux / Ubuntu

* dotnet7
* nodejs 18 (lts/hydrogen)



# Building

To build app, you should

First set environment variables.

```env
export VERSION=dev
export DIR_BUILD=.build
```



```bash
export VERSION=${VERSION:-dev}
export PLXR_DIR_BUILD="${PLXR_DIR_BUILD:-.build}"
export PLXR_DIR_WEBROOT="${PLXR_DIR_BUILD:-.build}"

# Step into src dir
cd src/

### 1. DOTNET Restore ###
# ====================
# @todo why is this necessary?
dotnet restore "src/WebAPI/WebAPI.csproj" --locked-mode

### 2. DOTNET Build ###
# ====================
# @todo why is this necessary?
dotnet restore "src/WebAPI/WebAPI.csproj" --locked-mode
```


# Resources

* https://www.mono-project.com/download/stable/#download-lin