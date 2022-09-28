using System.ComponentModel.DataAnnotations;

namespace WebApi.Model
{
    public class HomeRequest
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        public string FirstName { get; set; }

        [MaxLength(5)]
        public string LastName { get; set; }

        public string Title { get; set; }
    }
}
