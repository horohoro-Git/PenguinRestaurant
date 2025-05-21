using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class EncryptData 
{
    public static string ComputeHash(string content)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(content));
            return Convert.ToBase64String(hashBytes);


        }
    }

    public static string AddHashToJson(string filePath)
    {
        string jsonContent = File.ReadAllText(filePath);

        var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonContent);

        jsonObject.Remove("hash");

        string hash = ComputeHash(jsonObject.ToString(Formatting.None));


        jsonObject["hash"] = hash;

        File.WriteAllText(filePath, jsonObject.ToString(Formatting.Indented));

        return hash;

    }


    public static bool VerifyJsonHash(string fileData)
    {
        //   string jsonContent = File.ReadAllText(filePath);
        try
        {
            var jsonObject = JsonConvert.DeserializeObject<JObject>(fileData);

            string savedHash = jsonObject["hash"]?.ToString() ?? string.Empty;

            jsonObject.Remove("hash");

            string computedHash = ComputeHash(jsonObject.ToString(Formatting.None));

            return savedHash == computedHash;
        }
        catch
        {
            return false;
        }
    }

    // ������ ����
    public static void SaveBinaryFile(byte[] fileData, string key, string filePath)
    {
        using(SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(fileData);    //�ؽ� ����

            using(MemoryStream memoryStream = new MemoryStream())
            {
                using(BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    //memoryStream�� �����Ϳ� �ؽð� ����
                    writer.Write(fileData, 0, fileData.Length);
                    writer.Write(hash, 0, hash.Length);
                }
                byte[] combinedData = memoryStream.ToArray();
                byte[] enctyptedData = Encrypt(combinedData, key);  //�����Ϳ� �ؽ� ���� �Բ� ��ȣȭ

                using(FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileStream.Write(enctyptedData,0, enctyptedData.Length); //��ȣȭ �� ���� ���Ϸ� ����
                }
            }
        }
    }

    //������ ����
    public static void SaveBinaryData(byte[] fileData, string key, string filePath)
    {
        if(VerifyingHashes(filePath, key).Length > 1)
        {
            SaveBinaryFile(fileData, key, filePath);
        }
    }

    //�ؽ� ���� ����
    public static byte[] VerifyingHashes(string filePath, string key)
    {
        byte[] encryptedData;
        byte[] storedHash;

        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                //��ȣȭ�� ���� ����
                encryptedData = new byte[fs.Length];
                fs.Read(encryptedData, 0, encryptedData.Length);
            }

            if (encryptedData.Length >= 32)
            {
                byte[] decryptedData = Decrypt(encryptedData, key); //��ȣȭ ����

                storedHash = new byte[32];                           // ��ȣȭ�� �ؽ� ���� ��� �迭
                Array.Copy(decryptedData, decryptedData.Length - 32, storedHash, 0, 32);

                byte[] dataWithoutHash = new byte[decryptedData.Length - 32]; // ��ȣȭ�� �����Ͱ� ��� �迭
                Array.Copy(decryptedData, 0, dataWithoutHash, 0, decryptedData.Length - 32);

              
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] calculatedHash = sha256.ComputeHash(dataWithoutHash);

                    if (CompareHashes(calculatedHash, storedHash))   // ������ ���Ἲ ����
                    {
                        return dataWithoutHash;
                    }
                }
            }
        }
        catch
        {
            return new byte[0];
        }

        return new byte[0];
    }

    //���� ������ �ε�
    public static PlayerData LoadBinaryData(string filePath, string encryptionKey)
    {
        byte[] getData = VerifyingHashes(filePath, encryptionKey);
        if (getData.Length > 1)
        {
            using (BinaryReader reader = new BinaryReader(new MemoryStream(getData)))
            {
                PlayerData playerData = new PlayerData(reader.ReadInt32(), reader.ReadSingle(), reader.ReadInt32(),
                    reader.ReadInt32(), reader.ReadString(), reader.ReadInt32());

                return playerData;
            }
        }
        //�ε� ���� �ʱⰪ ��ȯ
        PlayerData defaultData = new PlayerData(1, 100, 0, 0, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 0);
        return defaultData;
    }
    private static bool CompareHashes(byte[] calculatedHash, byte[] storedHash)
    {
        for (int i = 0; i < calculatedHash.Length; i++)
        {
            if (calculatedHash[i] != storedHash[i])
                return false;
        }
        return true;
    }

    public static string Encrypt(string plainText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // 32����Ʈ Ű ���
            aes.IV = new byte[16]; // �ʱ�ȭ ����(IV)

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
                cs.Write(plainBytes, 0, plainBytes.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
        }
    }


    //��ȣȭ
    public static byte[] Encrypt(byte[] plainBytes, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // 32����Ʈ Ű ���
            aes.IV = new byte[16]; // �ʱ�ȭ ����

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(plainBytes, 0, plainBytes.Length); // ��ȣȭ ����
                cs.FlushFinalBlock();

             
                byte[] result = new byte[aes.IV.Length + ms.Length];
                aes.IV.CopyTo(result, 0);
                ms.ToArray().CopyTo(result, aes.IV.Length); 

                return ms.ToArray();
            }
        }
    }

    //��ȣȭ
    public static byte[] Decrypt(byte[] encryptedBytes, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // 32����Ʈ Ű ���
            aes.IV = new byte[16]; // �ʱ�ȭ ���� ũ�� ����
           
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(encryptedBytes, 0, encryptedBytes.Length); // ��ȣȭ ����
                cs.FlushFinalBlock();
               
                return ms.ToArray();
            }
        }
    }

    public static string Decrypt(string cipherText, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32));
            aes.IV = new byte[16];

            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream(Convert.FromBase64String(cipherText)))
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
            using (var reader = new StreamReader(cs))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
