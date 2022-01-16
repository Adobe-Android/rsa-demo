using System.Security.Cryptography;
using System.Text;

// Starter code copied from:
// https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider?view=net-6.0

class RSAConsole
{
    static void Main()
    {
        try
        {
            // Create a UnicodeEncoder to convert between byte array and string.
            UnicodeEncoding ByteConverter = new UnicodeEncoding();

            // Create byte arrays to hold original, encrypted, and decrypted data.
            byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");
            byte[] encryptedData;
            byte[] decryptedData;

            // Create a new instance of RSACryptoServiceProvider to generate
            // public and private key data.
            // As documented, RSACryptoServiceProvider initializes a new instance
            // of the RSACryptoServiceProvider class with a random key pair
            // unless cryptographic service provider parameters are specified in the constructor.
            // https://docs.microsoft.com/en-us/dotnet/api/system.security.cryptography.rsacryptoserviceprovider.-ctor?view=net-6.0#system-security-cryptography-rsacryptoserviceprovider-ctor(system-int32-system-security-cryptography-cspparameters)
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(2048))
            {

                // Pass the data to ENCRYPT, the public key information 
                // (using RSACryptoServiceProvider.ExportParameters(false),
                // and a boolean flag specifying no OAEP padding.
                encryptedData = RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);
                var payLoad = Convert.ToBase64String(encryptedData);

                // May want to store this data for use later.
                Console.WriteLine(RSA.ToXmlString(true));

                // Pass the data to DECRYPT, the private key information 
                // (using RSACryptoServiceProvider.ExportParameters(true),
                // and a boolean flag specifying no OAEP padding.
                decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);

                // Display the decrypted plaintext to the console. 
                Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedData));

                // You can also forgo the above steps
                // and take better advantage of built-in methods and existing keys.
                // It is totally secure to embed the components of your public key in your application.
                // These components are the Modulus and Exponent.
                var xmlString = "<RSAKeyValue><Modulus>1mArEnZxXBM7gcZF+fsVeMrtTzcMyeKQkqlfmH8/LLWQX7Pniwv3guZGY1z9c2HbRAhTKirjVLn3P7jiII/DvDp5goFEKhR2nMrU/sn71/suTvTi8WnEbUDy7yvNnymfkzB1fWm33yvitOQyTLn50uR4pITlMBGmXlfezmYXn3eG4Fd7xhJYr3N3D0plxHQhZTvY1Un3Tah1HRxldc93IVbDBWhNbnWnYwOnw9N5xPY7l6zmRMjRwyz3t5vkQs27YG5UpPXu96hRBwpLdf67UlLzgDlfoUFII2y/r7LD7ikDHm5kj8d1yAHnIxY9vk94IiD8mbU8m9gHYZTFNpm67Q==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

                RSA.FromXmlString(xmlString);
                var ed = RSA.Encrypt(dataToEncrypt, false);
                var b64String = Convert.ToBase64String(ed);

                // The P, Q, DP, DQ, InverseQ, and D portions of the RSA key should remain private.
                // The below key is embedded here only for teaching purposes.
                // Do NOT embed any part of your private key within your application!
                xmlString = "<RSAKeyValue><Modulus>1mArEnZxXBM7gcZF+fsVeMrtTzcMyeKQkqlfmH8/LLWQX7Pniwv3guZGY1z9c2HbRAhTKirjVLn3P7jiII/DvDp5goFEKhR2nMrU/sn71/suTvTi8WnEbUDy7yvNnymfkzB1fWm33yvitOQyTLn50uR4pITlMBGmXlfezmYXn3eG4Fd7xhJYr3N3D0plxHQhZTvY1Un3Tah1HRxldc93IVbDBWhNbnWnYwOnw9N5xPY7l6zmRMjRwyz3t5vkQs27YG5UpPXu96hRBwpLdf67UlLzgDlfoUFII2y/r7LD7ikDHm5kj8d1yAHnIxY9vk94IiD8mbU8m9gHYZTFNpm67Q==</Modulus><Exponent>AQAB</Exponent><P>1tFqn2Pja0L5w9kUK808WJObjUENAuRDPwSNNb6jbwbOC7iyweFUoJhzQc0UXtGmuKDBv2fhnkhHFOjNchjiRS4ssbQzOT1dwuLvz+wj0po/yrGRxYyya+y6wL3B4rYABDCou8pRiZodl/hVSvpFtyYOwW/5KzEYdDhMU39GwMM=</P><Q>/3kKlkv0+Jn8n8O5pExJ4CksvwvqZlKCkJoxyXJISV020DnWEDAP3AFvuXTArDV6d06HDsqe4sX7XYjlXDiNQ//dJOOxQ2H9r+OvJcMilAhy5hsG/5/zO8o5RMYtUCPt16jzWjARmb823imU88Xh243LCgJlFY0STQa9XOt62o8=</Q><DP>JYO64Az8qT2wCpC9YgzcbgeREbD7iba9O6Ma2fjp57jDgO6HYS5trgIRlrsuxIROXk3MqBWHJDIxH6isQDGySiyPJ5V1oJhj6GXjRWdStjOz0j0CXQJ+IjTcRE6fLm1kQVcSEIF7jdE5O9Hla0uboyllSEi6td1EDP/L7IT8oSs=</DP><DQ>5ZjP5gF/uQkTXDZYGguh/T6kA1LfJ3SR+QlLI2N9CconX/4Kn2PCbRQxZ7hBDMPHO4AsqyN4phAOd6J3l2kPtIQ4KFYl0ow3paL3nvV/lxD8ykaSBJyoyhxVWbTNto+DyuUBSBWZS9bopcMzJd66vEnzpxw94p91Shp7hw3uzIs=</DQ><InverseQ>FvlzqIRCfTgvHEIrmszYC8Hlc9403NRUkEKYp6yoasak4bseYoiDKbfGeb/04vyLMIV7ec1ptQiuNXkvNYwscXEE8n3fD5SV2mu47PEtqecew3HqSnB1izsSa0pLe74VTQKNZ73mson6QlLCH+AwALc8W9ZWcD0U68RJAZ/PWAc=</InverseQ><D>L4TLp2EHybAdrauEkV/dp5PbvIZoWEvXXhsbLJNwLnnkkcIwHfs+Fw7yJU5UhAH593+c56jwETTToJVb0nnrmkqf6pS6xlUY4bJ4Oi4NnBd6cVgJAhg+yTTvZKKiBpjUXW0NbgpEDr0MmKpImAdVYDDyqQ0oPFp9NUZnBftNWzRcOy0X+g9IvGurJuKEzonD26sZSJJ8GoiMxJDxfW26Nv7zqJRPJRcME98hPgNnCr8xwVbGmy7abO/bMN701TbbC56I7l6iqP4I+eCQ9+aeJvvwLR75fcWNbt3n7pGAFCr0jmXLs4O0XgImHWtiL3YqfiJbu+ulEsM426lWp8S1ZQ==</D></RSAKeyValue>";
                
                RSA.FromXmlString(xmlString);
                var encryptedBytes = Convert.FromBase64String(b64String);
                var decryptedBytes = RSA.Decrypt(encryptedBytes, false);
                Console.WriteLine("Decrypted plaintext: {0}", ByteConverter.GetString(decryptedBytes));
            }
        }
        catch (ArgumentNullException)
        {
            // Catch this exception in case the encryption did
            // not succeed.
            Console.WriteLine("Encryption failed.");
        }
    }

    public static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
    {
        try
        {
            byte[] encryptedData;
            // Create a new instance of RSACryptoServiceProvider.
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                // Import the RSA Key information. This only needs
                // to include the public key information.
                RSA.ImportParameters(RSAKeyInfo);

                // Encrypt the passed byte array and specify OAEP padding.  
                // OAEP padding is only available on Microsoft Windows XP or
                // later.  
                encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
            }
            return encryptedData;
        }
        // Catch and display a CryptographicException  
        // to the console.
        catch (CryptographicException e)
        {
            Console.WriteLine(e.Message);

            return null;
        }
    }

    public static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
    {
        try
        {
            byte[] decryptedData;
            // Create a new instance of RSACryptoServiceProvider.
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
            {
                // Import the RSA Key information. This needs
                // to include the private key information.
                RSA.ImportParameters(RSAKeyInfo);

                // Decrypt the passed byte array and specify OAEP padding.  
                // OAEP padding is only available on Microsoft Windows XP or
                // later.  
                decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
            }
            return decryptedData;
        }
        // Catch and display a CryptographicException  
        // to the console.
        catch (CryptographicException e)
        {
            Console.WriteLine(e.ToString());

            return null;
        }
    }
}