# ========================
# Step 1: Build
# ========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# 1. Copy solution
COPY ["Astralis.sln", "./"]

# 2. Copy API project
COPY ["Astralis_API/Astralis_API.csproj", "Astralis_API/"]

# 3. Copy Shared project
COPY ["Shared/Astralis.Shared/Astralis.Shared.csproj", "Shared/Astralis.Shared/"]

# 4. Copy Test project (ADDED HERE TO FIX THE ERROR)
COPY ["Astralis_APITests/Astralis_APITests.csproj", "Astralis_APITests/"]

# 5. Restore
RUN dotnet restore "Astralis.sln"

# 6. Copy all source code
COPY . .

# 7. Build
WORKDIR "/src/Astralis_API"
RUN dotnet publish "Astralis_API.csproj" -c Release -o /app/publish

# ========================
# Step 2: Run
# ========================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Astralis_API.dll"]