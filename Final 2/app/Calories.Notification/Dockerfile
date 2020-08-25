FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS runtime
LABEL maintainer="Vitaliy L"
WORKDIR /app
COPY /bin/Debug/netcoreapp3.1/publish/ ./
EXPOSE 5040
ENTRYPOINT ["dotnet", "Calories.Notification.dll"]