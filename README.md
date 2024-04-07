# Configurates PORT for the app server

Add a `PORT` number through the app `RUN` method:

```C#
app.Run("http://localhost:5000");
```

Add `PORT` number through `Urls` collection from app:

```C#
app.Urls.Add("http://localhost:5000");
```

Add `PORT` number by dotnet run option:

```bash
dotnet run --urls=http://localhost:5000
```

Add `PORT` number through Environment variable:

```C#
var port = Envrionment.GetEnvironmentVariable("PORT") ?? "5000";
```

Add `PORT` number through ASPNETCORE_URLS environment variable:

```bash
ASPNETCORE_URLS=http://localhost:5000;https://localhost:5001
```

# Certificate

## Using `dev-certs` tool

```bash
dotnet dev-certs https --clean
dotnet dev-certs https --trust
dotnet dev-certs https --check
```

Note: if chrome block this certificate: chrome://flags/#allow-insecure-localhost

**Certificate Generation**: A development SSL certificate is generated to be used by your ASP.NET Core applications. This is designed to facilitate HTTPS during development.

**Certificate Trust**: The --trust option adds the certificate to your *local machine's certificate store* and marks it as a trusted certificate. This means browsers and clients will trust the certificate, preventing them from showing warnings about untrusted certificates when you access your development sites.

**Certificate Storage**: The generated certificate is stored in a platform-specific location:

**Windows**: The certificate is placed in the Current **User's Personal certificate store (certlm.msc)** and the **Trusted Root Certification Authorities store** to be trusted by browsers and clients.
**macOS**: The certificate is added to the System Keychain and marked as trusted, which means it applies system-wide.
**Linux**: The handling of certificates can vary, but typically, the certificate is placed in a location that .NET Core knows to check, and you may need to manually trust the certificate depending on your distribution and environment setup.

**Usage by ASP.NET Core Applications**: ASP.NET Core applications are configured (via the development environment setup) to automatically use this development certificate for HTTPS. This is achieved through convention and the configuration system of ASP.NET Core, which checks for the existence of the development certificate in the appropriate store during application startup. ==You don't need to manually specify the certificate in your application settings for development environments==.

### How It Works Without Explicit Configuration in the Project

ASP.NET Core has built-in support for detecting and using the development certificate automatically when the application runs in the **development environment** (ASPNETCORE_ENVIRONMENT is set to Development). This seamless integration means you do not have to manually configure the certificate in your project settings for development purposes.

When you start an ASP.NET Core application in the development environment, it looks for the development certificate in the **certificate store or keychain**. Once found, it uses this certificate for HTTPS, enabling secure communication without additional configuration.

If you need to work with the certificate directly (for example, to use it with a different server or to distribute it to a team member), you might need to export it manually from the certificate store or System Keychain, depending on your operating system. This can usually be done through your **system's certificate management tools**.

### Access User's Personal Certificate

To access the User's Personal certificate store on Windows, you can use the ==Microsoft Management Console (MMC)== with the ==Certificate Snap-in==. Here's how to do it step-by-step:

**Open Microsoft Management Console (MMC):**

Press `Windows key + R` to open the ==Run dialog==.
Type `mmc` and press `Enter` or click OK. This opens the MMC console. If prompted by the User Account Control (UAC), click Yes to allow the MMC to run with administrative permissions.

**Add the Certificates Snap-in**:

* In the MMC console, click on `File > Add/Remove Snap-in...` or press `Ctrl + M` to open the Add or Remove Snap-ins dialog.

* From the list of available snap-ins, select `Certificates` and click the `Add > button`.
* A new window pops up asking you which certificates you want to manage. Select `My user account` to access the User's Personal certificate store. Alternatively, if you're looking to manage certificates for the local computer, you can select Computer account. After making your selection, click `Finish`.

*Click `OK` on the Add or Remove Snap-ins dialog to close it and return to the main MMC window.

**Navigate to the Personal Certificate Store:**

In the MMC, you should now see the Certificates snap-in for the account you selected. Expand the Certificates - Current User tree by clicking on the arrow next to it.
Navigate to Personal > Certificates. Here you will find the list of certificates stored in the User's Personal certificate store. This includes any development certificates generated by the dotnet dev-certs command, which are typically named something like "ASP.NET Core HTTPS development certificate".

**Manage Certificates**:

Within this view, you can manage your certificates. For example, you can double-click on a certificate to view its details, export it, or delete it if necessary. Right-click on a certificate to see the available options.

When done, you can simply close the MMC console. If you made changes and are prompted to save the console settings, you can choose to save or not based on your preference.

## Using a self-signed certificate generated locally

To create a self-signed SSL certificate, you can use OpenSSL, a widely-used tool for SSL/TLS operations. If you don't have OpenSSL installed.

Run the following `OpenSSL` command to create a new self-signed certificate and ==private key==:

```bash
openssl req -x509 -newkey rsa:4096 -sha256 -days 365 -nodes -out mycert.pem -keyout mykey.key -subj "/CN=localhost" -addext "subjectAltName=DNS:localhost"
```

* `req`: This option indicates the part of OpenSSL that deals with certificate request (CSR) files. It can be used to create and process CSR files needed for certificates.

* `x509`: This specifies that you want to use X.509 Certificate Signing Request (CSR) management. The x509 option is used here to generate a self-signed certificate.

* `-newkey rsa:4096`: This option tells OpenSSL to create a new certificate and a new private key at the same time. rsa:4096 indicates the key should be an RSA key that is 4096 bits long.

* `-sha256`: This specifies the hash algorithm to use. In this case, SHA-256 is used for signing the certificate, offering a good level of security.

* `-days 365`: This sets the validity period of the certificate. The certificate will be valid for 365 days from its creation.

* `-nodes`: This stands for "no DES". It means that the private key will not be encrypted with a passphrase or password. This is often more convenient for automated processes, but less secure.

* `-out mycert.pem`: This specifies the filename to write the newly created certificate to. In this case, the certificate is written to mycert.pem.

* `-keyout mykey.key`: This specifies the filename to write the newly created private key to. In this case, the key is written to mykey.key.

* `-subj "/CN=localhost"`: This sets the subject of the certificate. Here, it specifies the Common Name (CN) as localhost, which is typically used for development environments.

* `-addext "subjectAltName=DNS:localhost"`: This adds an extension to the certificate specifying the Subject Alternative Name (SAN). It allows the certificate to be valid not just for the CN, but also for additional names. Here, it's used to ensure the certificate is also valid for the DNS name localhost.

Convert your `PEM` (it has the public key inside of it also) certificate and `key` to a `PFX format`, as **ASP.NET Core typically uses PFX files for certificates**.

```bash
openssl pkcs12 -export -out mycert.pfx -inkey mykey.key -in mycert.pem
```

* `pkcs12`: This specifies that you want to use OpenSSL's PKCS#12 file management capabilities. PKCS#12 is a binary format for storing a certificate chain and private key in a single encrypted file.

* `-export`: This option indicates that you want to export the private key and certificates to a PKCS#12 file. This is typically used to bundle a private key with its corresponding certificate and any intermediate certificates into a single file.

* `-out mycert.pfx`: This specifies the filename of the PKCS#12 file that will be created by the command. In this case, mycert.pfx is the name of the file to be created.

* `-inkey mykey.key`: This option specifies the file containing the private key to be included in the PKCS#12 file. mykey.key is the private key file generated in the previous step.

* `-in mycert.pem`: This option specifies the certificate file to be included in the PKCS#12 file. mycert.pem is the certificate file generated in the previous step.

## Notes

### TLS workflow overview

**Certificate's Role**: The primary role of the certificate in the **context of a TLS (Transport Layer Security) session** is ==indeed to provide the server's public key to the client and to verify the server's identity==. *The certificate, which includes the public key, is issued by a Certificate Authority (CA) or can be self-signed (common in development scenarios)*.

**Public and Private Key Usage**: ==The public and private keys are used during the initial phase of the TLS handshake, not for encrypting the entire session's data directly==. Here’s how it works:

1. **Key Exchange**: At the start of a TLS session, the client and the server agree on a protocol version, select cryptographic algorithms, and authenticate the server based on its certificate. As part of this process, ==the client either:
Uses the server's public key (from the certificate) to encrypt a pre-master secret and sends it to the server, which then decrypts it using its private key==. This is common in RSA key exchange mechanisms.
2. ==The pre-master secret or shared secret is then used by both the client and the server to generate a symmetric session key==. This `session key` is what actually encrypts and decrypts the data exchanged during the session.
3. Symmetric Encryption for the Session: Once the symmetric session key is established, all data transmitted between the client and the server is encrypted and decrypted using this key. 4. Symmetric encryption is used because it's much faster than asymmetric encryption (which involves the public and private keys) and allows for efficient and secure data exchange after the initial handshake.

**Role of Private Key in Decryption**: Initially, it might seem like the server's private key is directly used to decrypt data sent by the client. However, the private key is primarily used to decrypt the pre-master secret (in RSA key exchange) or to authenticate in a key exchange mechanism, leading to the generation of a symmetric session key. It's this symmetric key that is used for encrypting and decrypting the session data, not the private key directly.

### PFX

A PFX file, also known as a PKCS#12 file, is a ==type of digital file that stores a certificate and its associated private key, often protected by a password==. The PFX (Personal Information Exchange) format is widely used in various applications to securely transport user certificates and private keys across programs and systems. It's particularly ==common in Windows environments== for importing and exporting certificates and keys.

