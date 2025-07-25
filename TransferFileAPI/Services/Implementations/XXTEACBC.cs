using WebTemplate.Services.Interfaces;

namespace WebTemplate.Services.Implementations;


public class XXTEACBC : IAlgorithm
{
    private const int BlockSize = 8;
    private readonly XXTEA _xxtea;

    public XXTEACBC(XXTEA xxtea)
    {
        _xxtea = xxtea;
    }

    public byte[] Encrypt(byte[] data)
    {
        if (data is null) throw new ArgumentNullException(nameof(data));

        byte[] iv = GenerateIV();
        var output = new List<byte>(iv);

        // pad to a multiple of 8
        var padded = Pad(data, BlockSize);
        byte[] prev = iv;

        for (int i = 0; i < padded.Length; i += BlockSize)
        {
            byte[] chunk = padded.AsSpan(i, BlockSize).ToArray();
            byte[] xored = XOR(chunk, prev);
            byte[] cipher = _xxtea.EncryptBlock(xored);   // <-- block call
            output.AddRange(cipher);
            prev = cipher;
        }

        return output.ToArray();
    }

    public byte[] Decrypt(byte[] data)
    {
        if (data == null || data.Length < BlockSize)
            throw new ArgumentException("Invalid data", nameof(data));

        byte[] iv = data[..BlockSize];
        byte[] cipher = data[BlockSize..];

        if (cipher.Length % BlockSize != 0)
            throw new ArgumentException("Invalid length", nameof(data));

        var plain = new List<byte>();
        byte[] prev = iv;

        for (int i = 0; i < cipher.Length; i += BlockSize)
        {
            byte[] block = cipher.AsSpan(i, BlockSize).ToArray();
            byte[] dec = _xxtea.DecryptBlock(block);    // <-- block call
            byte[] orig = XOR(dec, prev);
            plain.AddRange(orig);
            prev = block;
        }

        return Unpad(plain.ToArray());
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