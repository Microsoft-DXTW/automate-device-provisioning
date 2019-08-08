# HttpTrigger - C<span>#</span>

The `HttpTrigger` makes it incredibly easy to have your functions executed via an HTTP call to your function.

## How it works

When you call the function, be sure you checkout which security rules you apply. If you're using an apikey, you'll need to include that in your request.

## Learn more

## Publish to Azure

```bash
az functionapp create -n azp-msiotacc-func-dpsapi-pl -g azp-rg-msiotacc-pl --runtime dotnet -s azpmsiotaccstorazfuncpl --consumption-plan-location australiacentral

func azure functionapp publish azp-msiotacc-func-dpsapi-pl --csx --publish-local-settings -i 
```
