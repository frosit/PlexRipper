Working with Buildx
===================

To build and push docker images using buildx.

1. create builder
2. login to registry
3. build

# paltforms

* Buildx: linux/amd64,linux/amd64/v2,linux/amd64/v3,linux/arm64,linux/riscv64,linux/ppc64le,linux/s390x,linux/386,linux/mips64le,linux/mips64,linux/arm/v7,linux/arm/v6
* QEMU:  linux/amd64,linux/arm64,linux/riscv64,linux/ppc64le,linux/s390x,linux/386,linux/mips64le,linux/mips64,linux/arm/v7,linux/arm/v6
* selected: linux/amd64,linux/amd64/v2,linux/amd64/v3,linux/arm64,linux/arm/v7,linux/arm/v6

## SSH authentication

Setup SSH key authentication to raspberry pi

```bash

ssh-add -l # see keys in agent

# if no agent, do
eval $(ssh-agent)

# if no key, add it
ssh-add ~/.ssh/id_rsa

```

## Create builder and connect Pi

You should be able to connect to pi using ssh pi@<ip> using key
Change connection string after ssh://

```bash
docker buildx create --name plexripper_builder --append --node argon --platform linux/arm64,linux/arm/v7,linux/arm/v6 ssh://pi@192.168.178.104 --driver-opt env.BUILDKIT_STEP_LOG_MAX_SIZE=10000000 --driver-opt env.BUILDKIT_STEP_LOG_MAX_SPEED=10000000
```



# Cheatsheet

```bash
# login to registry
docker login

# create builder

docker buildx ls # show builders
docker buildx inspect # show current builder
docker use builder # select a builder

# create builder
docker buildx create --name plexripper_builder --append --node argon --platform linux/arm64,linux/arm/v7,linux/arm/v6 ssh://frosit@192.168.178.104 --driver-opt env.BUILDKIT_STEP_LOG_MAX_SIZE=10000000 --driver-opt env.BUILDKIT_STEP_LOG_MAX_SPEED=10000000

# build
docker buildx build --platform linux/amd64,linux/amd64/v2,linux/amd64/v3,linux/arm64,linux/arm/v7 -t frosit/plexripper:dev . --push


# Images
docker inspect image -f "{{.Os}}/{{.Architecture}}" 
```


# Resources

*   https://github.com/docker/setup-buildx-action
* https://medium.com/@life-is-short-so-enjoy-it/docker-how-to-build-and-push-multi-arch-docker-images-to-docker-hub-64dea4931df9
* https://github.com/elgohr/Publish-Docker-Github-Action
* https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-restore
* https://devblogs.microsoft.com/dotnet/improving-multiplatform-container-support/