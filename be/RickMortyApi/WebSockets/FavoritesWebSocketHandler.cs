using RickMortyApi.Models;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json.Serialization;

namespace RickMortyApi.WebSockets
{
    public class FavoritesWebSocketHandler
    {
        private static readonly ConcurrentDictionary<string, IList<WebSocket>> _sockets = new();

        public void TryAdd(string email, WebSocket webSocket)
        {
            if (!_sockets.ContainsKey(email))
            {
                _sockets[email] = new List<WebSocket>();
            }
            _sockets[email].Add(webSocket);
            Console.WriteLine($"Cliente conectado: {email}. Total: {_sockets[email].Count}");
        }

        public async Task BroadcastAsync(string email, WebSocketModel message)
        {
            if (!_sockets.ContainsKey(email))
            {
                Console.WriteLine($"No clients connected for email: {email}");
                return;
            }

            Console.WriteLine($"Broadcasting: {message} to {_sockets.Count} clients");
            var json = System.Text.Json.JsonSerializer.Serialize(message, new System.Text.Json.JsonSerializerOptions
            {
                PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
            });
            var buffer = Encoding.UTF8.GetBytes(json);
            var segment = new ArraySegment<byte>(buffer);

            var items = _sockets[email];
            for (var i = items.Count - 1; i >= 0; i--)
            {
                var s = items[i];
                if (s.State != WebSocketState.Open)
                {
                    items.RemoveAt(i);
                }
                else
                {
                    try
                    {
                        await s.SendAsync(segment, WebSocketMessageType.Text, true, CancellationToken.None);

                    }
                    catch(Exception exception)
                    {

                    }
                }
            }
        }
    }
}