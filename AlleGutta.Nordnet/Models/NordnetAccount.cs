namespace AlleGutta.Nordnet.Models;

public class NordnetAccount
{
    public int Accno { get; set; }
    public int Accid { get; set; }
    public string? Type { get; set; }
    public int Atyid { get; set; }
    public string? Symbol { get; set; }
    public string? AccountCode { get; set; }
    public string? Role { get; set; }
    public bool Default { get; set; }
    public string? Alias { get; set; }
}