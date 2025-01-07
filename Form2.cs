using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.Threading;
using WsProxyChecker.Components;
using System.Runtime.InteropServices;
using System.IO;
using System.Text.Json;

namespace WsProxyChecker
{
    public partial class Form2 : Form
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        private readonly GlitchAccountCreator accountCreator;
        private CancellationTokenSource cancellationTokenSource;
        private SemaphoreSlim semaphore;

        private readonly string captchaScript = @"
(function() {
    // Create container for reCAPTCHA
    const container = document.createElement('div');
    container.id = 'captcha-container';
    document.body.appendChild(container);

    // Style container
    container.style.position = 'fixed';
    container.style.top = '50%';
    container.style.left = '50%';
    container.style.transform = 'translate(-50%, -50%)';
    container.style.zIndex = '9999';
    container.style.backgroundColor = '#2c2c2c';
    container.style.padding = '20px';
    container.style.borderRadius = '5px';
    container.style.boxShadow = '0 2px 10px rgba(0,0,0,0.3)';
    container.style.color = '#fff';

    // Add close button
    const closeButton = document.createElement('button');
    closeButton.textContent = '×';
    closeButton.style.position = 'absolute';
    closeButton.style.right = '5px';
    closeButton.style.top = '5px';
    closeButton.style.border = 'none';
    closeButton.style.background = 'none';
    closeButton.style.fontSize = '20px';
    closeButton.style.cursor = 'pointer';
    closeButton.style.color = '#fff';
    closeButton.onclick = function() {
        container.remove();
    };
    container.appendChild(closeButton);

    // Add title
    const title = document.createElement('div');
    title.textContent = 'Solve reCAPTCHA to get token';
    title.style.marginBottom = '15px';
    title.style.textAlign = 'center';
    title.style.fontFamily = 'Arial, sans-serif';
    container.appendChild(title);

    // Add dedicated div for reCAPTCHA widget
    const widgetContainer = document.createElement('div');
    widgetContainer.id = 'recaptcha-widget';
    container.appendChild(widgetContainer);

    // Function to be called when reCAPTCHA loads
    window.onRecaptchaLoad = function() {
        try {
            console.log('Initializing reCAPTCHA...');
            grecaptcha.render('recaptcha-widget', {
                'sitekey': '6LcqF6gZAAAAAHE-lzA_9GAux7eX9OHaQ5VdEo0C',
                'theme': 'dark',
                'callback': function(token) {
                    console.log('%cCaptcha Token:', 'color: #4CAF50; font-weight: bold');
                    console.log(token);
                },
                'expired-callback': function() {
                    console.log('%cToken expired, please solve again', 'color: #f44336');
                },
                'error-callback': function() {
                    console.log('%cError occurred, please try again', 'color: #f44336');
                }
            });
            console.log('reCAPTCHA initialized, please solve the captcha');
        } catch (error) {
            console.error('Error initializing reCAPTCHA:', error);
        }
    };

    // Add reCAPTCHA script with callback
    const script = document.createElement('script');
    script.src = 'https://www.google.com/recaptcha/api.js?onload=onRecaptchaLoad&render=explicit';
    script.async = true;
    script.defer = true;
    document.head.appendChild(script);
})();
";

        public Form2()
        {
            AllocConsole();
            InitializeComponent();
            ApplyDarkTheme();
            SetupDataGridView();
            
            accountCreator = new GlitchAccountCreator();
            
            // Wire up events
            generateButton.Click += GenerateButton_Click;
            multithreadingCheckbox.CheckedChanged += MultithreadingCheckbox_CheckedChanged;
            threadsSlider.ValueChanged += ThreadsSlider_ValueChanged;
            titleBar.MouseDown += TitleBar_MouseDown;
            copyScriptButton.Click += CopyScriptButton_Click;
            useTokenButton.Click += UseTokenButton_Click;
            exportButton.Click += exportButton_Click;
            importButton.Click += importButton_Click;

            UpdateStats();
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(24, 24, 24);
            accountsGrid.BackgroundColor = Color.FromArgb(32, 32, 32);
            accountsGrid.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
            accountsGrid.DefaultCellStyle.ForeColor = Color.White;
            accountsGrid.GridColor = Color.FromArgb(45, 45, 45);
            accountsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 28, 28);
            accountsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void SetupDataGridView()
        {
            accountsGrid.Columns.Add("Email", "Email");
            accountsGrid.Columns.Add("Token", "Token");
            accountsGrid.Columns.Add("Status", "Status");
            accountsGrid.Columns.Add("Projects", "Projects");
            accountsGrid.Columns.Add("LastUsed", "Last Used");

            foreach (DataGridViewColumn column in accountsGrid.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void TitleBar_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                NativeWinAPI.ReleaseCapture();
                NativeWinAPI.SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }

        private async void GenerateButton_Click(object sender, EventArgs e)
        {
            generateButton.Enabled = false;
            int amount = (int)accountsAmount.Value;

            try
            {
                if (multithreadingCheckbox.Checked)
                {
                    await PrepareAccountsMultithreaded(amount);
                }
                else
                {
                    await PrepareAccountsSequential(amount);
                }
            }
            finally
            {
                generateButton.Enabled = true;
                UpdateStats();
            }
        }

        private async Task PrepareAccountsSequential(int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                await PrepareAccount();
            }
        }

        private async Task PrepareAccountsMultithreaded(int amount)
        {
            cancellationTokenSource = new CancellationTokenSource();
            semaphore = new SemaphoreSlim(threadsSlider.Value);

            var tasks = Enumerable.Range(0, amount)
                .Select(_ => PrepareAccountThreaded(cancellationTokenSource.Token));

            await Task.WhenAll(tasks);
        }

        private async Task PrepareAccountThreaded(CancellationToken token)
        {
            try
            {
                await semaphore.WaitAsync(token);
                await PrepareAccount();
            }
            finally
            {
                semaphore?.Release();
            }
        }

        private async Task PrepareAccount()
        {
            var row = accountsGrid.Rows[accountsGrid.Rows.Add()];
            row.Cells["Status"].Value = "Preparing...";

            try
            {
                Console.WriteLine("Creating temporary email...");
                var email = await accountCreator.PrepareEmail();
                row.Cells["Email"].Value = email;
                row.Cells["Status"].Value = "Ready for token";
                Console.WriteLine($"Account prepared with email: {email}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error preparing account: {ex.Message}");
                row.Cells["Status"].Value = "Error: " + ex.Message;
            }
        }

        private void UpdateStats()
        {
            int total = accountsGrid.Rows.Count;
            int valid = accountsGrid.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["Status"].Value?.ToString() == "Created");
            int invalid = accountsGrid.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["Status"].Value?.ToString()?.StartsWith("Error") == true);

            totalLabel.Text = $"Total: {total}";
            validLabel.Text = $"Valid: {valid}";
            invalidLabel.Text = $"Invalid: {invalid}";
        }

        private void MultithreadingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            threadsSlider.Enabled = multithreadingCheckbox.Checked;
        }

        private void ThreadsSlider_ValueChanged(object sender, EventArgs e)
        {
            threadsLabel.Text = $"Threads: {threadsSlider.Value}";
        }

        private void CopyScriptButton_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(captchaScript);
            MessageBox.Show(
                "Script copied to clipboard!\n\n" +
                "1. Open browser console (F12)\n" +
                "2. Paste and run the script\n" +
                "3. Solve the reCAPTCHA that appears\n" +
                "4. Copy the token from console (in green)\n" +
                "5. Paste token here and click 'Create Accounts with Token'",
                "Instructions",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private async void UseTokenButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(tokenTextBox.Text))
            {
                MessageBox.Show("Please paste a reCAPTCHA token first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            useTokenButton.Enabled = false;
            try
            {
                accountCreator.SetRecaptchaToken(tokenTextBox.Text);
                
                // Find all rows that are ready for token
                var readyRows = accountsGrid.Rows.Cast<DataGridViewRow>()
                    .Where(r => r.Cells["Status"].Value?.ToString() == "Ready for token")
                    .ToList();

                if (!readyRows.Any())
                {
                    MessageBox.Show("No accounts ready for token found!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                foreach (var row in readyRows)
                {
                    try
                    {
                        var email = row.Cells["Email"].Value?.ToString();
                        if (string.IsNullOrEmpty(email)) continue;

                        row.Cells["Status"].Value = "Creating...";
                        Console.WriteLine($"Creating account for {email}...");
                        
                        var account = await accountCreator.CreateAccountWithEmail(email);
                        
                        // Store the token in the grid
                        row.Cells["Token"].Value = account.Token;
                        row.Cells["Status"].Value = "Created";
                        row.Cells["Projects"].Value = account.ProjectCount.ToString();
                        row.Cells["LastUsed"].Value = account.CreatedAt.ToString("HH:mm:ss");
                        Console.WriteLine($"Account created successfully: {email}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating account: {ex.Message}");
                        row.Cells["Status"].Value = "Error: " + ex.Message;
                    }
                }

                tokenTextBox.Clear();
            }
            finally
            {
                useTokenButton.Enabled = true;
                UpdateStats();
            }
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;
                saveFileDialog.Title = "Export Accounts";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var accounts = new List<Dictionary<string, string>>();
                        foreach (DataGridViewRow row in accountsGrid.Rows)
                        {
                            var email = row.Cells["Email"].Value?.ToString();
                            var token = row.Cells["Token"].Value?.ToString();
                            var status = row.Cells["Status"].Value?.ToString();
                            if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(token))
                            {
                                accounts.Add(new Dictionary<string, string>
                                {
                                    { "email", email },
                                    { "token", token },
                                    { "status", status ?? "Unknown" }
                                });
                            }
                        }

                        var json = JsonSerializer.Serialize(accounts, new JsonSerializerOptions 
                        { 
                            WriteIndented = true 
                        });
                        File.WriteAllText(saveFileDialog.FileName, json);
                        MessageBox.Show("Accounts exported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void importButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 1;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var json = File.ReadAllText(openFileDialog.FileName);
                        var accounts = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);

                        foreach (var account in accounts)
                        {
                            if (account.TryGetValue("email", out string email) && 
                                account.TryGetValue("token", out string token))
                            {
                                var rowIndex = accountsGrid.Rows.Add();
                                var row = accountsGrid.Rows[rowIndex];
                                row.Cells["Email"].Value = email;
                                row.Cells["Token"].Value = token;
                                row.Cells["Status"].Value = account.GetValueOrDefault("status", "Imported");
                            }
                        }

                        UpdateStats();
                        MessageBox.Show("Accounts imported successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
