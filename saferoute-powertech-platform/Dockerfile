FROM mcr.microsoft.com/dotnet/sdk:10.0 AS builder
WORKDIR /app
COPY Powertech.Platform/*.csproj Powertech.Platform/
RUN dotnet restore ./Powertech.Platform
COPY . .
RUN dotnet publish ./Powertech.Platform -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=builder /app/out .
EXPOSE 8080
ENTRYPOINT ["dotnet", "Powertech.Platform.dll"]
