ARG VERSION=0.0.0
ARG PORT=7000

# Stage 1 - Build the Nuxt front-end
FROM --platform=$TARGETPLATFORM oven/bun:alpine AS client-build

ARG PORT
ARG TARGETPLATFORM
ARG TARGETARCH

ENV NUXT_HOST=0.0.0.0
ENV NUXT_PORT=${PORT}
ENV NUXT_PUBLIC_API_PORT=${PORT}
ENV NUXT_PUBLIC_IS_DOCKER=true

WORKDIR /tmp/ClientApp

COPY ./src/WebAPI/ClientApp/package.json ./src/WebAPI/ClientApp/bun.lockb ./
RUN bun install --frozen-lockfile

COPY ./src/WebAPI/ClientApp/ ./
RUN bun run generate --fail-on-error

# Stage 2 - Build the .NET back-end
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS back-end

ARG VERSION
ARG BUILDPLATFORM

WORKDIR /src

# Copy and restore dependencies

# @TODO redundant steps below
## Domain Projects
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Environment/Environment.csproj", "src/Environment/"]
COPY ["src/FluentResultExtensions/FluentResultExtensions.csproj", "src/FluentResultExtensions/"]
COPY ["src/Logging/Logging.csproj", "src/Logging/"]

## Core Projects
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Application.Contracts/Application.Contracts.csproj", "src/Application.Contracts/"]

## Infrastructure Projects
### Data Access
COPY ["src/Data/Data.csproj", "src/Data/"]
COPY ["src/Data.Contracts/Data.Contracts.csproj", "src/Data.Contracts/"]
### File System
COPY ["src/FileSystem/FileSystem.csproj", "src/FileSystem/"]
COPY ["src/FileSystem.Contracts/FileSystem.Contracts.csproj", "src/FileSystem.Contracts/"]
### Plex API
COPY ["src/PlexApi/PlexApi.csproj", "src/PlexApi/"]
COPY ["src/PlexApi.Contracts/PlexApi.Contracts.csproj", "src/PlexApi.Contracts/"]
### Settings
COPY ["src/Settings/Settings.csproj", "src/Settings/"]
COPY ["src/Settings.Contracts/Settings.Contracts.csproj", "src/Settings.Contracts/"]

## WebAPI Projects
COPY ["src/WebAPI/WebAPI.csproj", "src/WebAPI/"]
COPY ["src/WebAPI.Contracts/WebAPI.Contracts.csproj", "src/WebAPI.Contracts/"]


RUN dotnet restore "src/WebAPI/WebAPI.csproj" --locked-mode

# Copy the entire source code
COPY . .

WORKDIR "/src/src/WebAPI"

# Build and publish
RUN dotnet publish "WebAPI.csproj" -c Release -o /app/publish /p:AssemblyVersion=$VERSION

# Stage 3 - Runtime Image
FROM ghcr.io/linuxserver/baseimage-alpine:3.20 AS final

ARG VERSION
ARG PORT
ARG TARGETPLATFORM

ENV PORT=${PORT}
ENV VERSION=${VERSION}
ENV DOTNET_ENVIRONMENT=Production
ENV DOTNET_URLS=http://+:${PORT}
ENV ASPNETCORE_URLS=http://+:${PORT}
ENV S6_SERVICES_GRACETIME=15000


## Install dotnet runtime
RUN \
   echo "**** Updating package information ****" && \
   apk update && \
   echo "**** Install pre-reqs ****" && \
   apk add --no-cache bash icu-libs krb5-libs libgcc libintl libssl3 libstdc++ zlib && \
   echo "**** Installing dotnet runtime for $TARGETPLATFORM ****" && \
   mkdir -p /usr/share/dotnet && \
   wget https://dot.net/v1/dotnet-install.sh -O /tmp/dotnet-install.sh && \
   chmod +x /tmp/dotnet-install.sh && \
   if  [ "$TARGETPLATFORM" = "linux/arm/v7" ] ; then \
       /tmp/dotnet-install.sh --version 8.0.10 --runtime aspnetcore --install-dir /usr/share/dotnet --architecture arm ; \
   elif [ "$TARGETPLATFORM" = "linux/arm64/v8" ] ; then \
       /tmp/dotnet-install.sh --version 8.0.10 --runtime aspnetcore --install-dir /usr/share/dotnet --architecture arm64 ; \
   else \
       /tmp/dotnet-install.sh --version 8.0.10 --runtime aspnetcore --install-dir /usr/share/dotnet --architecture x64 ; \
   fi

## Make dotnet command available
ENV PATH="$PATH:/usr/share/dotnet"

WORKDIR /app

COPY --from=back-end /app/publish .
COPY --from=client-build /tmp/ClientApp/.output/public /app/wwwroot

## Copy the s6-overlay config files
COPY docker/etc/ /etc/

# Set version label
LABEL company="PlexRipper"
LABEL maintainer="plexripper@protonmail.com"

EXPOSE ${PORT}
VOLUME /Config /Downloads /Movies /TvShows

# Set the entrypoint (optional)
ENTRYPOINT ["/init"]
