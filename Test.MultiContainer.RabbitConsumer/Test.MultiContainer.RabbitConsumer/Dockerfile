FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

RUN ls
COPY . ./
RUN ls
RUN dotnet restore
RUN dotnet publish -c Release -o out

FROM mcr.microsoft.com/dotnet/aspnet:6.0
RUN ls
WORKDIR /app
COPY --from=build-env /app/out .
RUN ls
ENTRYPOINT [ "dotnet", "Test.MultiContainer.RabbitConsumer.dll" ]

