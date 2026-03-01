param(
    [string]$ResourceGroup = "MyResourceGroup",
    [string]$Location = "eastus",
    [string]$VMName = "MyVM",
    [string]$VMSize = "Standard_B1s"
)

az group create --name $ResourceGroup --location $Location
az configure --defaults group=$ResourceGroup location=$Location
az vm create --resource-group $ResourceGroup --name $VMName --image UbuntuLTS --size $VMSize --public-ip-sku Standard --admin-username azureuser --generate-ssh-keys
