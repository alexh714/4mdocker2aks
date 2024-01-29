#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
# syntax=docker/dockerfile:1
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /cron-app
    
# Copy csproj and restore as distinct layers
COPY ./*.csproj ./
# COPY ./ProjectName.sln ./
RUN dotnet restore ./
    
# Copy everything else and build
COPY ./ ./
RUN dotnet publish -c Release -o out
    
# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /cron-app
COPY --from=build-env /cron-app/out .

#Install Cron
RUN apt-get update
RUN apt-get -y install cron

#add our cronjob
RUN echo "* * * * * dotnet CommentsWebAPIDatabaseSynchronization /dev/stdout 2>&1" >> /etc/crontab

# ENTRYPOINT ["dotnet", "ProjectExe.dll"]#\CommentsWebAPIDatabaseSynchronization.dll
CMD /bin/bash