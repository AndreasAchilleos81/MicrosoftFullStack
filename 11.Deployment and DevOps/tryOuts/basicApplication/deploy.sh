#!/bin/bash

set -e

# Variables
RESOURCE_GROUP="test-resource-group"
APP_NAME="myNodeApp"
LOCATION="westeurope"

echo "Installing dependencies..."
npm install

# echo "Building application..."
# npm run build

echo "Logging into Azure..."
az login

echo "Creating resource group..."
az group create --name "$RESOURCE_GROUP" --location "$LOCATION"

echo "Creating App Service plan..."
az appservice plan create --name "${APP_NAME}Plan" --resource-group "$RESOURCE_GROUP" --sku F1 --is-linux

echo "Creating web app..."
az webapp create --resource-group "$RESOURCE_GROUP" --plan "${APP_NAME}Plan" --name "$APP_NAME" --runtime "NODE|18-lts"

echo "Deploying application..."
az webapp up --resource-group "$RESOURCE_GROUP" --name "$APP_NAME" --runtime "node|18-lts"

echo "Deployment completed successfully!"