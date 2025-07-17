namespace WebTemplate.Services.Interfaces
{
    public interface IAlgorithm
    {
        public Byte[] Encrypt(Byte[] content);
        public Byte[] Decrypt(Byte[] encryptedContent);
       
    }
}
