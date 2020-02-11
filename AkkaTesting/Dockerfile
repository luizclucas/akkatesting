FROM mcr.microsoft.com/dotnet/core/runtime:3.0 AS base

WORKDIR /app
COPY bin/Release/PublishOutput /app
ENV COMPlus_PerfMapEnabled 1
EXPOSE 10050 10150
ENTRYPOINT ["dotnet", "/app/AkkaTesting.dll"]
