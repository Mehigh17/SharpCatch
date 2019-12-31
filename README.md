<div align="center">
    <h1>SharpCatch</h1>
    <div>
        <a href="#installation">Installation</a> •
        <a href="#usage">Usage</a>
    </div>
</div>

![License](https://img.shields.io/github/license/Mehigh17/SharpCatch)
![Issues](https://img.shields.io/github/issues/Mehigh17/SharpCatch)
![Nuget Version](https://img.shields.io/nuget/v/SharpCatch.Asp)

SharpCatch is a small library allowing a simple integration for reCAPTCHA token validation.

# Installation

Download the package from nuget using the dotnet cli:

```
dotnet add package SharpCatch.Asp
```

or using the Package-Manager:

```
Install-Package SharpCatch.Asp
```

# Usage

## Registering the service

This is an example registering the service using the ASP.NET Core IoC container.
```cs
services.AddTransient<IRecaptchaService>(new RecaptchaService(Configuration["Recaptcha:SecretKey"]))
```

The library offers an action filter attribute that can be quickly assigned to an action without having to do the verification by yourself and polluting the controller with redundant checks.

**Notice for v2 users**: The parameters "action" and "minimumScoreThreshold" aren't part of the second version of reCAPTCHA so do not use these otherwise your validation will fail in most cases!

### reCAPTCHA v2

Example:
```cs
[HttpPost]
[RecaptchaValidation(errorMessage: "This message will be shown when recaptcha fails.")]
public async Task<IActionResult> SomeAction(SomeParams params) { }
```

### reCAPTCHA v3

Example:
```cs
[HttpPost]
[RecaptchaValidation(
    errorMessage: "This message will be shown when recaptcha fails.",
    action: "login",
    minimumScoreThreshold: 0.8)]
public async Task<IActionResult> SomeAction(SomeParams params) { }
```