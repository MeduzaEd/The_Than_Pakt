using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using Random = System.Random;

public class EncryptionManager : MonoBehaviour
{
    private static byte[] key=new byte[32];
    private static byte[] iv = new byte[16];
    public static string GenerateKey()
    {
        using var cryptoProvider = new RNGCryptoServiceProvider();
        byte[] data = new byte[32]; // 32 байта для ключа
        cryptoProvider.GetBytes(data);
        return Convert.ToBase64String(data);
    }

    public static string GenerateIV()
    {
        using var cryptoProvider = new RNGCryptoServiceProvider();
        byte[] data = new byte[16]; // 16 байт для IV
        cryptoProvider.GetBytes(data);
        return Convert.ToBase64String(data);
    }
    private void OnApplicationQuit()
    {
        // Сохраняем ключи при выходе из приложения
        SaveEncryptoAndDescrypto();
        Debug.Log("Saved E&D");
    }
    private void SaveEncryptoAndDescrypto()
    {
        PlayerPrefs.SetString("the_encryption_key", Convert.ToBase64String(key));
        PlayerPrefs.SetString("the_encryption_iv", Convert.ToBase64String(iv));
        PlayerPrefs.Save();
    }
    private void Start()
    {
        // Загружаем ключи при запуске приложения
        key = Convert.FromBase64String(PlayerPrefs.GetString("the_encryption_key", GenerateKey()));
        iv = Convert.FromBase64String(PlayerPrefs.GetString("the_encryption_iv", GenerateIV()));
        if((key.Length != 32)||(iv.Length != 16))
        {
            PlayerPrefs.DeleteAll();
            key = Convert.FromBase64String(GenerateKey());
            iv = Convert.FromBase64String(GenerateIV());
            SaveEncryptoAndDescrypto();
            Debug.Log("IncorrectLenght");
        }
        Debug.Log(key.Length);
        Debug.Log(iv.Length);
    }
    public static string Encrypt(string plainText)
    {
        using Aes aesAlg = Aes.Create();
      
        aesAlg.Key = key;
        aesAlg.IV = iv;

        ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msEncrypt = new ();
        {
            using CryptoStream csEncrypt = new(msEncrypt, encryptor, CryptoStreamMode.Write);
            {
                using StreamWriter swEncrypt = new(csEncrypt);
                {
                    swEncrypt.Write(plainText);
                }
            }
        }
     
        return Convert.ToBase64String(msEncrypt.ToArray());
        
    }

    public static string Decrypt(string cipherText)
    {
        using Aes aesAlg = Aes.Create();
        
        aesAlg.Key = key;
        aesAlg.IV = iv;

        ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

        using MemoryStream msDecrypt = new (Convert.FromBase64String(cipherText));
            
        using CryptoStream csDecrypt = new (msDecrypt, decryptor, CryptoStreamMode.Read);
                
        using StreamReader srDecrypt = new (csDecrypt);
                    
        return srDecrypt.ReadToEnd();

    }
}
