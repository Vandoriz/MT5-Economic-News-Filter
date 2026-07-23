using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EconomicCalendar
{
    /// <summary>
    /// Strongly-typed economic event row.
    /// </summary>
    public class EconomicEvent
    {
        public DateTime TimeUtc { get; set; }
        public string Currency { get; set; } = "";
        public string EventName { get; set; } = "";
        public string Impact { get; set; } = "Low";   // High / Medium / Low
        public string Previous { get; set; } = "";
        public string Forecast { get; set; } = "";
        public string Actual { get; set; } = "";

        // Internal state
        public bool Selected { get; set; }
        public bool TriggerFired { get; set; }

        public override string ToString() =>
            $"{TimeUtc:HH:mm:ss} {Currency} {EventName} [{Impact}]";
    }

    public partial class Form1 : Form
    {
        // ---------- Constants ----------
        private const string PipeName = "AUTOSCRIPTS_NEWS_TRADER";
        private static readonly Color NeonTeal  = ColorTranslator.FromHtml("#00E6C3");
        private static readonly Color NeonRed   = ColorTranslator.FromHtml("#FF3B30");
        private static readonly Color WarnAmber = ColorTranslator.FromHtml("#FFB020");
        private static readonly Color TextLight = ColorTranslator.FromHtml("#E6EAF2");
        private static readonly Color TextDim   = ColorTranslator.FromHtml("#8892A6");

        // ---------- State ----------
        private readonly List<EconomicEvent> _events = new();
        private readonly object _eventsLock = new();
        private readonly CancellationTokenSource _cts = new();

        private NamedPipeServerStream? _pipeServer;
        private StreamWriter? _pipeWriter;
        private readonly SemaphoreSlim _pipeWriteLock = new(1, 1);
        private volatile bool _pipeConnected;

        private Task? _pipeServerTask;
        private Task? _countdownTask;
        private Task? _clockTask;

        public Form1()
        {
            InitializeComponent();

            this.Load            += async (s, e) => await OnLoadAsync();
            this.FormClosing     += OnClosingAsync;
            this.dgvCalendar.CellValueChanged   += DgvCalendar_CellValueChanged;
            this.dgvCalendar.CurrentCellDirtyStateChanged += (s, e) =>
            {
                if (dgvCalendar.IsCurrentCellDirty)
                    dgvCalendar.CommitEdit(DataGridViewDataErrorContexts.Commit);
            };
            this.dgvCalendar.CellFormatting     += DgvCalendar_CellFormatting;
        }

        // =================================================================
        // Lifecycle
        // =================================================================
        private async Task OnLoadAsync()
        {
            BuildCalendarColumns();
            Log("Application initialized.", NeonTeal);
            Log($"Pipe endpoint: \\\\.\\pipe\\{PipeName}", TextDim);

            // Start background loops
            _clockTask      = Task.Run(() => ClockLoopAsync(_cts.Token));
            _pipeServerTask = Task.Run(() => PipeServerLoopAsync(_cts.Token));
            _countdownTask  = Task.Run(() => CountdownLoopAsync(_cts.Token));

            // Initial calendar fetch
            await RefreshCalendarAsync();
        }

        private async void OnClosingAsync(object? sender, FormClosingEventArgs e)
        {
            try
            {
                _cts.Cancel();
                try { _pipeServer?.Disconnect(); } catch { }
                try { _pipeServer?.Dispose(); } catch { }
                await Task.WhenAny(
                    Task.WhenAll(
                        _clockTask      ?? Task.CompletedTask,
                        _pipeServerTask ?? Task.CompletedTask,
                        _countdownTask  ?? Task.CompletedTask),
                    Task.Delay(1500));
            }
            catch { /* ignore on shutdown */ }
        }

        // =================================================================
        // UI Marshaling
        // =================================================================
        private void UiInvoke(Action a)
        {
            if (IsDisposed || Disposing) return;
            if (!IsHandleCreated) return;
            try
            {
                if (InvokeRequired) BeginInvoke(a);
                else a();
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        private void Log(string message, Color? color = null)
        {
            var stamp = DateTime.UtcNow.ToString("HH:mm:ss.fff");
            var line  = $"[{stamp}] {message}{Environment.NewLine}";
            UiInvoke(() =>
            {
                rtbLog.SelectionStart  = rtbLog.TextLength;
                rtbLog.SelectionLength = 0;
                rtbLog.SelectionColor  = color ?? TextLight;
                rtbLog.AppendText(line);
                rtbLog.SelectionColor  = TextLight;
                rtbLog.ScrollToCaret();
            });
        }

        // =================================================================
        // Calendar grid
        // =================================================================
        private void BuildCalendarColumns()
        {
            dgvCalendar.Columns.Clear();

            var colSel = new DataGridViewCheckBoxColumn
            {
                HeaderText  = "TRACK",
                Name        = "Selected",
                Width       = 60,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                FillWeight  = 8
            };
            dgvCalendar.Columns.Add(colSel);

            dgvCalendar.Columns.Add(MakeCol("TimeUtc",   "TIME (UTC)", true,  fill: 15));
            dgvCalendar.Columns.Add(MakeCol("Currency",  "CCY",        true,  fill: 8));
            dgvCalendar.Columns.Add(MakeCol("EventName", "EVENT",      true,  fill: 35));
            dgvCalendar.Columns.Add(MakeCol("Impact",    "IMPACT",     true,  fill: 12));
            dgvCalendar.Columns.Add(MakeCol("Previous",  "PREVIOUS",   true,  fill: 10));
            dgvCalendar.Columns.Add(MakeCol("Forecast",  "FORECAST",   true,  fill: 10));
            dgvCalendar.Columns.Add(MakeCol("Actual",    "ACTUAL",     true,  fill: 10));
        }

        private static DataGridViewTextBoxColumn MakeCol(string name, string header, bool readOnly, int fill)
        {
            return new DataGridViewTextBoxColumn
            {
                Name = name,
                HeaderText = header,
                ReadOnly = readOnly,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
                FillWeight = fill
            };
        }

        private void RebindGrid()
        {
            UiInvoke(() =>
            {
                dgvCalendar.SuspendLayout();
                dgvCalendar.Rows.Clear();
                lock (_eventsLock)
                {
                    foreach (var ev in _events.OrderBy(e => e.TimeUtc))
                    {
                        int idx = dgvCalendar.Rows.Add(
                            ev.Selected,
                            ev.TimeUtc.ToString("yyyy-MM-dd HH:mm:ss"),
                            ev.Currency,
                            ev.EventName,
                            ev.Impact,
                            ev.Previous,
                            ev.Forecast,
                            ev.Actual);
                        dgvCalendar.Rows[idx].Tag = ev;
                    }
                }
                dgvCalendar.ResumeLayout();
            });
        }

        private void DgvCalendar_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= dgvCalendar.Rows.Count) return;
            var row = dgvCalendar.Rows[e.RowIndex];
            if (row.Tag is EconomicEvent ev && e.CellStyle != null)
            {
                if (string.Equals(ev.Impact, "High", StringComparison.OrdinalIgnoreCase))
                    e.CellStyle.ForeColor = NeonRed;
                else if (string.Equals(ev.Impact, "Medium", StringComparison.OrdinalIgnoreCase))
                    e.CellStyle.ForeColor = WarnAmber;
            }
        }

        private void DgvCalendar_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex != 0) return;
            var row = dgvCalendar.Rows[e.RowIndex];
            if (row.Tag is EconomicEvent ev)
            {
                bool newVal = Convert.ToBoolean(row.Cells[0].Value ?? false);
                if (ev.Selected != newVal)
                {
                    ev.Selected      = newVal;
                    ev.TriggerFired  = false;
                    Log($"{(newVal ? "Tracking" : "Untracking")}: {ev}", newVal ? NeonTeal : TextDim);
                }
            }
        }

        // =================================================================
        // Calendar fetch
        // =================================================================
        private async void btnRefreshCalendar_Click(object? sender, EventArgs e) => await RefreshCalendarAsync();

        private async Task RefreshCalendarAsync()
        {
            try
            {
                UiInvoke(() => btnRefreshCalendar.Enabled = false);
                Log("Fetching economic calendar feed...", NeonTeal);
                var sw = Stopwatch.StartNew();
                var fetched = await FetchCalendarDataAsync(_cts.Token);
                sw.Stop();

                lock (_eventsLock)
                {
                    // Preserve Selected/TriggerFired state across refreshes
                    var prev = _events.ToDictionary(
                        x => $"{x.TimeUtc:O}|{x.Currency}|{x.EventName}",
                        x => x);
                    _events.Clear();
                    foreach (var ev in fetched)
                    {
                        var key = $"{ev.TimeUtc:O}|{ev.Currency}|{ev.EventName}";
                        if (prev.TryGetValue(key, out var old))
                        {
                            ev.Selected     = old.Selected;
                            ev.TriggerFired = old.TriggerFired;
                        }
                        _events.Add(ev);
                    }
                }

                RebindGrid();
                Log($"Calendar refreshed: {fetched.Count} events ({sw.ElapsedMilliseconds} ms).", NeonTeal);
            }
            catch (Exception ex)
            {
                Log($"Calendar fetch failed: {ex.Message}", NeonRed);
            }
            finally
            {
                UiInvoke(() => btnRefreshCalendar.Enabled = true);
            }
        }

        /// <summary>
        /// Fetches the economic event feed. Attempts a live HTTP fetch first
        /// (ForexFactory-style weekly JSON) and falls back to a locally
        /// synthesized intraday schedule keyed off the current UTC day so the
        /// engine has time-correct events to drive countdowns even without
        /// internet connectivity.
        /// </summary>
        public async Task<List<EconomicEvent>> FetchCalendarDataAsync(CancellationToken ct = default)
        {
            var list = new List<EconomicEvent>();

            // ---- 1. Try live structured feed ----
            try
            {
                using var http = new System.Net.Http.HttpClient
                {
                    Timeout = TimeSpan.FromSeconds(6)
                };
                http.DefaultRequestHeaders.UserAgent.ParseAdd("AutoscriptsNewsTrader/1.0");
                var url  = "https://nfs.faireconomy.media/ff_calendar_thisweek.json";
                var json = await http.GetStringAsync(url, ct);

                using var doc = System.Text.Json.JsonDocument.Parse(json);
                foreach (var item in doc.RootElement.EnumerateArray())
                {
                    string title    = item.TryGetProperty("title",    out var p1) ? p1.GetString() ?? "" : "";
                    string country  = item.TryGetProperty("country",  out var p2) ? p2.GetString() ?? "" : "";
                    string impact   = item.TryGetProperty("impact",   out var p3) ? p3.GetString() ?? "Low" : "Low";
                    string dateStr  = item.TryGetProperty("date",     out var p4) ? p4.GetString() ?? "" : "";
                    string forecast = item.TryGetProperty("forecast", out var p5) ? p5.GetString() ?? "" : "";
                    string previous = item.TryGetProperty("previous", out var p6) ? p6.GetString() ?? "" : "";
                    string actual   = item.TryGetProperty("actual",   out var p7) ? p7.GetString() ?? "" : "";

                    if (!DateTime.TryParse(dateStr, System.Globalization.CultureInfo.InvariantCulture,
                        System.Globalization.DateTimeStyles.AdjustToUniversal | System.Globalization.DateTimeStyles.AssumeUniversal,
                        out var dt))
                    {
                        continue;
                    }

                    list.Add(new EconomicEvent
                    {
                        TimeUtc   = dt.ToUniversalTime(),
                        Currency  = country,
                        EventName = title,
                        Impact    = NormalizeImpact(impact),
                        Previous  = previous,
                        Forecast  = forecast,
                        Actual    = actual
                    });
                }

                if (list.Count > 0)
                {
                    Log($"Live feed accepted ({list.Count} rows).", NeonTeal);
                    return list;
                }
            }
            catch (Exception ex)
            {
                Log($"Live feed unavailable ({ex.GetType().Name}: {ex.Message}). Falling back to local schedule.", WarnAmber);
            }

            // ---- 2. Fallback: structured intraday schedule for today (UTC) ----
            var today = DateTime.UtcNow.Date;
            (int h, int m, string ccy, string name, string impact)[] schedule = new[]
            {
                (08, 00, "EUR", "German CPI YoY",                   "High"),
                (09, 00, "EUR", "ECB Lagarde Speaks",               "Medium"),
                (10, 00, "EUR", "Eurozone Industrial Production",   "Medium"),
                (12, 30, "USD", "Initial Jobless Claims",           "Medium"),
                (12, 30, "USD", "Non-Farm Payrolls",                "High"),
                (12, 30, "USD", "Unemployment Rate",                "High"),
                (13, 30, "CAD", "BoC Rate Statement",               "High"),
                (14, 00, "USD", "ISM Manufacturing PMI",            "High"),
                (18, 00, "USD", "FOMC Statement",                   "High"),
                (18, 30, "USD", "FOMC Press Conference",            "High"),
                (23, 50, "JPY", "BoJ Monetary Policy Statement",    "High"),
            };
            foreach (var s in schedule)
            {
                list.Add(new EconomicEvent
                {
                    TimeUtc   = today.AddHours(s.h).AddMinutes(s.m),
                    Currency  = s.ccy,
                    EventName = s.name,
                    Impact    = s.impact,
                    Previous  = "-",
                    Forecast  = "-",
                    Actual    = ""
                });
            }
            return list;
        }

        private static string NormalizeImpact(string raw)
        {
            if (string.IsNullOrWhiteSpace(raw)) return "Low";
            var s = raw.Trim().ToLowerInvariant();
            if (s.StartsWith("h")) return "High";
            if (s.StartsWith("m")) return "Medium";
            return "Low";
        }

        // =================================================================
        // Clock loop (UTC + local) - 100ms cadence
        // =================================================================
        private async Task ClockLoopAsync(CancellationToken ct)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
            try
            {
                while (await timer.WaitForNextTickAsync(ct))
                {
                    var utc   = DateTime.UtcNow;
                    var local = DateTime.Now;
                    UiInvoke(() =>
                    {
                        lblUtcClock.Text    = $"UTC {utc:HH:mm:ss}.{utc.Millisecond:D3}";
                        lblBrokerClock.Text = $"LOCAL {local:HH:mm:ss}";
                    });
                }
            }
            catch (OperationCanceledException) { }
        }

        // =================================================================
        // Countdown loop (100ms precision) + auto-trigger
        // =================================================================
        private async Task CountdownLoopAsync(CancellationToken ct)
        {
            using var timer = new PeriodicTimer(TimeSpan.FromMilliseconds(100));
            try
            {
                while (await timer.WaitForNextTickAsync(ct))
                {
                    int offset = SafeReadOffset();
                    EconomicEvent? target;

                    var now = DateTime.UtcNow;
                    lock (_eventsLock)
                    {
                        target = _events
                            .Where(e => e.Selected
                                     && string.Equals(e.Impact, "High", StringComparison.OrdinalIgnoreCase)
                                     && !e.TriggerFired
                                     && e.TimeUtc > now.AddSeconds(-30))
                            .OrderBy(e => e.TimeUtc)
                            .FirstOrDefault();
                    }

                    if (target == null)
                    {
                        UiInvoke(() =>
                        {
                            lblCountdownTarget.Text = "No high-impact event tracked";
                            lblCountdownValue.Text  = "--:--:--.---";
                            lblCountdownValue.ForeColor = TextDim;
                        });
                        continue;
                    }

                    var remaining   = target.TimeUtc - now;
                    var displayText = FormatCountdown(remaining);
                    var color = remaining.TotalSeconds <= offset ? NeonRed
                              : remaining.TotalSeconds <= 60     ? WarnAmber
                              : NeonTeal;

                    var ev = target;
                    UiInvoke(() =>
                    {
                        lblCountdownTarget.Text     = $"Next: {ev.Currency} • {ev.EventName} @ {ev.TimeUtc:HH:mm:ss} UTC";
                        lblCountdownValue.Text      = displayText;
                        lblCountdownValue.ForeColor = color;
                    });

                    // Trigger condition
                    if (!target.TriggerFired && remaining.TotalSeconds <= offset)
                    {
                        target.TriggerFired = true;
                        Log($"COUNTDOWN TRIGGER reached for {target.EventName} ({target.Currency}) at offset {offset}s.", NeonRed);
                        _ = Task.Run(() => SendNewsOrderAsync(automated: true, ct));
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                Log($"Countdown loop error: {ex.Message}", NeonRed);
            }
        }

        private int SafeReadOffset()
        {
            int v = 5;
            try
            {
                if (InvokeRequired)
                    v = (int)Invoke(new Func<int>(() => (int)numOffsetSeconds.Value));
                else
                    v = (int)numOffsetSeconds.Value;
            }
            catch { }
            return v;
        }

        private static string FormatCountdown(TimeSpan ts)
        {
            if (ts.TotalMilliseconds <= 0)
            {
                var abs = ts.Negate();
                return $"-{(int)abs.TotalHours:D2}:{abs.Minutes:D2}:{abs.Seconds:D2}.{abs.Milliseconds:D3}";
            }
            return $"{(int)ts.TotalHours:D2}:{ts.Minutes:D2}:{ts.Seconds:D2}.{ts.Milliseconds:D3}";
        }

        // =================================================================
        // Named pipe server
        // =================================================================
        private async Task PipeServerLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    _pipeServer = new NamedPipeServerStream(
                        PipeName,
                        PipeDirection.InOut,
                        1,
                        PipeTransmissionMode.Byte,
                        PipeOptions.Asynchronous);

                    SetPipeStatus(false);
                    Log("Pipe server listening, awaiting MT5 EA connection...", TextDim);

                    await _pipeServer.WaitForConnectionAsync(ct);

                    _pipeWriter = new StreamWriter(_pipeServer, new UTF8Encoding(false))
                    {
                        AutoFlush = true,
                        NewLine   = "\n"
                    };
                    _pipeConnected = true;
                    SetPipeStatus(true);
                    Log("MT5 EA connected via named pipe.", NeonTeal);

                    // Read inbound messages from EA (heartbeats / acks)
                    using var reader = new StreamReader(_pipeServer, new UTF8Encoding(false), leaveOpen: true);
                    while (_pipeServer.IsConnected && !ct.IsCancellationRequested)
                    {
                        string? line;
                        try { line = await reader.ReadLineAsync(ct); }
                        catch (OperationCanceledException) { break; }
                        catch { break; }

                        if (line == null) break;
                        Log($"<EA< {line}", TextDim);
                    }
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    Log($"Pipe error: {ex.Message}", NeonRed);
                }
                finally
                {
                    _pipeConnected = false;
                    SetPipeStatus(false);
                    try { _pipeWriter?.Dispose(); } catch { }
                    _pipeWriter = null;
                    try { _pipeServer?.Dispose(); } catch { }
                    _pipeServer = null;
                    Log("Pipe client disconnected. Calendar & settings retained. Restarting listener...", WarnAmber);
                }

                if (!ct.IsCancellationRequested)
                {
                    try { await Task.Delay(500, ct); } catch { }
                }
            }
        }

        private void SetPipeStatus(bool connected)
        {
            UiInvoke(() =>
            {
                if (connected)
                {
                    lblPipeStatus.Text      = "● PIPE CONNECTED";
                    lblPipeStatus.ForeColor = NeonTeal;
                }
                else
                {
                    lblPipeStatus.Text      = "● PIPE OFFLINE";
                    lblPipeStatus.ForeColor = NeonRed;
                }
            });
        }

        // =================================================================
        // Trade dispatch
        // =================================================================
        private async void btnForceStraddle_Click(object? sender, EventArgs e)
        {
            await SendNewsOrderAsync(automated: false, _cts.Token);
        }

        /// <summary>
        /// Builds and dispatches the news order packet:
        /// COMMAND;NEWS_ORDER;[Symbol];[LotSize];[DistancePips];[SL];[TP];[OCO]
        /// </summary>
        private async Task SendNewsOrderAsync(bool automated, CancellationToken ct)
        {
            string  symbol   = "EURUSD";
            decimal lot      = 1.0M;
            decimal distance = 150;
            decimal sl       = 200;
            decimal tp       = 400;
            bool    oco      = true;

            try
            {
                Action snapshot = () =>
                {
                    symbol   = (txtSymbol.Text ?? "").Trim().ToUpperInvariant();
                    lot      = numLotSize.Value;
                    distance = numDistancePips.Value;
                    sl       = numSL.Value;
                    tp       = numTP.Value;
                    oco      = chkOCO.Checked;
                };
                if (InvokeRequired) Invoke(snapshot);
                else snapshot();
            }
            catch { }

            if (string.IsNullOrWhiteSpace(symbol))
            {
                Log("Symbol is empty - aborting dispatch.", NeonRed);
                return;
            }

            string packet = string.Format(System.Globalization.CultureInfo.InvariantCulture,
                "COMMAND;NEWS_ORDER;{0};{1};{2};{3};{4};{5}",
                symbol,
                lot.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture),
                ((int)distance).ToString(System.Globalization.CultureInfo.InvariantCulture),
                ((int)sl).ToString(System.Globalization.CultureInfo.InvariantCulture),
                ((int)tp).ToString(System.Globalization.CultureInfo.InvariantCulture),
                oco ? "1" : "0");

            if (!_pipeConnected || _pipeWriter == null)
            {
                Log($"[DISPATCH ABORTED - PIPE OFFLINE] would send: {packet}", NeonRed);
                return;
            }

            var sw   = Stopwatch.StartNew();
            var sent = DateTime.UtcNow;
            try
            {
                await _pipeWriteLock.WaitAsync(ct);
                try
                {
                    await _pipeWriter.WriteLineAsync(packet.AsMemory(), ct);
                    await _pipeWriter.FlushAsync();
                }
                finally { _pipeWriteLock.Release(); }
                sw.Stop();
                var tag = automated ? "AUTO" : "MANUAL";
                Log($">EA> [{tag}] {packet}", NeonTeal);
                Log($"Dispatched at {sent:HH:mm:ss.fff} UTC | transmit latency {sw.Elapsed.TotalMilliseconds:F3} ms", NeonTeal);
            }
            catch (Exception ex)
            {
                Log($"Dispatch failed: {ex.Message}", NeonRed);
            }
        }
    }
}
