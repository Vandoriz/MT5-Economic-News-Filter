<div align="center">

Topics: metatrader5, mql5, economic-calendar, expert-advisor, mql4, metatrader, forex-trading, automated-trading, trading-tools, market-analysis, news-trading, forex-news, fundamental-analysis, mt4, mt5, trading-bot, mt5-economic-calendar, mt5-news-filter, forex-news-lock

# Information

**Forex economic calendar and news intelligence suite for MT5 / MT4 traders, built to track macro events, central bank decisions, and high-impact data releases in real time. The project delivers a complete fundamental analysis layer with event scoring, currency impact filters, and automatic trade-pause logic for news-sensitive strategies.**

# 📅 Economic Calendar MT5/MT4

**Real-time economic calendar with high-impact event detection, automatic news filtering, and trade-pause automation for safer fundamental-aware trading.**

<br>

[![Stars](https://img.shields.io/github/stars/torvalds/linux?style=for-the-badge&color=00D4AA&label=Stars)](https://github.com/your-username/volume-profile-mt5/stargazers)
[![Forks](https://img.shields.io/github/forks/torvalds/linux?style=for-the-badge&color=4D9FFF&label=Forks)](https://github.com/your-username/volume-profile-mt5/network)
[![Issues](https://img.shields.io/github/issues/torvalds/linux?style=for-the-badge&color=FF4D6A&label=Issues)](https://github.com/your-username/volume-profile-mt5/issues)
[![Platform](https://img.shields.io/badge/MT5%20%2F%20MT4-Compatible-00D4AA?style=for-the-badge)](https://www.metatrader5.com)
[![License](https://img.shields.io/badge/License-MIT-4D9FFF?style=for-the-badge)](LICENSE)

</div>

<p align="center">
    <img src="https://minkxx-spotify-readme.vercel.app/api?theme=dark&rainbow=true&scan=true&spin=True" alt="Preview">
</p>

---

## 📸 Screenshot

<div align="center">

<p align="center">
  <img src="https://i.ibb.co/bj897d3k/1.png" alt="Economic Calendar UI" width="820">
</p>

</div>

---

## 🎬 Demo

<div align="center">

<img src="https://i.imgur.com/JPK4A8V.gif" alt="Demo">

</div>


---

## Why Fundamental Filtering?

Most retail losses on event days come from one thing: trading into a news spike.

This project removes that risk by giving you:
- A live, color-coded macro calendar  
- Currency-level impact filtering  
- Automatic trade-pause windows around red-folder events  

---

## What It Does

**Economic Calendar MT5/MT4** turns raw macro data into a structured decision layer for your trading engine.

| Module | Description |
|---|---|
| Event Feed | Pulls upcoming releases with impact rating |
| Impact Filter | Splits High / Medium / Low folder events |
| Currency Mapper | Maps each event to affected FX pairs |
| News Lock | Blocks order entry inside event windows |
| Countdown Engine | Live timer to the next red-folder event |
| Session Log | History of every news pause and trigger |

---

## Features

| Feature | Description |
|---|---|
| Live Calendar Grid | Time, country, event, forecast, previous |
| Impact Heatmap | Red / orange / yellow severity colors |
| Currency Filter | Show only events for traded symbols |
| Pre-News Window | Configurable lock minutes before release |
| Post-News Window | Configurable cool-down after release |
| Time Zone Sync | Auto-aligns calendar to broker time |
| MT4 / MT5 Support | Platform selection system |
| Audio Alerts | Sound notification on red-folder events |
| Event Notes | Forecast vs actual deviation tracking |
| Risk Controls | Pause-only or pause + close logic |

---

## System Behavior

- Fully event-driven UI
- Auto-refresh on schedule
- Configurable per-symbol filters
- Silent fail on network drop, never blocks trading without a flag

---

## Quick Start

**Requirements:**
- Windows 10 / 11  
- .NET 8+  
- Visual Studio 2022  

```bash
git clone https://github.com/your-username/economic-calendar.git
```

Open solution → Press **F5**

---

## How to Use

1. Launch app  
2. Select MT4 / MT5  
3. Enter login  
4. Click **CONNECT**  
5. Pick currencies & impact level  
6. Set pre/post news windows  
7. Click **ENABLE NEWS LOCK**  
8. Monitor countdown and pauses  

---

## Interface Logic

```
HIGH    ████  RED FOLDER
MED     ██    ORANGE
LOW     █     YELLOW
LOCK ─────[ EVENT ]─────
```

- Red = trading paused  
- Green = trading active  
- Countdown ticks every second  
- Triggered events logged below  

---

## Roadmap

- [x] Calendar feed  
- [x] Impact filter  
- [x] News-lock engine  
- [ ] Real MT5 EA bridge  
- [ ] Forecast vs actual scoring  
- [ ] Historical news backtest  
- [ ] Multi-source aggregation  

---

## Contributing

```
1. Fork
2. git checkout -b feature/new-feature
3. git commit -m "Add feature"
4. git push
5. Open PR
```

---

## License

MIT

---

<div align="center">

Economic Calendar MT5/MT4 · v1.0

</div>
