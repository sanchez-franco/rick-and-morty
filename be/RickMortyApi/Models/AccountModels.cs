using System.ComponentModel.DataAnnotations;

namespace RickMortyApi.Models
{
    public class AccountDto
    {
        public string Password { get; set; }
        public string Email { get; set; }
    }
}

