# AZ204 Project - Web API to upload images into blob Storage

> ## Todo
>
> - Create the Azure function

## Purpose

This project will help me to understand better the different topics of the Azure 204 Certification

- Azure App Service
- Azure Blob Storage
- Azure Functions
- Azure App Registration

## How the project integrates each service/topic

- Azure App Service: The application is a .NET Web API hosted in an Azure App Service, deployed with Github Actions

- Azure Blob Storage: The API uses the Blob Storage Nuget packages to upload the images into the Containers of our Storage Account, list and download the images.

- Azure Functions: An Azure Time Trigger Function will check weekly for images that have more than 2 days on the container and will delete it to keep the container clear.

### Another services to integrate

2. Application Insights
