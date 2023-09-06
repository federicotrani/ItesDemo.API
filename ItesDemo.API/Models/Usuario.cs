using System.ComponentModel.DataAnnotations;

namespace ItesDemo.API.Models;

public class Usuario
{
    [Key]
    public int id { get; set; }
    public string email { get; set; }
    public string nombre { get; set; }
    public string password { get; set; }
    public string usuario { get; set; }
}
