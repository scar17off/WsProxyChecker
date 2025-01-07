using Guna.UI2.WinForms;
using System.Net.Http;
using System.Text.Json;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace WsProxyChecker
{
    public partial class Form1 : Form
    {
        private List<ProxyInfo> proxies = new List<ProxyInfo>();
        private HttpClient httpClient = new HttpClient();
        private CancellationTokenSource cancellationTokenSource;
        private SemaphoreSlim semaphore;

        public Form1()
        {
            InitializeComponent();
            InitializeContextMenu();
            SetupDataGridView();
            ApplyDarkTheme();

            // Add title bar drag functionality
            titleBar.MouseDown += TitleBar_MouseDown;

            // Wire up button events
            importButton.Click += importButton_Click;
            exportButton.Click += exportButton_Click;
            checkAllButton.Click += CheckAll_Click;
            removeInvalidButton.Click += removeInvalidButton_Click;

            threadsLabel.Text = $"Threads: {threadsSlider.Value}";

            UpdateStats();

            // Button click handlers
            accountsButton.Click += AccountsButton_Click;
            projectsButton.Click += ProjectsButton_Click;
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(24, 24, 24);
            proxyGrid.BackgroundColor = Color.FromArgb(32, 32, 32);
            proxyGrid.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
            proxyGrid.DefaultCellStyle.ForeColor = Color.White;
            proxyGrid.GridColor = Color.FromArgb(45, 45, 45);
            proxyGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 28, 28);
            proxyGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
        }

        private void SetupDataGridView()
        {
            proxyGrid.Columns.Add("Proxy", "Proxy");
            proxyGrid.Columns.Add("Status", "Status");
            proxyGrid.Columns.Add("Connections", "Connections");
            proxyGrid.Columns.Add("Uptime", "Uptime");
            proxyGrid.Columns.Add("LastChecked", "Last Checked");

            foreach (DataGridViewColumn column in proxyGrid.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void InitializeContextMenu()
        {
            var menu = new Guna2ContextMenuStrip();
            menu.BackColor = Color.FromArgb(32, 32, 32);
            menu.ForeColor = Color.White;
            
            // Set renderer colors
            menu.RenderStyle.ArrowColor = Color.White;
            menu.RenderStyle.BorderColor = Color.FromArgb(45, 45, 45);
            
            // Create custom renderer
            menu.Renderer = new ToolStripProfessionalRenderer(new CustomColorTable());

            var addItem = new ToolStripMenuItem("Add Proxy");
            var checkItem = new ToolStripMenuItem("Check Selected");
            var checkAllItem = new ToolStripMenuItem("Check All");
            var removeItem = new ToolStripMenuItem("Remove Selected");
            var clearItem = new ToolStripMenuItem("Clear All");
            var copyItem = new ToolStripMenuItem("Copy Proxy");

            // Style each menu item
            ToolStripMenuItem[] items = { addItem, checkItem, checkAllItem, removeItem, clearItem, copyItem };
            foreach (var item in items)
            {
                item.BackColor = Color.FromArgb(32, 32, 32);
                item.ForeColor = Color.White;
            }

            addItem.Click += AddProxy_Click;
            checkItem.Click += CheckSelected_Click;
            checkAllItem.Click += CheckAll_Click;
            removeItem.Click += RemoveSelected_Click;
            clearItem.Click += ClearAll_Click;
            copyItem.Click += CopyProxy_Click;

            menu.Items.AddRange(items);
            proxyGrid.ContextMenuStrip = menu;
        }

        private async Task CheckProxy(string proxy)
        {
            try
            {
                var url = proxy.StartsWith("http") ? proxy : $"https://{proxy}.glitch.me";
                var response = await httpClient.GetAsync($"{url}/status");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var status = JsonSerializer.Deserialize<ProxyStatus>(content);

                    UpdateProxyStatus(proxy, "Online", status.connections, TimeSpan.FromSeconds(status.uptime));
                }
                else
                {
                    UpdateProxyStatus(proxy, "Offline", 0, TimeSpan.Zero);
                }
            }
            catch
            {
                UpdateProxyStatus(proxy, "Error", 0, TimeSpan.Zero);
            }
        }

        private void UpdateProxyStatus(string proxy, string status, int connections, TimeSpan uptime)
        {
            var row = proxyGrid.Rows.Cast<DataGridViewRow>()
                .FirstOrDefault(r => r.Cells["Proxy"].Value.ToString() == proxy);

            if (row != null)
            {
                row.Cells["Status"].Value = status;
                row.Cells["Connections"].Value = connections;
                row.Cells["Uptime"].Value = FormatUptime(uptime);
                row.Cells["LastChecked"].Value = DateTime.Now.ToString("HH:mm:ss");

                // Color the status cell based on the result
                row.Cells["Status"].Style.ForeColor = status switch
                {
                    "Online" => Color.LightGreen,
                    "Offline" => Color.Red,
                    _ => Color.Orange
                };
            }

            UpdateStats();
        }

        private string FormatUptime(TimeSpan uptime)
        {
            if (uptime == TimeSpan.Zero) return "-";
            return $"{(int)uptime.TotalHours}h {uptime.Minutes}m {uptime.Seconds}s";
        }

        // Context menu event handlers
        private void AddProxy_Click(object sender, EventArgs e)
        {
            using var input = new Guna2TextBox
            {
                PlaceholderText = "Enter proxy URL or Glitch project name",
                Width = 300
            };

            var dialog = new Form
            {
                Text = "Add Proxy",
                ClientSize = new Size(320, 100),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                BackColor = Color.FromArgb(24, 24, 24)
            };

            var okButton = new Guna2Button
            {
                Text = "Add",
                Location = new Point(120, 50),
                Size = new Size(80, 30)
            };

            okButton.Click += (s, ev) =>
            {
                if (!string.IsNullOrWhiteSpace(input.Text))
                {
                    proxyGrid.Rows.Add(input.Text, "Not Checked", "-", "-", "-");
                    dialog.Close();
                }
            };

            dialog.Controls.AddRange(new Control[] { input, okButton });
            dialog.ShowDialog(this);
        }

        private async void CheckSelected_Click(object sender, EventArgs e)
        {
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                var selectedProxies = proxyGrid.SelectedRows.Cast<DataGridViewRow>()
                    .Select(r => r.Cells["Proxy"].Value.ToString())
                    .ToList();

                if (multithreadingCheckbox.Checked)
                {
                    int threadCount = threadsSlider.Value;
                    semaphore = new SemaphoreSlim(threadCount);
                    
                    var tasks = selectedProxies.Select(proxy => CheckProxyThreaded(proxy, cancellationTokenSource.Token));
                    await Task.WhenAll(tasks);
                }
                else
                {
                    foreach (var proxy in selectedProxies)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested) break;
                        await CheckProxy(proxy);
                    }
                }
            }
            finally
            {
                semaphore?.Dispose();
                semaphore = null;
            }
        }

        private async void CheckAll_Click(object sender, EventArgs e)
        {
            // Cancel any ongoing checks
            cancellationTokenSource?.Cancel();
            cancellationTokenSource = new CancellationTokenSource();

            try
            {
                // Disable buttons during check
                checkAllButton.Enabled = false;
                importButton.Enabled = false;
                
                var proxies = proxyGrid.Rows.Cast<DataGridViewRow>()
                    .Select(r => r.Cells["Proxy"].Value.ToString())
                    .ToList();

                if (multithreadingCheckbox.Checked)
                {
                    int threadCount = threadsSlider.Value;
                    semaphore = new SemaphoreSlim(threadCount);
                    
                    var tasks = proxies.Select(proxy => CheckProxyThreaded(proxy, cancellationTokenSource.Token));
                    await Task.WhenAll(tasks);
                }
                else
                {
                    foreach (var proxy in proxies)
                    {
                        if (cancellationTokenSource.Token.IsCancellationRequested) break;
                        await CheckProxy(proxy);
                    }
                }
            }
            finally
            {
                // Re-enable buttons
                checkAllButton.Enabled = true;
                importButton.Enabled = true;
                
                semaphore?.Dispose();
                semaphore = null;
            }
        }

        private async Task CheckProxyThreaded(string proxy, CancellationToken cancellationToken)
        {
            try
            {
                await semaphore.WaitAsync(cancellationToken);
                await CheckProxy(proxy);
            }
            finally
            {
                if (semaphore != null) // Check if not disposed
                {
                    semaphore.Release();
                }
            }
        }

        private void RemoveSelected_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in proxyGrid.SelectedRows)
            {
                proxyGrid.Rows.Remove(row);
            }
        }

        private void ClearAll_Click(object sender, EventArgs e)
        {
            proxyGrid.Rows.Clear();
        }

        private void CopyProxy_Click(object sender, EventArgs e)
        {
            if (proxyGrid.SelectedRows.Count > 0)
            {
                var proxy = proxyGrid.SelectedRows[0].Cells["Proxy"].Value.ToString();
                Clipboard.SetText(proxy);
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

        private class CustomColorTable : ProfessionalColorTable
        {
            public override Color MenuItemSelected => Color.FromArgb(45, 45, 45);
            public override Color MenuItemSelectedGradientBegin => Color.FromArgb(45, 45, 45);
            public override Color MenuItemSelectedGradientEnd => Color.FromArgb(45, 45, 45);
            public override Color MenuItemBorder => Color.FromArgb(45, 45, 45);
            public override Color MenuBorder => Color.FromArgb(45, 45, 45);
            public override Color MenuItemPressedGradientBegin => Color.FromArgb(45, 45, 45);
            public override Color MenuItemPressedGradientEnd => Color.FromArgb(45, 45, 45);
            public override Color MenuItemPressedGradientMiddle => Color.FromArgb(45, 45, 45);
            public override Color ToolStripDropDownBackground => Color.FromArgb(32, 32, 32);
            public override Color ImageMarginGradientBegin => Color.FromArgb(32, 32, 32);
            public override Color ImageMarginGradientMiddle => Color.FromArgb(32, 32, 32);
            public override Color ImageMarginGradientEnd => Color.FromArgb(32, 32, 32);
        }

        private void UpdateStats()
        {
            int total = proxyGrid.Rows.Count;
            int valid = proxyGrid.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["Status"].Value?.ToString() == "Online");
            int invalid = proxyGrid.Rows.Cast<DataGridViewRow>()
                .Count(r => r.Cells["Status"].Value?.ToString() == "Offline" || r.Cells["Status"].Value?.ToString() == "Error");

            totalLabel.Text = $"Total: {total}";
            validLabel.Text = $"Valid: {valid}";
            invalidLabel.Text = $"Invalid: {invalid}";
        }

        private async void importButton_Click(object sender, EventArgs e)
        {
            using var dialog = new OpenFileDialog
            {
                Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*",
                Title = "Import Proxies"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var proxies = await File.ReadAllLinesAsync(dialog.FileName);
                foreach (var proxy in proxies.Where(p => !string.IsNullOrWhiteSpace(p)))
                {
                    proxyGrid.Rows.Add(proxy.Trim(), "Not Checked", "-", "-", "-");
                }
                UpdateStats();
            }
        }

        private async void exportButton_Click(object sender, EventArgs e)
        {
            using var dialog = new SaveFileDialog
            {
                Filter = "Text files (*.txt)|*.txt",
                Title = "Export Valid Proxies"
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var validProxies = proxyGrid.Rows.Cast<DataGridViewRow>()
                    .Where(r => r.Cells["Status"].Value?.ToString() == "Online")
                    .Select(r => r.Cells["Proxy"].Value.ToString());

                await File.WriteAllLinesAsync(dialog.FileName, validProxies);
            }
        }

        private void removeInvalidButton_Click(object sender, EventArgs e)
        {
            var invalidRows = proxyGrid.Rows.Cast<DataGridViewRow>()
                .Where(r => r.Cells["Status"].Value?.ToString() == "Offline" || 
                            r.Cells["Status"].Value?.ToString() == "Error")
                .ToList();

            foreach (var row in invalidRows)
            {
                proxyGrid.Rows.Remove(row);
            }
            UpdateStats();
        }

        private void MultithreadingCheckbox_CheckedChanged(object sender, EventArgs e)
        {
            threadsSlider.Enabled = multithreadingCheckbox.Checked;
        }

        private void ThreadsSlider_ValueChanged(object sender, EventArgs e)
        {
            threadsLabel.Text = $"Threads: {threadsSlider.Value}";
        }

        private void UpdateCheckProgress(int checkedCount, int total)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateCheckProgress(checkedCount, total)));
                return;
            }

            checkAllButton.Text = $"Checking ({checkedCount}/{total})";
        }

        private void AccountsButton_Click(object sender, EventArgs e)
        {
            var accountsForm = new Form2();
            accountsForm.Show();
        }

        private void ProjectsButton_Click(object sender, EventArgs e)
        {
            var projectsForm = new Form3();
            projectsForm.Show();
        }
    }

    class ProxyStatus
    {
        public string status { get; set; }
        public int connections { get; set; }
        public double uptime { get; set; }
    }

    class ProxyInfo
    {
        public string Url { get; set; }
        public string Status { get; set; }
        public int Connections { get; set; }
        public TimeSpan Uptime { get; set; }
        public DateTime LastChecked { get; set; }
    }
}
