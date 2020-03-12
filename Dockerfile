FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

COPY /app/*.csproj ./
COPY /app/* ./
RUN rm -rf *build*
RUN rm -rf *obj*
RUN rm -f AssemblyInfo.cs

RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "twitch-emotes-discord.dll"]
