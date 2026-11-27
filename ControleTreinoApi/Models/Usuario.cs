using System.Text.Json.Serialization;

namespace ControleTreinoApi.Models;

public class Usuario
{
    public int Id {get; set;}
    public string Nome {get; set;} = string.Empty;
    public string Email {get; set;} = string.Empty; 
    [JsonIgnore]
    public string SenhaHash {get; set;} = string.Empty;
}