# ===== Build Stage =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy .sln และ .csproj ก่อน แล้ว restore
COPY ExcelEditor.sln ./
COPY ExcelEditer/ExcelEditor.csproj ./ExcelEditer/
RUN dotnet restore "ExcelEditor.sln"

# Copy โค้ดทั้งหมดแล้ว build
COPY . ./
RUN dotnet publish "ExcelEditer/ExcelEditor.csproj" -c Release -o /app

# ===== Runtime Stage =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ExcelEditer.dll"]