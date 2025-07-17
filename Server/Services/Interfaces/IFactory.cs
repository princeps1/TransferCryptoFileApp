using WebTemplate.Enums;

namespace WebTemplate.Services.Interfaces;

public interface IFactory
{
    IAlgorithm GetService(AlgorithmType type);
}
