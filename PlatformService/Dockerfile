FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-env
WORKDIR /app

COPY *.csproj ./
COPY . ./

RUN dotnet restore

RUN dotnet publish -c release -o out

# build runtime
FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /app

COPY --from=build-env /app/out .
ENTRYPOINT [ "dotnet", "PlatformService.dll" ]

