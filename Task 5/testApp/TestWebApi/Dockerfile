FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS runtime
LABEL maintainer="Vitaliy L"
WORKDIR /app
COPY TestWebApi/bin/Debug/netcoreapp3.1/publish/* ./
EXPOSE 8081
ENTRYPOINT ["dotnet", "TestWebApi.dll"]