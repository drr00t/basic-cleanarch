FROM mcr.microsoft.com/dotnet/runtime:7.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Stock/Stock.csproj", "Stock/"]
RUN dotnet restore "Stock/Stock.csproj"
COPY . .
WORKDIR "/src/Stock"
RUN dotnet build "Stock.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Stock.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Stock.dll"]
