#!/bin/bash

# Azure Infrastructure Automation Script

set -e

# Configuration
RESOURCE_GROUP="myResourceGroup"
LOCATION="eastus"
STORAGE_ACCOUNT="mystorageacct"
APP_SERVICE_PLAN="myAppServicePlan"
APP_NAME="myApp"
SQL_SERVER="mysqlserver"
SQL_DB="mydb"
ADMIN_USERNAME="azureadmin"

# Function to check if Azure CLI is installed
check_azure_cli() {
    if ! command -v az &> /dev/null; then
        echo "Azure CLI is not installed. Please install it first."
        exit 1
    fi
    echo "Azure CLI found."
}

# Function to create resource group
create_resource_group() {
    echo "Creating resource group: $RESOURCE_GROUP"
    az group create --name "$RESOURCE_GROUP" --location "$LOCATION"
}

# Function to create storage account
create_storage_account() {
    echo "Creating storage account: $STORAGE_ACCOUNT"
    az storage account create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$STORAGE_ACCOUNT" \
        --location "$LOCATION" \
        --sku Standard_LRS
}

# Function to create App Service Plan
create_app_service_plan() {
    echo "Creating App Service Plan: $APP_SERVICE_PLAN"
    az appservice plan create \
        --name "$APP_SERVICE_PLAN" \
        --resource-group "$RESOURCE_GROUP" \
        --sku B1 \
        --is-linux
}

# Function to create Web App
create_web_app() {
    echo "Creating Web App: $APP_NAME"
    az webapp create \
        --resource-group "$RESOURCE_GROUP" \
        --plan "$APP_SERVICE_PLAN" \
        --name "$APP_NAME" \
        --runtime "NODE|18-lts"
}

# Function to create SQL Server and Database
create_sql_resources() {
    echo "Creating SQL Server and Database"
    az sql server create \
        --resource-group "$RESOURCE_GROUP" \
        --name "$SQL_SERVER" \
        --location "$LOCATION" \
        --admin-user "$ADMIN_USERNAME" \
        --admin-password "P@ssw0rd123!"
    
    az sql db create \
        --resource-group "$RESOURCE_GROUP" \
        --server "$SQL_SERVER" \
        --name "$SQL_DB"
}

# Main execution
main() {
    echo "Starting Azure Infrastructure Deployment..."
    check_azure_cli
    create_resource_group
    create_storage_account
    create_app_service_plan
    create_web_app
    create_sql_resources
    echo "Infrastructure deployment completed successfully!"
}

main