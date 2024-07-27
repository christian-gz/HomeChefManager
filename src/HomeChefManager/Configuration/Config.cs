namespace HomeChefManager.Configuration;

public class Config : IConfig
{
    public ConnectionStrings ConnectionStrings { get; init; } = new();
}