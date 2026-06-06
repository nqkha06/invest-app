using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using StockExchange.Shared.DTOs;

namespace StockExchange.Client.WinForms.Services;

public class AuthClientService
{
	private readonly string _host;
	private readonly int _port;

	public AuthClientService(string host = "127.0.0.1", int port = 7000)
	{
		_host = host;
		_port = port;
	}

	public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
	{
		var response = new LoginResponseDto { Success = false, Message = "Unable to connect to server." };
		try
		{
			using var client = new TcpClient();
			await client.ConnectAsync(_host, _port);
			using var network = client.GetStream();

			var payload = JsonSerializer.Serialize(request);
			var bytes = Encoding.UTF8.GetBytes(payload);
			var lengthPrefix = BitConverter.GetBytes(bytes.Length);
			await network.WriteAsync(lengthPrefix.AsMemory(0, 4));
			await network.WriteAsync(bytes.AsMemory(0, bytes.Length));

			// read length prefix
			var lenBuf = new byte[4];
			var read = await network.ReadAsync(lenBuf.AsMemory(0, 4));
			if (read != 4)
			{
				response.Message = "Invalid response from server.";
				return response;
			}

			var respLen = BitConverter.ToInt32(lenBuf, 0);
			var respBuf = new byte[respLen];
			var total = 0;
			while (total < respLen)
			{
				var r = await network.ReadAsync(respBuf.AsMemory(total, respLen - total));
				if (r == 0) break;
				total += r;
			}

			var respJson = Encoding.UTF8.GetString(respBuf, 0, total);
			var loginResp = JsonSerializer.Deserialize<LoginResponseDto>(respJson);
			return loginResp ?? response;
		}
		catch (Exception ex)
		{
			response.Message = ex.Message;
			return response;
		}
	}
}
