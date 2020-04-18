FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS runtime
LABEL maintainer="Vitaliy L"
WORKDIR /app
COPY published/* ./
EXPOSE 8080
ENTRYPOINT ["dotnet", "SimpleWebApi.dll"]