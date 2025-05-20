FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# คัดลอกไฟล์ solution และ project
COPY ExcelEditor.sln ./
COPY ExcelEditer/*.csproj ./ExcelEditer/

# รัน restore
RUN dotnet restore "ExcelEditor.sln"

# คัดลอกไฟล์ทั้งหมดแล้ว build
COPY . ./
RUN dotnet publish "ExcelEditer/ExcelEditer.csproj" -c Release -o /app

# สร้าง runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ExcelEditer.dll"]