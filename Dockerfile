FROM mcr.microsoft.com/dotnet/sdk:9.0-alpine AS build
WORKDIR /src
COPY *.sln .
COPY ManagerDiscordBot/*.csproj ./ManagerDiscordBot/
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /publish --no-restore


FROM mcr.microsoft.com/dotnet/runtime:9.0-alpine
WORKDIR /app
COPY --from=build /publish .
RUN apk add --no-cache icu-libs

ENTRYPOINT ["dotnet", "OpenShock.ManagerDiscordBot.dll"]