namespace SuggestionsSample.ViewModels;

public class Transaction
{
    public Transaction(int id)
    {
        Id = id;
    }

    public int Id { get; }

    public override string ToString()
    {
        return "Transaction " + Id;
    }
}