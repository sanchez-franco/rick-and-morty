using System.ComponentModel.DataAnnotations;

namespace RickMortyApi.Database
{
    public class AccountDb
    {
        [Key]
        public int Id { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    public class FavoriteDb
    {
        public int Id { get; set; }
        public string Email { get; set; }
    }
}
