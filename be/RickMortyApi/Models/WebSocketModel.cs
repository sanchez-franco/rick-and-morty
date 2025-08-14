namespace RickMortyApi.Models
{
    public class WebSocketModel
    {
        public bool IsRemoved { get; set; }
        public IList<int> Favorites { get; set;  }
    }
}
