namespace BuilderPC.Models;

public class SavedBuild
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Author { get; set; } = "";
    public List<Part> Parts { get; set; } = new();

    public decimal TotalPrice => Parts.Sum(p => p.Price);
    public string TotalPriceDisplay => $"{TotalPrice:N0} ₽";
}
