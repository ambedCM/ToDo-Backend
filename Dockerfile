FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["ToDo-Backend.csproj", "./"]
RUN dotnet restore "ToDo-Backend.csproj"

COPY . .
RUN dotnet publish "ToDo-Backend.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "ToDo-Backend.dll"]
