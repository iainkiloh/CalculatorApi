FROM mcr.microsoft.com/dotnet/core/aspnet:2.2 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY *.sln ./
COPY CalculatorApi/CalculatorApi/CalculatorApi.csproj CalculatorApi/CalculatorApi/
RUN dotnet restore
COPY . .
WORKDIR /src/CalculatorApi/CalculatorApi
RUN dotnet build -c Release -o /app

FROM build AS publish
RUN dotnet publish -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "CalculatorApi.dll"]
