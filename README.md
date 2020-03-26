# Swiss Life - SLKV Partner API

This sample application demonstrates how you can use the SLKV Partner API.

## Requirements

* .[NET Core 3.1 SDK](https://www.microsoft.com/net/download/core)

## Important Snippets

### 1. Configuration

Navigate to [appsettings.json](appsettings.json) and configure your application:

```json
 "Auth0": {
    "Domain": "<the authentication endpoint - e.g. login.f2c-uat.swisslife.ch>",
    "ClientId": "<your client id, given by Swiss Life>",
    "ClientSecret": "<your client secret, given by Siss Life>"
  },
  "SlkvPartnerApi": {
    "Endpoint": "<the API endpoint - partner-api.uat-fimu.swisslife.ch>"
  }
```

### 2. Request required scopes

Navigate to [Startup.cs](Startup.cs) method ConfigureServices and enter required scopes:

```csharp
options.Scope.Add("slkv_partner_access");
// options.Scope.Add("slkv_process_contractconnect");
// options.Scope.Add("slkv_process_entry");
// options.Scope.Add("slkv_process_pensionplanchange");
// options.Scope.Add("slkv_process_salarychange");
// options.Scope.Add("slkv_process_contractaddresschange");
// options.Scope.Add("slkv_process_leaving");
// options.Scope.Add("slkv_process_personaddresschange");
// options.Scope.Add("slkv_process_salarybreak");
// options.Scope.Add("slkv_process_persondatachange");
// options.Scope.Add("slkv_process_iam");
```

### 3. Run the application

1. Run the application from the command line:

    ```bash
    dotnet run
    ```

2. Go to `http://localhost:5000` in your web browser to view the website.

3. You should get redirected to Swiss Life login page.

4. Log in, like you log in to MyLife

5. Get redirected back to this application.


# Log

| Date | Author | Desciption |
|:---:|:--- |:--- |
| 26.03.2020 | Andreas Regner | First version of client app sample |