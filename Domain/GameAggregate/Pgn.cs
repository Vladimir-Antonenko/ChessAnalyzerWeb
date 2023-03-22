namespace Domain.GameAggregate;

public class Pgn
{
    public string Content { get; private set; }

    private Pgn() { }

    private Pgn(string fileContent)
    {
        Content = fileContent;
    }

    public static Pgn Create(string fileContent) => new(fileContent);
}
