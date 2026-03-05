namespace Reg4MissionX.Models;

public class Municipality
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string CountyCode { get; set; } = "";  // "01"
    public string CountyName { get; set; } = "";  // "Stockholms län"
    public string ScbCode { get; set; } = "";     // "0114"
}