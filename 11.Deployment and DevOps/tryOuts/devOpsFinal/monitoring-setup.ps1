# Connect to Azure
Connect-AzAccount

# Set variables
$resourceGroupName = "myResourceGroup"
$vmName = "myVM"
$metricName = "Percentage CPU"
$timeGrain = [System.TimeSpan]::FromMinutes(5)
$statistic = "Average"

# Get the VM resource
$vm = Get-AzVM -ResourceGroupName $resourceGroupName -Name $vmName

# Create a metric alert rule for CPU
$actionGroupName = "cpuAlertGroup"
$alertRuleName = "cpuAlertRule"
$alertDescription = "Alert when CPU exceeds 80%"
$threshold = 80

# Create action group if it doesn't exist
$actionGroup = Get-AzActionGroup -ResourceGroupName $resourceGroupName -Name $actionGroupName -ErrorAction SilentlyContinue
if (-not $actionGroup) {
    New-AzActionGroup -ResourceGroupName $resourceGroupName -Name $actionGroupName
}

# Create metric alert rule
Add-AzMetricAlertRuleV2 -Name $alertRuleName `
    -ResourceGroupName $resourceGroupName `
    -TargetResourceId $vm.Id `
    -MetricName $metricName `
    -Operator "GreaterThan" `
    -Threshold $threshold `
    -WindowSize $timeGrain `
    -Frequency $timeGrain `
    -Statistic $statistic `
    -Description $alertDescription `
    -Severity 2 `
    -ActionGroupId $actionGroup.Id