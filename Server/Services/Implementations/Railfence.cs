namespace WebTemplate.Services.Implementations;

public class Railfence : IAlgorithm
{
    public Byte[] Encrypt(Byte[] content)
    {
        List<Byte> EncryptedContent = new List<Byte>();

        int Depth = 2;
        List<int> Key = MakeKey(Depth); // Create key

        int left = 0, right = Depth - 1;
        // Encode content
        for (int i = 0; i < Depth; i++) // Number of rows
        {
            int j = 0;
            int index = i;
            do
            {
                if ((j % 2) == 0)
                {
                    EncryptedContent.Add(content[index]);
                    index += Key[left];
                }
                else
                {
                    EncryptedContent.Add(content[index]);
                    index += Key[right];
                }
                j++;
            } while (index < content.Count());
            left++;
            right--;
        }
        return EncryptedContent.ToArray();
    }

    public Byte[] Decrypt(Byte[] encryptedContent)
    {
        List<Byte> DecodedContent = new List<Byte>(new Byte[encryptedContent.Length]);
        int Depth = 2;
        List<int> Key = MakeKey(Depth); // Create key

        int[] rowLengths = new int[Depth];
        int left = 0, right = Depth - 1;

        // Calculate row lengths
        for (int i = 0; i < Depth; i++)
        {
            int currentIndex = i;
            int j = 0;
            do
            {
                rowLengths[i]++;
                currentIndex += (j % 2 == 0) ? Key[left] : Key[right];
                j++;
            } while (currentIndex < encryptedContent.Length);

            left++;
            right--;
        }

        // Set row start indices
        int[] rowStartIndices = new int[Depth]; rowStartIndices[0] = 0;
        for (int i = 1; i < Depth; i++)
        {
            rowStartIndices[i] = rowStartIndices[i - 1] + rowLengths[i - 1];
        }

        // Decode content
        left = 0;
        right = Depth - 1;
        for (int i = 0; i < Depth; i++)
        {
            int rowIndex = rowStartIndices[i];
            int j = 0;
            int currentIndex = i;

            do
            {
                DecodedContent[currentIndex] = encryptedContent[rowIndex++];
                currentIndex += (j % 2 == 0) ? Key[left] : Key[right];
                j++;
            } while (currentIndex < encryptedContent.Length);

            left++;
            right--;
        }

        return DecodedContent.ToArray();
    }


    public List<int> MakeKey(int Depth)
    {
        List<int> Key = new List<int>();
        int m = 0;
        int factor = Depth * 2 - 2;

        // Create the key
        try
        {
            for (int i = 0; i < Depth; i++)
            {
                Key.Add(factor - m);
                m += 2;
            }
            if (Key.Count == 0)
                throw new ArgumentException("Depth must be greater than 0.");
            Key[Key.Count - 1] = Key[0];
        }
        catch (Exception ex)
        {
            throw;
        }
        return Key;
    }
}
