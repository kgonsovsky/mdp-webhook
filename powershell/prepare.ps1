#Account          : azurerus001@gmail.com
#SubscriptionName : Azure subscription 1
#SubscriptionId   : 6d9652ae-29d4-471b-9bf4-e23ddaaeb05a
#TenantId         : ed925669-c818-463b-bd8e-5bcb7131f38d
#Environment      : AzureCloud
#
#https://www.red-gate.com/simple-talk/sysadmin/powershell/azure-windows-powershell-basics/
#https://www.veritas.com/support/en_US/article.100046413

Install-Module -Name Az -Scope CurrentUser -Repository PSGallery -Force -AllowClobber
Install-Module -Name Az.EventGrid -Scope CurrentUser -Repository PSGallery -Force -AllowClobber
Import-Module Az
Login-AzureRMLogin
Save-AzureRmContext -Path "C:\AzureProfile.json"
Import-AzureRmContext -Path "C:\AzureProfile.json"