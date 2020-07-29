FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS runtime
LABEL maintainer="Vitaliy L"
WORKDIR /app
COPY CruidWebApi/bin/Debug/netcoreapp3.1/publish/* ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "CruidWebApi.dll"]