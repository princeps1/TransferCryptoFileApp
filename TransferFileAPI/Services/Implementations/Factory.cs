namespace WebTemplate.Services.Implementations;

public class Factory : IFactory
{
    private readonly Railfence _railfence;
    private readonly XXTEA _xxtea;
    private readonly XXTEACBC _xxteacbc;

    public Factory(Railfence railfence,XXTEA xxtea,XXTEACBC xxteacbc)
    {
        _railfence = railfence;
        _xxtea = xxtea;
        _xxteacbc = xxteacbc;
    }

    public IAlgorithm GetService(AlgorithmType type)
    {
        return type switch
        {
            AlgorithmType.Railfence => _railfence,
            AlgorithmType.XXTEA => _xxtea,
            AlgorithmType.XXTEACBC => _xxteacbc,
            _ => throw new NotImplementedException($"Service for {type} is not implemented.")
        };
    }
}
