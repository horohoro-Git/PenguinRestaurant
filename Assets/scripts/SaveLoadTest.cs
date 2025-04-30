using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class SaveLoadTest
{

    private static string path = Application.persistentDataPath;

    static int Data1 = 5529;
    static int Data2 = 653474585;
    static float Data3 = 5523629.1f;
    static bool Data4 = false;
    static string Data5 = "justTest";
    static string Data6 = "more";
    static int Data7 = 166;
    static float Data8 = 5529.56f;
    static double Data9 = 5529.5d;

    static string key = "AAA";

    public static void WriteData()
    {
        string pathdata = Path.Combine(path, "save/test.dat");
       //
        using(MemoryStream ms = new MemoryStream())
        {
            using(BinaryWriter writer = new BinaryWriter(ms))
            {
                writer.Write(Data1);
                writer.Write(Data2);
                writer.Write(Data3);
                writer.Write(Data4);
                writer.Write(Data5);
                writer.Write(Data6);
                writer.Write(Data7);
                writer.Write(Data8);
                writer.Write(Data9);

                Debug.Log("저장할 데이터를 메모리에 저장 Data1 = " + Data1);
                Debug.Log("암호화 전 바이트 배열\n" + Convert.ToBase64String(ms.ToArray()));
            }
            byte[] result = Encrypt(ms.ToArray(), key);
           
            Debug.Log("저장된 데이터를 암호화 진행");
            using(FileStream fs = new FileStream(pathdata, FileMode.OpenOrCreate))
            {
            
                Debug.Log("암호화된 데이터를 파일로 저장\n" + Convert.ToBase64String(result));
                fs.Write(result);

            }
        }
                Debug.Log("저장 완료");

    }
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
    public static void ReadData()
    {
        string pathdata = Path.Combine(path, "save/test.dat");
        byte[] encryptedData;
        // 파일이 존재하는지 확인
        if (File.Exists(pathdata))
        {
            try
            {
                using (FileStream fs = new FileStream(pathdata, FileMode.Open))
                {
                    //암호화된 값을 읽음
                    encryptedData = new byte[fs.Length];
                    fs.Read(encryptedData, 0, encryptedData.Length);
                    Debug.Log("암호화된 파일의 값을 읽기\n" + Convert.ToBase64String(encryptedData));


                }
                byte[] decryptedData = Decrypt(encryptedData, key);


               
                Debug.Log("암호화된 데이터를 복호화\n" + Convert.ToBase64String(decryptedData));

                // 파일을 읽기 위해 FileStream을 열고 BinaryReader를 사용
               
                using (BinaryReader reader = new BinaryReader(new MemoryStream(decryptedData)))
                {
                    // 파일에 기록된 데이터를 순차적으로 읽기
                    int data1 = reader.ReadInt32();
                    int data2 = reader.ReadInt32();
                    float data3 = reader.ReadSingle();
                    bool data4 = reader.ReadBoolean();
                    string data5 = reader.ReadString();
                    string data6 = reader.ReadString();
                    int data7 = reader.ReadInt32();
                    float data8 = reader.ReadSingle();
                    double data9 = reader.ReadDouble();

                    Debug.Log("데이터를 로드 Data1 = " + data1);
                    
                    // 읽은 데이터를 디버그 로그로 출력
                   // Debug.Log($"Data1: {data1}, Data9: {data9}");
                }
                Debug.Log("로드 완료");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error reading file: {ex.Message}");
            }
        }
        else
        {
            Debug.LogError("File not found: " + pathdata);
        }
    }
}
