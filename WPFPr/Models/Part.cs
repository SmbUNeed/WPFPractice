namespace BuilderPC.Models;

public class Part
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Manufacturer { get; set; } = "";
    public int ManufacturerId { get; set; }
    public int PartTypeId { get; set; }
    public string PartTypeName { get; set; } = "";
    public string Image { get; set; } = "";
    public decimal Price { get; set; }
    public Dictionary<string, string> Specs { get; set; } = new();

    public string PriceDisplay => $"{Price:N0} ₽";
    public string SpecsSummary => string.Join(" | ", Specs.Select(kv => $"{kv.Key}: {kv.Value}"));
}

public class PartType
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class Manufacturer
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public override string ToString() => Name;
}
