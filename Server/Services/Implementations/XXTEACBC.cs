using WebTemplate.Services.Interfaces;

namespace WebTemplate.Services.Implementations;


public class XXTEACBC : IAlgorithm
{
    private const int BlockSize = 8; // Velicina bloka (64 bita)
    private readonly XXTEA _xxtea;

    public XXTEACBC(XXTEA xxtea)
    {
        _xxtea = xxtea;
    }

    // Metoda za enkripciju u CBC modu
    public Byte[] Encrypt(Byte[] data)
    {
        if (data == null)
            throw new ArgumentNullException(nameof(data));

        // Generišemo nasumičan IV
        Byte[] iv = GenerateIV();
        List<Byte> encrypted = new List<Byte>(iv); // Početak rezultata sa IV-om

        // Dodaj padding podacima
        data = Pad(data, BlockSize);

        Byte[] prevBlock = iv;

        // Procesiramo svaki blok
        for (int i = 0; i < data.Length; i += BlockSize)
        {
            Byte[] block = data.Skip(i).Take(BlockSize).ToArray();
            Byte[] xoredBlock = XOR(block, prevBlock); // XOR sa prethodnim blokom
            Byte[] encryptedBlock = _xxtea.Encrypt(xoredBlock); // Šifrujemo
            encrypted.AddRange(encryptedBlock); // Dodajemo šifrovani blok
            prevBlock = encryptedBlock; // Postavljamo za sledeću iteraciju
        }

        return encrypted.ToArray();
    }


    // Metoda za dekripciju u CBC modu
    public Byte[] Decrypt(Byte[] data)
    {
        if (data == null || data.Length < BlockSize)
            throw new ArgumentException("Invalid encrypted data");

        // Prvi blok je IV
        Byte[] iv = data.Take(BlockSize).ToArray();
        Byte[] encryptedData = data.Skip(BlockSize).ToArray();

        if (encryptedData.Length % BlockSize != 0)
            throw new ArgumentException("Invalid encrypted data length");

        List<Byte> decrypted = new List<Byte>();
        Byte[] prevBlock = iv;

        // Procesiramo svaki šifrovani blok
        for (int i = 0; i < encryptedData.Length; i += BlockSize)
        {
            Byte[] encryptedBlock = encryptedData.Skip(i).Take(BlockSize).ToArray();
            Byte[] decryptedBlock = _xxtea.Decrypt(encryptedBlock); // Dešifrujemo blok
            Byte[] originalBlock = XOR(decryptedBlock, prevBlock); // XOR sa prethodnim blokom
            decrypted.AddRange(originalBlock); // Dodajemo dešifrovane podatke
            prevBlock = encryptedBlock; // Postavljamo za sledeću iteraciju
        }

        return Unpad(decrypted.ToArray());
    }


    // Generisanje nasumičnog IV-a
    private Byte[] GenerateIV()
    {
        Byte[] iv = new Byte[BlockSize];
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            rng.GetBytes(iv);
        }
        return iv;
    }

    private Byte[] XOR(Byte[] a, Byte[] b)
    {
        if (a.Length != b.Length)
            throw new ArgumentException("Arrays must have the same length for XOR operation");

        Byte[] result = new Byte[a.Length];
        for (int i = 0; i < a.Length; i++)
        {
            result[i] = (Byte)(a[i] ^ b[i]);
        }
        return result;
    }

    private Byte[] Pad(Byte[] data, int blockSize)
    {
        int paddingSize = blockSize - (data.Length % blockSize);
        Byte[] paddedData = new Byte[data.Length + paddingSize];
        Array.Copy(data, 0, paddedData, 0, data.Length);

        for (int i = data.Length; i < paddedData.Length; i++)
        {
            paddedData[i] = (Byte)paddingSize;
        }
        return paddedData;
    }

    private Byte[] Unpad(Byte[] data)
    {
        int paddingSize = data[data.Length - 1];
        return data.Take(data.Length - paddingSize).ToArray();
    }
}
