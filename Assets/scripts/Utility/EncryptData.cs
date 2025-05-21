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

    // 파일을 저장
    public static void SaveBinaryFile(byte[] fileData, string key, string filePath)
    {
        using(SHA256 sha = SHA256.Create())
        {
            byte[] hash = sha.ComputeHash(fileData);    //해시 생성

            using(MemoryStream memoryStream = new MemoryStream())
            {
                using(BinaryWriter writer = new BinaryWriter(memoryStream))
                {
                    //memoryStream에 데이터와 해시값 저장
                    writer.Write(fileData, 0, fileData.Length);
                    writer.Write(hash, 0, hash.Length);
                }
                byte[] combinedData = memoryStream.ToArray();
                byte[] enctyptedData = Encrypt(combinedData, key);  //데이터와 해시 값을 함께 암호화

                using(FileStream fileStream = new FileStream(filePath, FileMode.Create))
                {
                    fileStream.Write(enctyptedData,0, enctyptedData.Length); //암호화 된 값을 파일로 저장
                }
            }
        }
    }

    //데이터 저장
    public static void SaveBinaryData(byte[] fileData, string key, string filePath)
    {
        if(VerifyingHashes(filePath, key).Length > 1)
        {
            SaveBinaryFile(fileData, key, filePath);
        }
    }

    //해시 값을 검증
    public static byte[] VerifyingHashes(string filePath, string key)
    {
        byte[] encryptedData;
        byte[] storedHash;

        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                //암호화된 값을 읽음
                encryptedData = new byte[fs.Length];
                fs.Read(encryptedData, 0, encryptedData.Length);
            }

            if (encryptedData.Length >= 32)
            {
                byte[] decryptedData = Decrypt(encryptedData, key); //복호화 진행

                storedHash = new byte[32];                           // 복호화된 해시 값이 담길 배열
                Array.Copy(decryptedData, decryptedData.Length - 32, storedHash, 0, 32);

                byte[] dataWithoutHash = new byte[decryptedData.Length - 32]; // 복호화된 데이터가 담길 배열
                Array.Copy(decryptedData, 0, dataWithoutHash, 0, decryptedData.Length - 32);

              
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] calculatedHash = sha256.ComputeHash(dataWithoutHash);

                    if (CompareHashes(calculatedHash, storedHash))   // 데이터 무결성 검증
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

    //게임 데이터 로드
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
        //로드 실패 초기값 반환
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
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // 32바이트 키 사용
            aes.IV = new byte[16]; // 초기화 벡터(IV)

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


    //암호화
    public static byte[] Encrypt(byte[] plainBytes, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // 32바이트 키 사용
            aes.IV = new byte[16]; // 초기화 벡터

            using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                cs.Write(plainBytes, 0, plainBytes.Length); // 암호화 진행
                cs.FlushFinalBlock();

             
                byte[] result = new byte[aes.IV.Length + ms.Length];
                aes.IV.CopyTo(result, 0);
                ms.ToArray().CopyTo(result, aes.IV.Length); 

                return ms.ToArray();
            }
        }
    }

    //복호화
    public static byte[] Decrypt(byte[] encryptedBytes, string key)
    {
        using (Aes aes = Aes.Create())
        {
            aes.Key = Encoding.UTF8.GetBytes(key.PadRight(32)); // 32바이트 키 사용
            aes.IV = new byte[16]; // 초기화 벡터 크기 지정
           
            using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
            using (var ms = new MemoryStream())
            using (var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Write))
            {
                cs.Write(encryptedBytes, 0, encryptedBytes.Length); // 복호화 진행
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
