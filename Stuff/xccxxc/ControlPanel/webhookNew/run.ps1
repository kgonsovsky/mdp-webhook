using namespace System.Net

# Input bindings
param($Request, $TriggerMetadata)

# subscriptionNname
$subscriptionNname = $Request.Query.subscriptionNname
if (-not $subscriptionNname) {$subscriptionNname = $Request.Body.subscriptionNname}

# endPoint
$endPoint = $Request.Query.Name
if (-not $endPoint) {$endPoint = $Request.Body.Name}

Write-Host "Creating subscription ${subscriptionName} with end point ${endPoint}..."

$body="OK"

# Associate values to output bindings by calling 'Push-OutputBinding'.
Push-OutputBinding -Name Response -Value ([HttpResponseContext]@{
    StatusCode = [HttpStatusCode]::OK
    Body = $body
})