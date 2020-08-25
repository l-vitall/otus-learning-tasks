FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS runtime
LABEL maintainer="Vitaliy L"
WORKDIR /app
COPY /bin/Debug/netcoreapp3.1/publish/ ./
EXPOSE 5060
ENTRYPOINT ["dotnet", "Calories.Recommendation.dll"]