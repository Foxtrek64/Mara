FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Mara.WebAdmin/Mara.WebAdmin.csproj", "Mara.WebAdmin/"]
RUN dotnet restore "Mara.WebAdmin/Mara.WebAdmin.csproj"
COPY . .
WORKDIR "/src/Mara.WebAdmin"
RUN dotnet build "Mara.WebAdmin.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Mara.WebAdmin.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Mara.WebAdmin.dll"]
