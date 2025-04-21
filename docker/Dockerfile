FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env
WORKDIR /app

RUN mkdir -p Generator.Worker
COPY Generator.Worker/*.csproj Generator.Worker
RUN dotnet restore Generator.Worker

# Software version number
#   - Should correspond to tag
#   - Including default value so if someone just runs "docker build", it will work
ARG SOLUTION_VERSION=docker

COPY . ./

# Handle config.toml.
# Current setup is to ensure that config.toml is in place before building.
# This is useful locally, but in order to publish it, we'll have to
# set up a volume to a local version.
#RUN rm -f Controller/config.toml || true

RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/runtime:9.0
WORKDIR /app
COPY --from=build-env /app/out ./

RUN useradd -ms /bin/bash moduleuser
USER moduleuser

ENTRYPOINT ["dotnet", "Generator.Worker.dll"]