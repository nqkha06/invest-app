using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using StockExchange.Client.WinForms.Services;
using StockExchange.Shared.DTOs;

namespace StockExchange.Client.WinForms.Forms;

public class LoginForm : Form
{
	private readonly TextBox _txtUsername;
	private readonly TextBox _txtPassword;
	private readonly Button _btnLogin;
	private readonly AuthClientService _authService;

	public LoginForm()
	{
		Text = "Login";
		ClientSize = new Size(320, 160);

		_txtUsername = new TextBox { Location = new Point(20, 20), Width = 280 };
		_txtPassword = new TextBox { Location = new Point(20, 60), Width = 280, UseSystemPasswordChar = true };
		_btnLogin = new Button { Text = "Login", Location = new Point(20, 100), Width = 280 };
		_btnLogin.Click += async (_, __) => await OnLoginClicked();

		Controls.Add(new Label { Text = "Username or Email:", Location = new Point(20, 0), AutoSize = true });
		Controls.Add(_txtUsername);
		Controls.Add(new Label { Text = "Password:", Location = new Point(20, 40), AutoSize = true });
		Controls.Add(_txtPassword);
		Controls.Add(_btnLogin);

		_authService = new AuthClientService();
	}

	private async Task OnLoginClicked()
	{
		_btnLogin.Enabled = false;
		try
		{
			var req = new LoginRequestDto
			{
				UsernameOrEmail = _txtUsername.Text.Trim(),
				Password = _txtPassword.Text
			};

			var resp = await _authService.LoginAsync(req);
			if (resp.Success)
			{
				MessageBox.Show(this, resp.Message, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
				// TODO: Open main form / proceed
			}
			else
			{
				MessageBox.Show(this, resp.Message, "Login failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
			}
		}
		finally
		{
			_btnLogin.Enabled = true;
		}
	}
}
