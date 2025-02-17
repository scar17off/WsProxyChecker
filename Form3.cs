using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;
using System.IO;
using WsProxyChecker.Components;
using System.Text.Json;

namespace WsProxyChecker
{
    public partial class Form3 : Form
    {
        private int proxiesPerAccount = 1;
        private List<Dictionary<string, string>> importedAccounts = new List<Dictionary<string, string>>();
        private string BaseProxyName => baseProxyTextBox.Text.Trim();

        public Form3()
        {
            InitializeComponent();
            ApplyDarkTheme();
            SetupDataGridView();
            
            // Wire up events
            titleBar.MouseDown += TitleBar_MouseDown;
            generateButton.Click += GenerateButton_Click;
            importAccountsButton.Click += ImportAccountsButton_Click;
            importProxiesButton.Click += ImportProxiesButton_Click;
            exportProxiesButton.Click += ExportProxiesButton_Click;
            proxiesPerAccountSlider.ValueChanged += ProxiesPerAccountSlider_ValueChanged;
            filterFailedButton.Click += filterFailedButton_Click;
        }

        private void SetupDataGridView()
        {
            projectsGrid.Columns.Clear();

            // Add columns with specific widths
            projectsGrid.Columns.Add("Account", "Account");
            projectsGrid.Columns.Add("ProxyName", "Proxy Name");
            projectsGrid.Columns.Add("Status", "Status");
            projectsGrid.Columns.Add("Created", "Created");
            projectsGrid.Columns.Add("LastChecked", "Last Checked");

            // Set column widths
            projectsGrid.Columns["Account"].Width = 200;
            projectsGrid.Columns["ProxyName"].Width = 200;
            projectsGrid.Columns["Status"].Width = 100;
            projectsGrid.Columns["Created"].Width = 100;
            projectsGrid.Columns["LastChecked"].Width = 100;

            // Style the grid
            projectsGrid.BackgroundColor = Color.FromArgb(32, 32, 32);
            projectsGrid.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
            projectsGrid.DefaultCellStyle.ForeColor = Color.White;
            projectsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 45, 45);
            projectsGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            projectsGrid.GridColor = Color.FromArgb(45, 45, 45);
            projectsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 28, 28);
            projectsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            projectsGrid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(28, 28, 28);
            projectsGrid.EnableHeadersVisualStyles = false;
            projectsGrid.RowTemplate.Height = 25;

            foreach (DataGridViewColumn column in projectsGrid.Columns)
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            }
        }

        private void ImportAccountsButton_Click(object sender, EventArgs e)
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
                        importedAccounts = JsonSerializer.Deserialize<List<Dictionary<string, string>>>(json);
                        
                        MessageBox.Show("Imported accounts successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing accounts: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async void GenerateButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(BaseProxyName))
                {
                    MessageBox.Show("Please enter a base proxy name!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (importedAccounts.Count == 0)
                {
                    MessageBox.Show("Please import accounts first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                generateButton.Enabled = false;

                // First, create all rows with "Pending" status
                var rowIndices = new Dictionary<string, List<int>>();
                foreach (var account in importedAccounts)
                {
                    if (!account.TryGetValue("email", out string email)) continue;
                    
                    rowIndices[email] = new List<int>();
                    for (int i = 0; i < proxiesPerAccount; i++)
                    {
                        var rowIndex = projectsGrid.Rows.Add();
                        var row = projectsGrid.Rows[rowIndex];
                        
                        row.Cells["Account"].Value = email;
                        row.Cells["ProxyName"].Value = "Pending...";
                        row.Cells["Status"].Value = "Pending";
                        row.Cells["Created"].Value = "-";
                        row.Cells["LastChecked"].Value = "-";

                        rowIndices[email].Add(rowIndex);
                    }
                }

                // Then start generating proxies and updating rows
                foreach (var account in importedAccounts)
                {
                    if (!account.TryGetValue("token", out string token) || 
                        !account.TryGetValue("email", out string email))
                    {
                        continue;
                    }
                    
                    try
                    {
                        var proxyGenerator = new GlitchProxyGenerator(token, BaseProxyName);

                        for (int i = 0; i < proxiesPerAccount; i++)
                        {
                            try
                            {
                                var rowIndex = rowIndices[email][i];
                                var row = projectsGrid.Rows[rowIndex];

                                row.Cells["Status"].Value = "Generating...";
                                var domain = await proxyGenerator.CreateProxyProject();
                                
                                row.Cells["ProxyName"].Value = domain;
                                row.Cells["Status"].Value = "Active";
                                row.Cells["Created"].Value = DateTime.Now.ToString("HH:mm:ss");
                            }
                            catch (Exception ex)
                            {
                                var rowIndex = rowIndices[email][i];
                                var row = projectsGrid.Rows[rowIndex];
                                
                                if (ex.Message.Contains("NEW_ACCOUNT_RESOURCE_CREATION_LIMIT"))
                                {
                                    // Mark current row as failed
                                    row.Cells["Status"].Value = "Failed";
                                    row.Cells["ProxyName"].Value = "Resource limit reached";

                                    // Remove all remaining pending proxies for this account
                                    for (int j = i + 1; j < rowIndices[email].Count; j++)
                                    {
                                        var pendingRowIndex = rowIndices[email][j];
                                        projectsGrid.Rows.RemoveAt(pendingRowIndex);
                                    }

                                    // Break the loop for this account
                                    break;
                                }
                                else
                                {
                                    row.Cells["Status"].Value = "Failed";
                                    row.Cells["ProxyName"].Value = "Generation failed";
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error generating proxies for {email}: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                generateButton.Enabled = true;
            }
        }

        private void ImportProxiesButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var lines = File.ReadAllLines(openFileDialog.FileName);
                        
                        projectsGrid.Rows.Clear();
                        foreach (var line in lines)
                        {
                            if (string.IsNullOrWhiteSpace(line)) continue;

                            var domain = line.Trim();
                            var rowIndex = projectsGrid.Rows.Add();
                            var row = projectsGrid.Rows[rowIndex];
                            
                            row.Cells["Account"].Value = "Imported";
                            row.Cells["ProxyName"].Value = domain;
                            row.Cells["Status"].Value = "Unknown";
                            row.Cells["Created"].Value = "-";
                            row.Cells["LastChecked"].Value = "-";
                        }

                        MessageBox.Show($"Imported {lines.Length} proxies successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error importing proxies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportProxiesButton_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                saveFileDialog.FilterIndex = 1;

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var domains = projectsGrid.Rows
                            .Cast<DataGridViewRow>()
                            .Select(row => row.Cells["ProxyName"].Value?.ToString())
                            .Where(domain => !string.IsNullOrWhiteSpace(domain))
                            .ToList();

                        File.WriteAllLines(saveFileDialog.FileName, domains);
                        MessageBox.Show($"Exported {domains.Count} proxies successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting proxies: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ApplyDarkTheme()
        {
            this.BackColor = Color.FromArgb(24, 24, 24);
            
            // Style right panel
            rightPanel.BackColor = Color.FromArgb(28, 28, 28);

            // Style buttons
            foreach (Control control in rightPanel.Controls)
            {
                if (control is Guna.UI2.WinForms.Guna2Button button)
                {
                    button.FillColor = Color.FromArgb(45, 45, 45);
                    button.ForeColor = Color.White;
                    button.Font = new Font("Segoe UI", 9F);
                }
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

        private void ProxiesPerAccountSlider_ValueChanged(object sender, EventArgs e)
        {
            proxiesPerAccount = proxiesPerAccountSlider.Value;
            sliderLabel.Text = $"Proxies per account: {proxiesPerAccount}";
            sliderLabel.Refresh(); // Force refresh of the label
        }

        private void filterFailedButton_Click(object sender, EventArgs e)
        {
            var rowsToRemove = projectsGrid.Rows.Cast<DataGridViewRow>()
                .Where(row => row.Cells["Status"].Value?.ToString() == "Failed")
                .ToList();

            foreach (var row in rowsToRemove)
            {
                projectsGrid.Rows.Remove(row);
            }

            MessageBox.Show($"Removed {rowsToRemove.Count} failed projects", "Filter Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
