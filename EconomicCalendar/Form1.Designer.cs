namespace EconomicCalendar
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        // ---------- Sidebar ----------
        private Panel pnlSidebar;
        private Label lblLogo;
        private Label lblLogoSub;
        private Label lblClockTitle;
        private Label lblUtcClock;
        private Label lblBrokerClock;
        private Panel pnlPipeStatus;
        private Label lblPipeStatus;
        private Panel pnlEngineStatus;
        private Label lblEngineStatus;

        // ---------- Main content host ----------
        private TableLayoutPanel tlpRoot;
        private TableLayoutPanel tlpMain;

        // ---------- Card 1: Calendar ----------
        private Panel cardCalendar;
        private Label lblCalendarTitle;
        private Button btnRefreshCalendar;
        private DataGridView dgvCalendar;

        // ---------- Card 2: News Trigger Settings ----------
        private Panel cardSettings;
        private Label lblSettingsTitle;
        private Label lblSymbol;
        private TextBox txtSymbol;
        private Label lblOffset;
        private NumericUpDown numOffsetSeconds;
        private Label lblDistance;
        private NumericUpDown numDistancePips;
        private Label lblLot;
        private NumericUpDown numLotSize;
        private Label lblSL;
        private NumericUpDown numSL;
        private Label lblTP;
        private NumericUpDown numTP;
        private CheckBox chkOCO;
        private Button btnForceStraddle;

        // ---------- Card 3: Countdown ----------
        private Panel cardCountdown;
        private Label lblCountdownTitle;
        private Label lblCountdownTarget;
        private Label lblCountdownValue;

        // ---------- Log ----------
        private Panel cardLog;
        private Label lblLogTitle;
        private RichTextBox rtbLog;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // Palette
            var bgDeep = System.Drawing.ColorTranslator.FromHtml("#121622");
            var bgCard = System.Drawing.ColorTranslator.FromHtml("#1A1F2C");
            var neonTeal = System.Drawing.ColorTranslator.FromHtml("#00E6C3");
            var neonRed = System.Drawing.ColorTranslator.FromHtml("#FF3B30");
            var textLight = System.Drawing.ColorTranslator.FromHtml("#E6EAF2");
            var textDim = System.Drawing.ColorTranslator.FromHtml("#8892A6");
            var bgInput = System.Drawing.ColorTranslator.FromHtml("#0E1320");

            // ============ Form ============
            this.SuspendLayout();
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = bgDeep;
            this.ClientSize = new System.Drawing.Size(1500, 900);
            this.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.ForeColor = textLight;
            this.Name = "Form1";
            this.Text = "AUTOSCRIPTS NEWS - News Trader & Economic Calendar Automation";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(1280, 800);

            // ============ Root layout (sidebar | main) ============
            this.tlpRoot = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = bgDeep
            };
            this.tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 260));
            this.tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            this.tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            // ============ Sidebar ============
            this.pnlSidebar = new Panel { Dock = DockStyle.Fill, BackColor = bgCard, Padding = new Padding(18) };

            this.lblLogo = new Label
            {
                Text = "AUTOSCRIPTS",
                Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold),
                ForeColor = neonTeal,
                AutoSize = false,
                Height = 32,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.lblLogoSub = new Label
            {
                Text = "NEWS TRADER",
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular),
                ForeColor = textDim,
                AutoSize = false,
                Height = 24,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            this.lblClockTitle = new Label
            {
                Text = "PRECISION CLOCK",
                Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold),
                ForeColor = textDim,
                AutoSize = false,
                Height = 22,
                Dock = DockStyle.Top,
                Padding = new Padding(0, 18, 0, 4),
                TextAlign = System.Drawing.ContentAlignment.BottomLeft
            };
            this.lblUtcClock = new Label
            {
                Text = "UTC --:--:--.---",
                Font = new System.Drawing.Font("Consolas", 14F, System.Drawing.FontStyle.Bold),
                ForeColor = textLight,
                AutoSize = false,
                Height = 32,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.lblBrokerClock = new Label
            {
                Text = "LOCAL --:--:--",
                Font = new System.Drawing.Font("Consolas", 11F, System.Drawing.FontStyle.Regular),
                ForeColor = textDim,
                AutoSize = false,
                Height = 26,
                Dock = DockStyle.Top,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            this.pnlPipeStatus = new Panel
            {
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#0E1320"),
                Margin = new Padding(0, 24, 0, 6),
                Padding = new Padding(10, 0, 10, 0)
            };
            this.lblPipeStatus = new Label
            {
                Text = "● PIPE OFFLINE",
                Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold),
                ForeColor = System.Drawing.ColorTranslator.FromHtml("#FF3B30"),
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.pnlPipeStatus.Controls.Add(this.lblPipeStatus);

            this.pnlEngineStatus = new Panel
            {
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#0E1320"),
                Margin = new Padding(0, 6, 0, 6),
                Padding = new Padding(10, 0, 10, 0)
            };
            this.lblEngineStatus = new Label
            {
                Text = "● EXECUTION ENGINE READY",
                Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold),
                ForeColor = neonTeal,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.pnlEngineStatus.Controls.Add(this.lblEngineStatus);

            // Order matters for DockStyle.Top (reverse stacking)
            this.pnlSidebar.Controls.Add(this.pnlEngineStatus);
            this.pnlSidebar.Controls.Add(this.pnlPipeStatus);
            this.pnlSidebar.Controls.Add(this.lblBrokerClock);
            this.pnlSidebar.Controls.Add(this.lblUtcClock);
            this.pnlSidebar.Controls.Add(this.lblClockTitle);
            this.pnlSidebar.Controls.Add(this.lblLogoSub);
            this.pnlSidebar.Controls.Add(this.lblLogo);

            this.tlpRoot.Controls.Add(this.pnlSidebar, 0, 0);

            // ============ Main area ============
            this.tlpMain = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = bgDeep,
                Padding = new Padding(14)
            };
            this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
            this.tlpMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
            this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 62F));
            this.tlpMain.RowStyles.Add(new RowStyle(SizeType.Percent, 38F));

            // -------- Card 1: Calendar (top-left) --------
            this.cardCalendar = new Panel { Dock = DockStyle.Fill, BackColor = bgCard, Padding = new Padding(14), Margin = new Padding(6) };
            this.lblCalendarTitle = new Label
            {
                Text = "ECONOMIC CALENDAR",
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                ForeColor = textLight,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.btnRefreshCalendar = new Button
            {
                Text = "REFRESH FEED",
                Font = new System.Drawing.Font("Segoe UI", 8.75F, System.Drawing.FontStyle.Bold),
                BackColor = System.Drawing.ColorTranslator.FromHtml("#243049"),
                ForeColor = neonTeal,
                FlatStyle = FlatStyle.Flat,
                Dock = DockStyle.Top,
                Height = 32,
                Cursor = Cursors.Hand
            };
            this.btnRefreshCalendar.FlatAppearance.BorderColor = System.Drawing.ColorTranslator.FromHtml("#2A3550");
            this.btnRefreshCalendar.FlatAppearance.BorderSize = 1;
            this.btnRefreshCalendar.Click += new EventHandler(this.btnRefreshCalendar_Click);

            this.dgvCalendar = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = bgCard,
                BorderStyle = BorderStyle.None,
                GridColor = System.Drawing.ColorTranslator.FromHtml("#262E44"),
                EnableHeadersVisualStyles = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                ReadOnly = false,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = 32,
                RowTemplate = { Height = 26 },
                Font = new System.Drawing.Font("Segoe UI", 9F)
            };
            this.dgvCalendar.DefaultCellStyle.BackColor = bgCard;
            this.dgvCalendar.DefaultCellStyle.ForeColor = textLight;
            this.dgvCalendar.DefaultCellStyle.SelectionBackColor = System.Drawing.ColorTranslator.FromHtml("#243049");
            this.dgvCalendar.DefaultCellStyle.SelectionForeColor = textLight;
            this.dgvCalendar.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#10141F");
            this.dgvCalendar.ColumnHeadersDefaultCellStyle.ForeColor = textDim;
            this.dgvCalendar.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.dgvCalendar.AlternatingRowsDefaultCellStyle.BackColor = System.Drawing.ColorTranslator.FromHtml("#1E2434");

            this.cardCalendar.Controls.Add(this.dgvCalendar);
            this.cardCalendar.Controls.Add(this.btnRefreshCalendar);
            this.cardCalendar.Controls.Add(this.lblCalendarTitle);

            // -------- Card 2: Settings (top-right) --------
            this.cardSettings = new Panel { Dock = DockStyle.Fill, BackColor = bgCard, Padding = new Padding(14), Margin = new Padding(6) };
            this.lblSettingsTitle = new Label
            {
                Text = "NEWS TRIGGER SETTINGS",
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                ForeColor = textLight,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            var tlpSettings = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 8,
                BackColor = bgCard,
                Padding = new Padding(0, 8, 0, 0)
            };
            tlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
            tlpSettings.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
            for (int i = 0; i < 7; i++) tlpSettings.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tlpSettings.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            this.lblSymbol = MakeFieldLabel("Symbol", textDim);
            this.txtSymbol = new TextBox { Dock = DockStyle.Fill, BackColor = bgInput, ForeColor = textLight, BorderStyle = BorderStyle.FixedSingle, Text = "EURUSD", Font = new System.Drawing.Font("Consolas", 10F) };

            this.lblOffset = MakeFieldLabel("Offset (sec before)", textDim);
            this.numOffsetSeconds = MakeNum(0, 600, 5, bgInput, textLight);

            this.lblDistance = MakeFieldLabel("Breakout Distance (pts)", textDim);
            this.numDistancePips = MakeNum(0, 5000, 150, bgInput, textLight);

            this.lblLot = MakeFieldLabel("Lot Size", textDim);
            this.numLotSize = MakeNum(0.01M, 100M, 1.0M, bgInput, textLight, 2, 0.01M);

            this.lblSL = MakeFieldLabel("Stop Loss (pts)", textDim);
            this.numSL = MakeNum(0, 100000, 200, bgInput, textLight);

            this.lblTP = MakeFieldLabel("Take Profit (pts)", textDim);
            this.numTP = MakeNum(0, 100000, 400, bgInput, textLight);

            this.chkOCO = new CheckBox
            {
                Text = "OCO (One-Cancels-Other) Enabled",
                Dock = DockStyle.Fill,
                ForeColor = textLight,
                BackColor = bgCard,
                Checked = true,
                Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold),
                FlatStyle = FlatStyle.Flat
            };

            this.btnForceStraddle = new Button
            {
                Text = "⚡ FORCE INSTANT STRADDLE",
                Dock = DockStyle.Fill,
                BackColor = neonTeal,
                ForeColor = System.Drawing.Color.Black,
                FlatStyle = FlatStyle.Flat,
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                Cursor = Cursors.Hand,
                Height = 48
            };
            this.btnForceStraddle.FlatAppearance.BorderSize = 0;
            this.btnForceStraddle.Click += new EventHandler(this.btnForceStraddle_Click);

            tlpSettings.Controls.Add(this.lblSymbol, 0, 0); tlpSettings.Controls.Add(this.txtSymbol, 1, 0);
            tlpSettings.Controls.Add(this.lblOffset, 0, 1); tlpSettings.Controls.Add(this.numOffsetSeconds, 1, 1);
            tlpSettings.Controls.Add(this.lblDistance, 0, 2); tlpSettings.Controls.Add(this.numDistancePips, 1, 2);
            tlpSettings.Controls.Add(this.lblLot, 0, 3); tlpSettings.Controls.Add(this.numLotSize, 1, 3);
            tlpSettings.Controls.Add(this.lblSL, 0, 4); tlpSettings.Controls.Add(this.numSL, 1, 4);
            tlpSettings.Controls.Add(this.lblTP, 0, 5); tlpSettings.Controls.Add(this.numTP, 1, 5);
            tlpSettings.Controls.Add(this.chkOCO, 0, 6); tlpSettings.SetColumnSpan(this.chkOCO, 2);
            tlpSettings.Controls.Add(this.btnForceStraddle, 0, 7); tlpSettings.SetColumnSpan(this.btnForceStraddle, 2);

            this.cardSettings.Controls.Add(tlpSettings);
            this.cardSettings.Controls.Add(this.lblSettingsTitle);

            // -------- Card 3: Countdown (bottom-left) --------
            this.cardCountdown = new Panel { Dock = DockStyle.Fill, BackColor = bgCard, Padding = new Padding(14), Margin = new Padding(6) };
            this.lblCountdownTitle = new Label
            {
                Text = "LIVE COUNTDOWN CONSOLE",
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                ForeColor = textLight,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.lblCountdownTarget = new Label
            {
                Text = "No high-impact event selected",
                Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular),
                ForeColor = textDim,
                Dock = DockStyle.Top,
                Height = 28,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.lblCountdownValue = new Label
            {
                Text = "--:--:--.---",
                Font = new System.Drawing.Font("Consolas", 64F, System.Drawing.FontStyle.Bold),
                ForeColor = neonTeal,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleCenter
            };

            this.cardCountdown.Controls.Add(this.lblCountdownValue);
            this.cardCountdown.Controls.Add(this.lblCountdownTarget);
            this.cardCountdown.Controls.Add(this.lblCountdownTitle);

            // -------- Card 4: Log (bottom-right) --------
            this.cardLog = new Panel { Dock = DockStyle.Fill, BackColor = bgCard, Padding = new Padding(14), Margin = new Padding(6) };
            this.lblLogTitle = new Label
            {
                Text = "DIAGNOSTICS / LATENCY LOG",
                Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold),
                ForeColor = textLight,
                Dock = DockStyle.Top,
                Height = 30,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
            this.rtbLog = new RichTextBox
            {
                Dock = DockStyle.Fill,
                BackColor = System.Drawing.ColorTranslator.FromHtml("#0B0F1A"),
                ForeColor = textLight,
                BorderStyle = BorderStyle.None,
                Font = new System.Drawing.Font("Consolas", 9F),
                ReadOnly = true,
                DetectUrls = false,
                HideSelection = true
            };
            this.cardLog.Controls.Add(this.rtbLog);
            this.cardLog.Controls.Add(this.lblLogTitle);

            // -------- Place cards --------
            this.tlpMain.Controls.Add(this.cardCalendar, 0, 0);
            this.tlpMain.Controls.Add(this.cardSettings, 1, 0);
            this.tlpMain.Controls.Add(this.cardCountdown, 0, 1);
            this.tlpMain.Controls.Add(this.cardLog, 1, 1);

            this.tlpRoot.Controls.Add(this.tlpMain, 1, 0);

            this.Controls.Add(this.tlpRoot);
            this.ResumeLayout(false);
        }

        private static Label MakeFieldLabel(string text, System.Drawing.Color color)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                ForeColor = color,
                Font = new System.Drawing.Font("Segoe UI", 9F),
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };
        }

        private static NumericUpDown MakeNum(decimal min, decimal max, decimal val, System.Drawing.Color bg, System.Drawing.Color fg, int decimals = 0, decimal increment = 1M)
        {
            return new NumericUpDown
            {
                Dock = DockStyle.Fill,
                Minimum = min,
                Maximum = max,
                Value = val,
                DecimalPlaces = decimals,
                Increment = increment,
                BackColor = bg,
                ForeColor = fg,
                BorderStyle = BorderStyle.FixedSingle,
                Font = new System.Drawing.Font("Consolas", 10F)
            };
        }
    }
}
