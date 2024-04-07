# Configurates a url for the app server

1. app.Run("http://localhost:5000");
2. app.Urls.Add("http://localhost:5000");
3. dotnet run --urls=http://localhost:5000
4. var port = Envrionment.GetEnvironmentVariable("PORT") ?? "5000";
5. ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001
6. app.Urls.Add($"http://localhost:{port}");

# Self-signed certificate

1. Generate a private key:

```bash
openssl genrsa -out mycert.key 2048
```

note: The private key is used to decrypt data encripted by a public key. (only the private key holder can read encripted messages from a public key).
note: It's a  asymmetric cryptography, because is using a private key.