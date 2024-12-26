//using WebTemplate.Algorithms;

//public static class XXTEACBC
//{
//    private const int BlockSize = 8; // Velicina bloka (64 bita)

//    // Metoda za enkripciju u CBC modu
//    public static byte[] EncryptWithCBC(byte[] data)
//    {
//        if (data == null)
//            throw new ArgumentNullException(nameof(data));

//        // Generišemo nasumičan IV
//        byte[] iv = GenerateIV();
//        List<byte> encrypted = new List<byte>(iv); // Početak rezultata sa IV-om

//        // Dodaj padding podacima
//        data = Pad(data, BlockSize);

//        byte[] prevBlock = iv;

//        // Procesiramo svaki blok
//        for (int i = 0; i < data.Length; i += BlockSize)
//        {
//            byte[] block = data.Skip(i).Take(BlockSize).ToArray();
//            byte[] xoredBlock = XOR(block, prevBlock); // XOR sa prethodnim blokom
//            byte[] encryptedBlock = XXTEA.Encrypt(xoredBlock); // Šifrujemo
//            encrypted.AddRange(encryptedBlock); // Dodajemo šifrovani blok
//            prevBlock = encryptedBlock; // Postavljamo za sledeću iteraciju
//        }

//        return encrypted.ToArray();
//    }


//    // Metoda za dekripciju u CBC modu
//    public static byte[] DecryptWithCBC(byte[] data)
//    {
//        if (data == null || data.Length < BlockSize)
//            throw new ArgumentException("Invalid encrypted data");

//        // Prvi blok je IV
//        byte[] iv = data.Take(BlockSize).ToArray();
//        byte[] encryptedData = data.Skip(BlockSize).ToArray();

//        if (encryptedData.Length % BlockSize != 0)
//            throw new ArgumentException("Invalid encrypted data length");

//        List<byte> decrypted = new List<byte>();
//        byte[] prevBlock = iv;

//        // Procesiramo svaki šifrovani blok
//        for (int i = 0; i < encryptedData.Length; i += BlockSize)
//        {
//            byte[] encryptedBlock = encryptedData.Skip(i).Take(BlockSize).ToArray();
//            byte[] decryptedBlock = XXTEA.Decrypt(encryptedBlock); // Dešifrujemo blok
//            byte[] originalBlock = XOR(decryptedBlock, prevBlock); // XOR sa prethodnim blokom
//            decrypted.AddRange(originalBlock); // Dodajemo dešifrovane podatke
//            prevBlock = encryptedBlock; // Postavljamo za sledeću iteraciju
//        }

//        return Unpad(decrypted.ToArray());
//    }


//    // Generisanje nasumičnog IV-a
//    private static byte[] GenerateIV()
//    {
//        byte[] iv = new byte[BlockSize];
//        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
//        {
//            rng.GetBytes(iv);
//        }
//        return iv;
//    }

//    private static byte[] XOR(byte[] a, byte[] b)
//    {
//        if (a.Length != b.Length)
//            throw new ArgumentException("Arrays must have the same length for XOR operation");

//        byte[] result = new byte[a.Length];
//        for (int i = 0; i < a.Length; i++)
//        {
//            result[i] = (byte)(a[i] ^ b[i]);
//        }
//        return result;
//    }

//    private static byte[] Pad(byte[] data, int blockSize)
//    {
//        int paddingSize = blockSize - (data.Length % blockSize);
//        byte[] paddedData = new byte[data.Length + paddingSize];
//        Array.Copy(data, 0, paddedData, 0, data.Length);

//        for (int i = data.Length; i < paddedData.Length; i++)
//        {
//            paddedData[i] = (byte)paddingSize;
//        }
//        return paddedData;
//    }

//    private static byte[] Unpad(byte[] data)
//    {
//        int paddingSize = data[data.Length - 1];
//        return data.Take(data.Length - paddingSize).ToArray();
//    }
//}
