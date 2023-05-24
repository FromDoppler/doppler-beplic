#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["doppler-beplic/doppler-beplic.csproj", "doppler-beplic/"]
RUN dotnet restore "doppler-beplic/doppler-beplic.csproj"
COPY . .
WORKDIR "/src/doppler-beplic"
RUN dotnet build "doppler-beplic.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "doppler-beplic.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "doppler-beplic.dll"]