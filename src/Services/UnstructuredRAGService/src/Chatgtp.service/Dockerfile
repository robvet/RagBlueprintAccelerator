#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Chatgtp.service/chatgtp.service.csproj", "Chatgtp.service/"]
COPY ["chatgtp-shared-models/chatgtp-shared-models.csproj", "chatgtp-shared-models/"]
COPY ["plumbing/SharedUtilities/SharedUtilities.csproj", "plumbing/SharedUtilities/"]
RUN dotnet restore "Chatgtp.service/chatgtp.service.csproj"
COPY . .
WORKDIR "/src/Chatgtp.service"
RUN dotnet build "chatgtp.service.csproj" -c "$BUILD_CONFIGURATION" -o /app/build

FROM build AS publish
RUN dotnet publish "chatgtp.service.csproj" -c "$BUILD_CONFIGURATION" -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "chatgtp.service.dll"]