namespace SuggestionsSample.ViewModels;

public class Suggestion
{
    public Suggestion(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public override string ToString()
    {
        return "Suggestion " + Id;
    }
}