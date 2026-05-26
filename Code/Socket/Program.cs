using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

if (args.Length > 0 && args[0].Equals("client", StringComparison.OrdinalIgnoreCase))
{
	await RunClientAsync();
	return;
}

var symbols = new[] { "AAA", "BBB", "CCC", "DDD", "EEE" };
var state = symbols.ToDictionary(s => s, s => new StockState(s, GetStartPrice()));

var listener = new TcpListener(IPAddress.Loopback, 5055);
listener.Start();

Console.WriteLine("TCP socket server is running on 127.0.0.1:5055");
Console.WriteLine("Clients receive one JSON array per second (NDJSON). Press Ctrl+C to stop.");

using var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
	e.Cancel = true;
	cts.Cancel();
};

var acceptTask = AcceptClientsAsync(listener, cts.Token);
await BroadcastLoopAsync(state, cts.Token);
await acceptTask;

static decimal GetStartPrice()
{
	return Random.Shared.Next(120, 350) + Random.Shared.Next(0, 99) / 100m;
}

static async Task HandleClientAsync(TcpClient client, CancellationToken token)
{
	using var _ = client;
	client.NoDelay = true;
	var endPoint = client.Client.RemoteEndPoint?.ToString() ?? "unknown";
	Console.WriteLine($"Client connected: {endPoint}");

	try
	{
		while (!token.IsCancellationRequested && client.Connected)
		{
			await Task.Delay(1000, token);
		}
	}
	catch (OperationCanceledException)
	{
		// Ignore cancellation.
	}
	finally
	{
		Console.WriteLine($"Client disconnected: {endPoint}");
	}
}

static async Task BroadcastLoopAsync(Dictionary<string, StockState> state, CancellationToken token)
{
	while (!token.IsCancellationRequested)
	{
		var snapshot = state.Values.Select(s => s.NextTick()).ToList();
		var payload = JsonSerializer.Serialize(snapshot);
		var bytes = Encoding.UTF8.GetBytes(payload + "\n");

		foreach (var client in TcpClientRegistry.Clients)
		{
			if (!client.Connected)
			{
				TcpClientRegistry.Remove(client);
				continue;
			}

			try
			{
				await client.GetStream().WriteAsync(bytes, token);
			}
			catch
			{
				TcpClientRegistry.Remove(client);
			}
		}

		await Task.Delay(1000, token);
	}
}

static async Task RunClientAsync()
{
	Console.WriteLine("Client mode: connecting to 127.0.0.1:5055...");
	using var client = new TcpClient();
	await client.ConnectAsync(IPAddress.Loopback, 5055);

	using var stream = client.GetStream();
	using var reader = new StreamReader(stream, Encoding.UTF8);

	while (true)
	{
		var line = await reader.ReadLineAsync();
		if (line is null)
		{
			break;
		}

		Console.WriteLine(line);
	}
}

static async Task HandleClientAsyncWithRegistry(TcpClient client, CancellationToken token)
{
	TcpClientRegistry.Add(client);
	await HandleClientAsync(client, token);
	TcpClientRegistry.Remove(client);
}

static async Task AcceptClientsAsync(TcpListener listener, CancellationToken token)
{
	while (!token.IsCancellationRequested)
	{
		TcpClient client;
		try
		{
			client = await listener.AcceptTcpClientAsync(token);
		}
		catch (OperationCanceledException)
		{
			break;
		}

		_ = Task.Run(() => HandleClientAsyncWithRegistry(client, token), token);
	}
}

static class TcpClientRegistry
{
	private static readonly object Gate = new();
	private static readonly List<TcpClient> Items = new();

	public static IReadOnlyList<TcpClient> Clients
	{
		get
		{
			lock (Gate)
			{
				return Items.ToList();
			}
		}
	}

	public static void Add(TcpClient client)
	{
		lock (Gate)
		{
			Items.Add(client);
		}
	}

	public static void Remove(TcpClient client)
	{
		lock (Gate)
		{
			Items.Remove(client);
		}
	}
}

sealed class StockState
{
	private readonly string _symbol;
	private decimal _lastPrice;

	public StockState(string symbol, decimal startPrice)
	{
		_symbol = symbol;
		_lastPrice = startPrice;
	}

	public StockTick NextTick()
	{
		var change = (decimal)(Random.Shared.NextDouble() * 2.0 - 1.0); // -1.0 .. 1.0
		_lastPrice = Math.Max(1m, _lastPrice + change);

		return new StockTick(
			_symbol,
			Math.Round(_lastPrice, 2),
			Math.Round(change, 2),
			Random.Shared.Next(100, 2000),
			DateTimeOffset.UtcNow);
	}
}

record StockTick(string Symbol, decimal Price, decimal Change, int Volume, DateTimeOffset Timestamp);
