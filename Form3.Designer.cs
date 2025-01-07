namespace WsProxyChecker
{
    partial class Form3
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private Guna.UI2.WinForms.Guna2Panel titleBar;
        private Guna.UI2.WinForms.Guna2HtmlLabel titleLabel;
        private Guna.UI2.WinForms.Guna2ControlBox closeButton;
        private Guna.UI2.WinForms.Guna2ControlBox minimizeButton;
        private System.Windows.Forms.Panel mainPanel;
        private Guna.UI2.WinForms.Guna2DataGridView projectsGrid;
        private System.Windows.Forms.Panel rightPanel;
        private Guna.UI2.WinForms.Guna2GroupBox statsGroup;
        private Guna.UI2.WinForms.Guna2TrackBar proxiesPerAccountSlider;
        private Guna.UI2.WinForms.Guna2HtmlLabel sliderLabel;
        private Guna.UI2.WinForms.Guna2Button generateButton;
        private Guna.UI2.WinForms.Guna2Button importAccountsButton;
        private Guna.UI2.WinForms.Guna2Button importProxiesButton;
        private Guna.UI2.WinForms.Guna2Button exportProxiesButton;
        private Guna.UI2.WinForms.Guna2Button filterFailedButton;

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
            // Form settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Proxy Generator";
            this.Size = new Size(1000, 600);
            this.BackColor = Color.FromArgb(24, 24, 24);

            // Title bar
            this.titleBar = new Guna.UI2.WinForms.Guna2Panel();
            this.titleBar.Dock = DockStyle.Top;
            this.titleBar.Height = 30;
            this.titleBar.BackColor = Color.FromArgb(28, 28, 28);

            // Title label
            this.titleLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.titleLabel.Text = "Proxy Generator";
            this.titleLabel.ForeColor = Color.White;
            this.titleLabel.Location = new Point(10, 5);
            this.titleBar.Controls.Add(titleLabel);

            // Control buttons
            this.closeButton = new Guna.UI2.WinForms.Guna2ControlBox();
            this.minimizeButton = new Guna.UI2.WinForms.Guna2ControlBox();
            this.minimizeButton.ControlBoxType = Guna.UI2.WinForms.Enums.ControlBoxType.MinimizeBox;

            // Configure control buttons
            var buttons = new[] { closeButton, minimizeButton };
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].Anchor = AnchorStyles.Top | AnchorStyles.Right;
                buttons[i].FillColor = Color.FromArgb(28, 28, 28);
                buttons[i].IconColor = Color.White;
                buttons[i].Size = new Size(45, 30);
                buttons[i].Location = new Point(Width - (45 * (i + 1)), 0);
                this.titleBar.Controls.Add(buttons[i]);
            }

            // Right panel
            this.rightPanel = new Panel();
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
            this.statsGroup.Height = 100;

            // Slider
            this.proxiesPerAccountSlider = new Guna.UI2.WinForms.Guna2TrackBar();
            this.proxiesPerAccountSlider.Minimum = 1;
            this.proxiesPerAccountSlider.Maximum = 5;
            this.proxiesPerAccountSlider.Value = 1;
            this.proxiesPerAccountSlider.ThumbColor = Color.FromArgb(94, 148, 255);
            this.proxiesPerAccountSlider.Location = new Point(10, statsGroup.Bottom + 20);
            this.proxiesPerAccountSlider.Size = new Size(rightPanel.Width - 20, 20);

            // Slider label
            this.sliderLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.sliderLabel.Text = "Proxies per account: 1";
            this.sliderLabel.ForeColor = Color.White;
            this.sliderLabel.Location = new Point(10, proxiesPerAccountSlider.Bottom + 5);

            // Buttons
            this.generateButton = new Guna.UI2.WinForms.Guna2Button();
            this.importAccountsButton = new Guna.UI2.WinForms.Guna2Button();
            this.importProxiesButton = new Guna.UI2.WinForms.Guna2Button();
            this.exportProxiesButton = new Guna.UI2.WinForms.Guna2Button();
            this.filterFailedButton = new Guna.UI2.WinForms.Guna2Button();

            var actionButtons = new[] { 
                generateButton, 
                importAccountsButton, 
                importProxiesButton, 
                exportProxiesButton,
                filterFailedButton
            };

            string[] buttonTexts = { 
                "Generate Proxies", 
                "Import Accounts", 
                "Import Proxies", 
                "Export Proxies",
                "Remove Failed"
            };

            for (int i = 0; i < actionButtons.Length; i++)
            {
                actionButtons[i].Text = buttonTexts[i];
                actionButtons[i].FillColor = Color.FromArgb(45, 45, 45);
                actionButtons[i].ForeColor = Color.White;
                actionButtons[i].Font = new Font("Segoe UI", 9F);
                actionButtons[i].Size = new Size(rightPanel.Width - 20, 35);
                actionButtons[i].Location = new Point(10, sliderLabel.Bottom + 20 + (i * 45));
                this.rightPanel.Controls.Add(actionButtons[i]);
            }

            // Main panel
            this.mainPanel = new Panel();
            this.mainPanel.Dock = DockStyle.Fill;
            this.mainPanel.Padding = new Padding(10);
            
            // Projects grid
            this.projectsGrid = new Guna.UI2.WinForms.Guna2DataGridView();
            this.projectsGrid.Dock = DockStyle.Fill;
            this.projectsGrid.Theme = Guna.UI2.WinForms.Enums.DataGridViewPresetThemes.Dark;
            this.projectsGrid.AllowUserToAddRows = false;
            this.projectsGrid.AllowUserToResizeRows = false;
            this.projectsGrid.MultiSelect = true;
            this.projectsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.projectsGrid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            this.projectsGrid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            this.projectsGrid.EnableHeadersVisualStyles = false;
            this.projectsGrid.BackgroundColor = Color.FromArgb(32, 32, 32);
            this.projectsGrid.DefaultCellStyle.BackColor = Color.FromArgb(32, 32, 32);
            this.projectsGrid.DefaultCellStyle.ForeColor = Color.White;
            this.projectsGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(45, 45, 45);
            this.projectsGrid.DefaultCellStyle.SelectionForeColor = Color.White;
            this.projectsGrid.GridColor = Color.FromArgb(45, 45, 45);
            this.projectsGrid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(28, 28, 28);
            this.projectsGrid.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            this.projectsGrid.ColumnHeadersHeight = 30;
            this.projectsGrid.RowTemplate.Height = 25;
            this.projectsGrid.RowHeadersVisible = false;

            // Add controls in the correct order
            this.mainPanel.Controls.Add(this.projectsGrid);
            this.rightPanel.Controls.AddRange(new Control[] { 
                statsGroup, 
                proxiesPerAccountSlider, 
                sliderLabel,
                generateButton,
                importAccountsButton,
                importProxiesButton,
                exportProxiesButton,
                filterFailedButton
            });
            this.Controls.Add(this.titleBar);
            this.Controls.Add(this.rightPanel);
            this.Controls.Add(this.mainPanel);
        }

        #endregion
    }
}