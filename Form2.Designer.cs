namespace WsProxyChecker
{
    partial class Form2
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Guna.UI2.WinForms.Guna2Panel titleBar;
        private Guna.UI2.WinForms.Guna2HtmlLabel titleLabel;
        private Guna.UI2.WinForms.Guna2ControlBox closeButton;
        private Guna.UI2.WinForms.Guna2ControlBox minimizeButton;
        private Guna.UI2.WinForms.Guna2DataGridView accountsGrid;
        private Guna.UI2.WinForms.Guna2Panel rightPanel;
        private Guna.UI2.WinForms.Guna2GroupBox statsGroup;
        private Guna.UI2.WinForms.Guna2HtmlLabel totalLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel validLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel invalidLabel;
        private Guna.UI2.WinForms.Guna2Button importButton;
        private Guna.UI2.WinForms.Guna2Button exportButton;
        private Guna.UI2.WinForms.Guna2Button generateButton;
        private Guna.UI2.WinForms.Guna2NumericUpDown accountsAmount;
        private Guna.UI2.WinForms.Guna2CheckBox multithreadingCheckbox;
        private Guna.UI2.WinForms.Guna2TrackBar threadsSlider;
        private Guna.UI2.WinForms.Guna2HtmlLabel threadsLabel;
        private Guna.UI2.WinForms.Guna2Button copyScriptButton;
        private Guna.UI2.WinForms.Guna2TextBox tokenTextBox;
        private Guna.UI2.WinForms.Guna2Button useTokenButton;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.titleBar = new Guna.UI2.WinForms.Guna2Panel();
            this.titleLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.closeButton = new Guna.UI2.WinForms.Guna2ControlBox();
            this.minimizeButton = new Guna.UI2.WinForms.Guna2ControlBox();
            this.accountsGrid = new Guna.UI2.WinForms.Guna2DataGridView();

            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Glitch Accounts";
            this.Size = new Size(800, 800);
            this.BackColor = Color.FromArgb(24, 24, 24);

            // Title bar
            this.titleBar.Dock = DockStyle.Top;
            this.titleBar.Height = 30;
            this.titleBar.BackColor = Color.FromArgb(28, 28, 28);

            // Title label
            this.titleLabel.Text = "Glitch Accounts";
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

            // Accounts grid
            this.accountsGrid.Dock = DockStyle.Fill;
            this.accountsGrid.AllowUserToAddRows = false;
            this.accountsGrid.AllowUserToResizeRows = false;
            this.accountsGrid.MultiSelect = true;
            this.accountsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.accountsGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.accountsGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.accountsGrid.EnableHeadersVisualStyles = false;
            this.accountsGrid.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Dark;

            // Right panel
            this.rightPanel = new Guna.UI2.WinForms.Guna2Panel();
            this.rightPanel.Dock = DockStyle.Right;
            this.rightPanel.Width = 250;
            this.rightPanel.BackColor = Color.FromArgb(28, 28, 28);
            this.rightPanel.Padding = new Padding(10);

            // Stats group
            this.statsGroup = new Guna.UI2.WinForms.Guna2GroupBox();
            this.statsGroup.Text = "Statistics";
            this.statsGroup.ForeColor = Color.White;
            this.statsGroup.Font = new Font("Segoe UI", 9F);
            this.statsGroup.CustomBorderColor = Color.FromArgb(45, 45, 45);
            this.statsGroup.BorderColor = Color.FromArgb(45, 45, 45);
            this.statsGroup.FillColor = Color.FromArgb(32, 32, 32);
            this.statsGroup.Dock = DockStyle.Top;
            this.statsGroup.Height = 120;

            // Stats labels
            this.totalLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.validLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.invalidLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.totalLabel.ForeColor = Color.White;
            this.totalLabel.Font = new Font("Segoe UI", 9F);
            this.totalLabel.AutoSize = false;
            this.totalLabel.Width = statsGroup.Width - 20;
            this.totalLabel.Height = 20;
            this.totalLabel.BackColor = Color.Transparent;
            this.totalLabel.Location = new Point(10, 40);

            this.validLabel.ForeColor = Color.White;
            this.validLabel.Font = new Font("Segoe UI", 9F);
            this.validLabel.AutoSize = false;
            this.validLabel.Width = statsGroup.Width - 20;
            this.validLabel.Height = 20;
            this.validLabel.BackColor = Color.Transparent;
            this.validLabel.Location = new Point(10, 65);

            this.invalidLabel.ForeColor = Color.White;
            this.invalidLabel.Font = new Font("Segoe UI", 9F);
            this.invalidLabel.AutoSize = false;
            this.invalidLabel.Width = statsGroup.Width - 20;
            this.invalidLabel.Height = 20;
            this.invalidLabel.BackColor = Color.Transparent;
            this.invalidLabel.Location = new Point(10, 90);

            statsGroup.Controls.AddRange(new Control[] { totalLabel, validLabel, invalidLabel });

            // Buttons
            this.importButton = new Guna.UI2.WinForms.Guna2Button();
            this.exportButton = new Guna.UI2.WinForms.Guna2Button();
            this.generateButton = new Guna.UI2.WinForms.Guna2Button();

            // Configure importButton
            this.importButton.FillColor = Color.FromArgb(45, 45, 45);
            this.importButton.ForeColor = Color.White;
            this.importButton.Font = new Font("Segoe UI", 9F);
            this.importButton.BorderRadius = 3;
            this.importButton.Width = rightPanel.Width - 20;
            this.importButton.Height = 35;
            this.importButton.Text = "Import Accounts";

            // Configure exportButton
            this.exportButton.FillColor = Color.FromArgb(45, 45, 45);
            this.exportButton.ForeColor = Color.White;
            this.exportButton.Font = new Font("Segoe UI", 9F);
            this.exportButton.BorderRadius = 3;
            this.exportButton.Width = rightPanel.Width - 20;
            this.exportButton.Height = 35;
            this.exportButton.Text = "Export Accounts";

            // Configure generateButton
            this.generateButton.FillColor = Color.FromArgb(45, 45, 45);
            this.generateButton.ForeColor = Color.White;
            this.generateButton.Font = new Font("Segoe UI", 9F);
            this.generateButton.BorderRadius = 3;
            this.generateButton.Width = rightPanel.Width - 20;
            this.generateButton.Height = 35;
            this.generateButton.Text = "Generate Accounts";

            // Amount input
            this.accountsAmount = new Guna.UI2.WinForms.Guna2NumericUpDown();
            this.accountsAmount.FillColor = Color.FromArgb(45, 45, 45);
            this.accountsAmount.ForeColor = Color.White;
            this.accountsAmount.Font = new Font("Segoe UI", 9F);
            this.accountsAmount.Width = rightPanel.Width - 20;
            this.accountsAmount.Minimum = 1;
            this.accountsAmount.Maximum = 100;
            this.accountsAmount.Value = 1;
            this.accountsAmount.BackColor = Color.Transparent;

            // Multithreading controls
            this.multithreadingCheckbox = new Guna.UI2.WinForms.Guna2CheckBox();
            this.multithreadingCheckbox.Text = "Multithreading";
            this.multithreadingCheckbox.ForeColor = Color.White;
            this.multithreadingCheckbox.Font = new Font("Segoe UI", 9F);
            this.multithreadingCheckbox.Checked = true;

            this.threadsSlider = new Guna.UI2.WinForms.Guna2TrackBar();
            this.threadsSlider.Minimum = 1;
            this.threadsSlider.Maximum = 100;
            this.threadsSlider.Value = 10;
            this.threadsSlider.ThumbColor = Color.FromArgb(94, 148, 255);
            this.threadsSlider.Width = rightPanel.Width - 20;
            this.threadsSlider.Enabled = multithreadingCheckbox.Checked;

            this.threadsLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.threadsLabel.Text = "Threads: 10";
            this.threadsLabel.ForeColor = Color.White;
            this.threadsLabel.Font = new Font("Segoe UI", 9F);
            this.threadsLabel.BackColor = Color.Transparent;

            // Layout controls
            this.importButton.Location = new Point(10, statsGroup.Bottom + 20);
            this.exportButton.Location = new Point(10, this.importButton.Bottom + 10);
            this.accountsAmount.Location = new Point(10, this.exportButton.Bottom + 20);
            this.generateButton.Location = new Point(10, this.accountsAmount.Bottom + 10);
            this.multithreadingCheckbox.Location = new Point(10, this.generateButton.Bottom + 20);
            this.threadsSlider.Location = new Point(10, this.multithreadingCheckbox.Bottom + 10);
            this.threadsLabel.Location = new Point(10, this.threadsSlider.Bottom + 5);

            // Create a separator panel for visual grouping
            var separatorPanel = new Guna.UI2.WinForms.Guna2Panel
            {
                Height = 1,
                Dock = DockStyle.None,
                BackColor = Color.FromArgb(45, 45, 45),
                Width = rightPanel.Width - 20,
                Location = new Point(10, this.threadsLabel.Bottom + 20)
            };

            // Update token-related controls
            this.copyScriptButton = new Guna.UI2.WinForms.Guna2Button();
            this.tokenTextBox = new Guna.UI2.WinForms.Guna2TextBox();
            this.useTokenButton = new Guna.UI2.WinForms.Guna2Button();

            // Configure token-related controls
            this.copyScriptButton.FillColor = Color.FromArgb(45, 45, 45);
            this.copyScriptButton.ForeColor = Color.White;
            this.copyScriptButton.Font = new Font("Segoe UI", 9F);
            this.copyScriptButton.BorderRadius = 3;
            this.copyScriptButton.Width = rightPanel.Width - 20;
            this.copyScriptButton.Height = 35;
            this.copyScriptButton.Text = "Copy Captcha Script";
            this.copyScriptButton.Location = new Point(10, separatorPanel.Bottom + 20);

            this.tokenTextBox.FillColor = Color.FromArgb(45, 45, 45);
            this.tokenTextBox.ForeColor = Color.White;
            this.tokenTextBox.Font = new Font("Segoe UI", 9F);
            this.tokenTextBox.BorderRadius = 3;
            this.tokenTextBox.Width = rightPanel.Width - 20;
            this.tokenTextBox.Height = 35;
            this.tokenTextBox.PlaceholderText = "Paste intercepted token here";
            this.tokenTextBox.PlaceholderForeColor = Color.Gray;
            this.tokenTextBox.BorderColor = Color.FromArgb(45, 45, 45);
            this.tokenTextBox.Location = new Point(10, this.copyScriptButton.Bottom + 10);

            this.useTokenButton.FillColor = Color.FromArgb(45, 45, 45);
            this.useTokenButton.ForeColor = Color.White;
            this.useTokenButton.Font = new Font("Segoe UI", 9F);
            this.useTokenButton.BorderRadius = 3;
            this.useTokenButton.Width = rightPanel.Width - 20;
            this.useTokenButton.Height = 35;
            this.useTokenButton.Text = "Create Accounts with Token";
            this.useTokenButton.Location = new Point(10, this.tokenTextBox.Bottom + 10);

            // Add a label above the token controls
            var tokenSectionLabel = new Guna.UI2.WinForms.Guna2HtmlLabel
            {
                Text = "Manual Token Input",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                AutoSize = false,
                Width = rightPanel.Width - 20,
                Height = 20,
                Location = new Point(10, separatorPanel.Bottom + 5)
            };

            // Add controls to right panel in the correct order
            this.rightPanel.Controls.AddRange(new Control[] {
                this.statsGroup,
                this.importButton,
                this.exportButton,
                this.accountsAmount,
                this.generateButton,
                this.multithreadingCheckbox,
                this.threadsSlider,
                this.threadsLabel,
                separatorPanel,
                tokenSectionLabel,
                this.copyScriptButton,
                this.tokenTextBox,
                this.useTokenButton
            });

            // Update the grid to fill the remaining space
            this.accountsGrid.Dock = DockStyle.Fill;
            this.accountsGrid.Location = new Point(0, this.titleBar.Bottom);
            this.accountsGrid.Size = new Size(Width - this.rightPanel.Width, Height - this.titleBar.Height);

            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.accountsGrid);
            this.Controls.Add(this.titleBar);
        }

        #endregion
    }
}