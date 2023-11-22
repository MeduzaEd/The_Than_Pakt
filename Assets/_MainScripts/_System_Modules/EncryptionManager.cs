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
        using (var cryptoProvider = new RNGCryptoServiceProvider())
        {
            byte[] data = new byte[32]; // 32 ����� ��� �����
            cryptoProvider.GetBytes(data);
            return Convert.ToBase64String(data);
        }
    }

    public static string GenerateIV()
    {
        using (var cryptoProvider = new RNGCryptoServiceProvider())
        {
            byte[] data = new byte[16]; // 16 ���� ��� IV
            cryptoProvider.GetBytes(data);
            return Convert.ToBase64String(data);
        }
    }
    private void OnApplicationQuit()
    {
        // ��������� ����� ��� ������ �� ����������
        PlayerPrefs.SetString("the_encryption_key2", Convert.ToBase64String(key));
        PlayerPrefs.SetString("the_encryption_iv2", Convert.ToBase64String(iv));
        PlayerPrefs.Save();
    }

    private void Start()
    {
        // ��������� ����� ��� ������� ����������
        key = Convert.FromBase64String(PlayerPrefs.GetString("the_encryption_key2", GenerateKey()));
        iv = Convert.FromBase64String(PlayerPrefs.GetString("the_encryption_iv2", GenerateIV()));
        Debug.Log(key);
        Debug.Log(iv);
    }
    public static string Encrypt(string plainText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msEncrypt = new MemoryStream())
            {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                }
                return Convert.ToBase64String(msEncrypt.ToArray());
            }
        }
    }

    public static string Decrypt(string cipherText)
    {
        using (Aes aesAlg = Aes.Create())
        {
            aesAlg.Key = key;
            aesAlg.IV = iv;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

            using (MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(cipherText)))
            {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        return srDecrypt.ReadToEnd();
                    }
                }
            }
        }
    }

}
