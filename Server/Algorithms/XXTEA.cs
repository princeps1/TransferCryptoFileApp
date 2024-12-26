namespace WebTemplate.Algorithms;

using System;
using System.Text;
using System.Text.Unicode;

public class XXTEA
{
    private static readonly uint Delta = 0x9E3779B9;
    private static readonly string key = "princeps";

    public static byte[] Encrypt(byte[] data)
    {
        if (data == null || data.Length == 0) return data;

        uint[] v = ToUInt32Array(data, true);
        uint[] k = ToUInt32Array(Encoding.ASCII.GetBytes(key), false);

        if (k.Length < 4)
        {
            Array.Resize(ref k, 4); // Dodaje 0 sve dok niz nema 4 elementa
        }

        uint n = (uint)v.Length - 1;
        if (n < 1) return data;

        uint sum = 0;
        uint rounds = 6 + 52 / (n + 1);

        for (uint i = 0; i < rounds; i++)
        {
            sum += Delta;
            uint e = (sum >> 2) & 3;

            for (uint p = 0; p < n; p++)
            {
                uint y = v[p + 1];
                uint z = v[p];
                v[p] += ((z >> 5) ^ (y << 2)) + ((y ^ sum) + (k[(p & 3) ^ e] ^ z));
            }

            v[n] += ((v[0] >> 5) ^ (v[n - 1] << 2)) + ((v[n - 1] ^ sum) + (k[(n & 3) ^ e] ^ v[0]));
        }

        byte[] res = ToByteArray(v, true);
        return res;
    }




    public static byte[] Decrypt(byte[] data)
    {
        if (data == null || data.Length == 0) return data;

        uint[] v = ToUInt32Array(data, true);
        uint[] k = ToUInt32Array(Encoding.ASCII.GetBytes(key), false);

        if (k.Length < 4)
        {
            Array.Resize(ref k, 4); // Dodaje 0 sve dok niz nema 4 elementa
        }

        uint n = (uint)v.Length - 1;
        if (n < 1) return data;

        uint rounds = 6 + 52 / (n + 1);
        uint sum = rounds * Delta;

        for (uint i = 0; i < rounds; i++)
        {
            uint e = (sum >> 2) & 3;

            for (uint p = n; p > 0; p--)
            {
                uint z = v[p - 1];
                uint y = v[p];
                v[p] -= ((z >> 5) ^ (y << 2)) + ((y ^ sum) + (k[(p & 3) ^ e] ^ z));
            }

            uint zLast = v[n];
            uint yFirst = v[0];
            v[0] -= ((zLast >> 5) ^ (yFirst << 2)) + ((yFirst ^ sum) + (k[(n & 3) ^ e] ^ zLast));

            sum -= Delta;
        }

        // Vraća podatke u originalnom formatu (izbacujući dužinu)
        return ToByteArray(v, false);
    }





    private static uint[] ToUInt32Array(byte[] data, bool includeLength)
    {
        int n = (data.Length + 3) / 4;
        uint[] result = new uint[includeLength ? n + 1 : n];
        Buffer.BlockCopy(data, 0, result, 0, data.Length);//uint je velicine 4 bajta,pa ce ovo podeliti da svaki element niza result sadrzi blok od 4 slova.Obrnuti redosled ide

        if (includeLength)
            result[n] = (uint)data.Length;

        return result;
    }

    private static byte[] ToByteArray(uint[] data, bool includeLength)
    {
        int n = data.Length * 4;
        if (includeLength)
        {
            int m = (int)data[data.Length - 1];
            n = m < n ? m : n;
        }

        byte[] result = new byte[n];
        Buffer.BlockCopy(data, 0, result, 0, n);

        return result;
    }
}
