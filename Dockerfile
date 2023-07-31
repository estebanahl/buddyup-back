FROM mcr.microsoft.com/dotnet/aspnet:6.0 as base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 as build
WORKDIR /src
COPY ["buddyUp.csproj", "."]
RUN dotnet restore "./buddyUp.csproj"
COPY . . 
WORKDIR "/src/."
RUN dotnet build "buddyUp.csproj" -c Release -o /app/build

FROM build as publish
RUN dotnet publish "buddyUp.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base as final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "buddyUp.dll"]