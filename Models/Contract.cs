using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DotnetBackend.Models;

public class ContractModel
{
    [BsonId]
    public string? Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("value")]
    public double Value { get; set; }

    [BsonElement("yearlyPlus")]
    public string YearlyPlus { get; set; }

    [BsonElement("gain")]
    public double Gain { get; set; }

    [BsonElement("duration")]
    public int Duration { get; set; }

    [BsonElement("firstWithdraw")]
    public int FirstWithdraw { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("imageUrl")]
    public string? ImageUrl { get; set; }


    public ContractModel(string title, double value, string yearlyPlus, int duration, string description, double gain, int firstWithdraw)
    {
        Title = title;
        Value = value;
        YearlyPlus = yearlyPlus;
        Duration = duration;
        Description = description;
        Gain = gain;
        FirstWithdraw = firstWithdraw;
    }
}


