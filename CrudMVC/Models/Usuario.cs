using System.ComponentModel.DataAnnotations;

namespace CrudMVC.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }
        [Required(ErrorMessage = "O campo Nome é obrigatório.")]
        public string Nome { get; set; }
        [Required(ErrorMessage = "O campo Email é obrigatório."),
            EmailAddress]
        public string Email { get; set; }
        [Required(ErrorMessage = "O campo Cargo é obrigatório.")]
        public string Cargo { get; set; }
    }
}
