namespace WsProxyChecker
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                cancellationTokenSource?.Cancel();
                cancellationTokenSource?.Dispose();
                semaphore?.Dispose();
                if (components != null)
            {
                components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.proxyGrid = new Guna.UI2.WinForms.Guna2DataGridView();
            this.titleBar = new Guna.UI2.WinForms.Guna2Panel();
            this.titleLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.closeButton = new Guna.UI2.WinForms.Guna2ControlBox();
            this.minimizeButton = new Guna.UI2.WinForms.Guna2ControlBox();

            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "WebSocket Proxy Checker";
            this.Size = new Size(800, 500);

            // Title bar
            this.titleBar.Dock = DockStyle.Top;
            this.titleBar.Height = 30;
            this.titleBar.BackColor = Color.FromArgb(28, 28, 28);

            // Title label
            this.titleLabel.Text = "WebSocket Proxy Checker";
            this.titleLabel.ForeColor = Color.White;
            this.titleLabel.Location = new Point(10, 5);
            this.titleBar.Controls.Add(titleLabel);

            // Control buttons
            this.closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.closeButton.FillColor = Color.FromArgb(28, 28, 28);
            this.closeButton.IconColor = Color.White;
            this.closeButton.Location = new Point(Width - 45, 0);
            this.closeButton.Size = new Size(45, 30);

            this.minimizeButton.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;
            this.minimizeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            this.minimizeButton.FillColor = Color.FromArgb(28, 28, 28);
            this.minimizeButton.IconColor = Color.White;
            this.minimizeButton.Location = new Point(Width - 90, 0);
            this.minimizeButton.Size = new Size(45, 30);

            this.titleBar.Controls.Add(closeButton);
            this.titleBar.Controls.Add(minimizeButton);

            // DataGridView
            this.proxyGrid.Dock = DockStyle.Fill;
            this.proxyGrid.AllowUserToAddRows = false;
            this.proxyGrid.AllowUserToResizeRows = false;
            this.proxyGrid.MultiSelect = true;
            this.proxyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.proxyGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.proxyGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.proxyGrid.EnableHeadersVisualStyles = false;
            this.proxyGrid.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Dark;

            // Add controls to form
            this.Controls.Add(proxyGrid);
            this.Controls.Add(titleBar);

            // Initialize right panel
            this.rightPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.rightPanel.Dock = DockStyle.Right;
            this.rightPanel.Width = 200;
            this.rightPanel.BackColor = Color.FromArgb(28, 28, 28);
            this.rightPanel.Padding = new Padding(10);

            // Initialize stats group
            this.statsGroup = new Guna.UI2.WinForms.Guna2GroupBox();
            this.statsGroup.Text = "Statistics";
            this.statsGroup.ForeColor = Color.White;
            this.statsGroup.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            this.statsGroup.CustomBorderColor = Color.FromArgb(45, 45, 45);
            this.statsGroup.BorderColor = Color.FromArgb(45, 45, 45);
            this.statsGroup.FillColor = Color.FromArgb(32, 32, 32);
            this.statsGroup.Dock = DockStyle.Top;
            this.statsGroup.Height = 120;

            // Initialize stats labels
            this.totalLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.validLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.invalidLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();

            // Total label
            this.totalLabel.ForeColor = Color.White;
            this.totalLabel.Font = new Font("Segoe UI", 9F);
            this.totalLabel.AutoSize = false;
            this.totalLabel.Width = statsGroup.Width - 20;
            this.totalLabel.Height = 20;
            this.totalLabel.Location = new Point(10, 40);
            this.totalLabel.BackColor = Color.Transparent;

            // Valid label
            this.validLabel.ForeColor = Color.White;
            this.validLabel.Font = new Font("Segoe UI", 9F);
            this.validLabel.AutoSize = false;
            this.validLabel.Width = statsGroup.Width - 20;
            this.validLabel.Height = 20;
            this.validLabel.Location = new Point(10, 65);
            this.validLabel.BackColor = Color.Transparent;

            // Invalid label
            this.invalidLabel.ForeColor = Color.White;
            this.invalidLabel.Font = new Font("Segoe UI", 9F);
            this.invalidLabel.AutoSize = false;
            this.invalidLabel.Width = statsGroup.Width - 20;
            this.invalidLabel.Height = 20;
            this.invalidLabel.Location = new Point(10, 90);
            this.invalidLabel.BackColor = Color.Transparent;

            // Add labels to stats group
            this.statsGroup.Controls.Add(this.totalLabel);
            this.statsGroup.Controls.Add(this.validLabel);
            this.statsGroup.Controls.Add(this.invalidLabel);

            // Initialize buttons
            this.importButton = new Guna.UI2.WinForms.Guna2Button();
            this.exportButton = new Guna.UI2.WinForms.Guna2Button();
            this.checkAllButton = new Guna.UI2.WinForms.Guna2Button();
            this.removeInvalidButton = new Guna.UI2.WinForms.Guna2Button();

            // Import button
            this.importButton.FillColor = Color.FromArgb(45, 45, 45);
            this.importButton.ForeColor = Color.White;
            this.importButton.Font = new Font("Segoe UI", 9F);
            this.importButton.BorderRadius = 3;
            this.importButton.Width = rightPanel.Width - 20;
            this.importButton.Height = 35;
            this.importButton.Text = "Import Proxies";
            this.importButton.Location = new Point(10, statsGroup.Bottom + 20);

            // Export button
            this.exportButton.FillColor = Color.FromArgb(45, 45, 45);
            this.exportButton.ForeColor = Color.White;
            this.exportButton.Font = new Font("Segoe UI", 9F);
            this.exportButton.BorderRadius = 3;
            this.exportButton.Width = rightPanel.Width - 20;
            this.exportButton.Height = 35;
            this.exportButton.Text = "Export Valid";
            this.exportButton.Location = new Point(10, importButton.Bottom + 10);

            // Check All button
            this.checkAllButton.FillColor = Color.FromArgb(45, 45, 45);
            this.checkAllButton.ForeColor = Color.White;
            this.checkAllButton.Font = new Font("Segoe UI", 9F);
            this.checkAllButton.BorderRadius = 3;
            this.checkAllButton.Width = rightPanel.Width - 20;
            this.checkAllButton.Height = 35;
            this.checkAllButton.Text = "Check All";
            this.checkAllButton.Location = new Point(10, exportButton.Bottom + 10);

            // Remove Invalid button
            this.removeInvalidButton.FillColor = Color.FromArgb(45, 45, 45);
            this.removeInvalidButton.ForeColor = Color.White;
            this.removeInvalidButton.Font = new Font("Segoe UI", 9F);
            this.removeInvalidButton.BorderRadius = 3;
            this.removeInvalidButton.Width = rightPanel.Width - 20;
            this.removeInvalidButton.Height = 35;
            this.removeInvalidButton.Text = "Remove Invalid";
            this.removeInvalidButton.Location = new Point(10, checkAllButton.Bottom + 10);

            // Multithreading controls
            this.multithreadingCheckbox = new Guna.UI2.WinForms.Guna2CheckBox();
            this.multithreadingCheckbox.Text = "Multithreading";
            this.multithreadingCheckbox.ForeColor = Color.White;
            this.multithreadingCheckbox.Font = new Font("Segoe UI", 9F);
            this.multithreadingCheckbox.Location = new Point(10, removeInvalidButton.Bottom + 20);
            this.multithreadingCheckbox.Checked = true;

            this.threadsSlider = new Guna.UI2.WinForms.Guna2TrackBar();
            this.threadsSlider.Minimum = 1;
            this.threadsSlider.Maximum = 100;
            this.threadsSlider.Value = 10;
            this.threadsSlider.ThumbColor = Color.FromArgb(94, 148, 255);
            this.threadsSlider.Location = new Point(10, multithreadingCheckbox.Bottom + 10);
            this.threadsSlider.Width = rightPanel.Width - 20;
            this.threadsSlider.Enabled = multithreadingCheckbox.Checked;

            this.threadsLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.threadsLabel.Text = "Threads: 10";
            this.threadsLabel.ForeColor = Color.White;
            this.threadsLabel.Font = new Font("Segoe UI", 9F);
            this.threadsLabel.Location = new Point(10, threadsSlider.Bottom + 5);
            this.threadsLabel.BackColor = Color.Transparent;

            // Add event handlers in Form1.cs
            this.multithreadingCheckbox.CheckedChanged += MultithreadingCheckbox_CheckedChanged;
            this.threadsSlider.ValueChanged += ThreadsSlider_ValueChanged;

            // Add buttons to right panel
            this.rightPanel.Controls.Add(this.importButton);
            this.rightPanel.Controls.Add(this.exportButton);
            this.rightPanel.Controls.Add(this.checkAllButton);
            this.rightPanel.Controls.Add(this.removeInvalidButton);
            this.rightPanel.Controls.Add(this.multithreadingCheckbox);
            this.rightPanel.Controls.Add(this.threadsSlider);
            this.rightPanel.Controls.Add(this.threadsLabel);

            rightPanel.Controls.Add(statsGroup);

            // Bottom panel
            this.bottomPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.bottomPanel.Dock = DockStyle.Bottom;
            this.bottomPanel.Height = 50;
            this.bottomPanel.BackColor = Color.FromArgb(28, 28, 28);
            this.bottomPanel.Padding = new Padding(10);

            // Navigation buttons
            this.accountsButton = new Guna.UI2.WinForms.Guna2Button();
            this.projectsButton = new Guna.UI2.WinForms.Guna2Button();

            // Accounts button
            this.accountsButton.FillColor = Color.FromArgb(45, 45, 45);
            this.accountsButton.ForeColor = Color.White;
            this.accountsButton.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            this.accountsButton.BorderRadius = 3;
            this.accountsButton.Height = 30;
            this.accountsButton.Width = (bottomPanel.Width - 30) / 2; // Adjusted for 2 buttons
            this.accountsButton.Text = "Accounts";
            this.accountsButton.Location = new Point(10, 10);

            // Projects button
            this.projectsButton.FillColor = Color.FromArgb(45, 45, 45);
            this.projectsButton.ForeColor = Color.White;
            this.projectsButton.Font = new Font("Segoe UI", 11F, FontStyle.Regular);
            this.projectsButton.BorderRadius = 3;
            this.projectsButton.Height = 30;
            this.projectsButton.Width = (bottomPanel.Width - 30) / 2; // Adjusted for 2 buttons
            this.projectsButton.Text = "Projects";
            this.projectsButton.Location = new Point(accountsButton.Right + 10, 10);

            bottomPanel.Controls.Add(accountsButton);
            bottomPanel.Controls.Add(projectsButton);

            // Add bottom panel to right panel
            rightPanel.Controls.Add(bottomPanel);

            // Add right panel to form
            this.Controls.Add(rightPanel);

            // Adjust grid to make room for right panel
            this.proxyGrid.Dock = DockStyle.Fill;
        }

        #endregion

        private Guna.UI2.WinForms.Guna2DataGridView proxyGrid;
        private Guna.UI2.WinForms.Guna2Panel titleBar;
        private Guna.UI2.WinForms.Guna2HtmlLabel titleLabel;
        private Guna.UI2.WinForms.Guna2ControlBox closeButton;
        private Guna.UI2.WinForms.Guna2ControlBox minimizeButton;
        private Guna.UI2.WinForms.Guna2Panel rightPanel;
        private Guna.UI2.WinForms.Guna2GroupBox statsGroup;
        private Guna.UI2.WinForms.Guna2HtmlLabel totalLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel validLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel invalidLabel;
        private Guna.UI2.WinForms.Guna2Button importButton;
        private Guna.UI2.WinForms.Guna2Button exportButton;
        private Guna.UI2.WinForms.Guna2Button checkAllButton;
        private Guna.UI2.WinForms.Guna2Button removeInvalidButton;
        private Guna.UI2.WinForms.Guna2CheckBox multithreadingCheckbox;
        private Guna.UI2.WinForms.Guna2TrackBar threadsSlider;
        private Guna.UI2.WinForms.Guna2HtmlLabel threadsLabel;
        private Guna.UI2.WinForms.Guna2Panel bottomPanel;
        private Guna.UI2.WinForms.Guna2Button accountsButton;
        private Guna.UI2.WinForms.Guna2Button projectsButton;
    }
}
