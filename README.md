# SensorsGame3 🎯

**Tactical Agent Investigation Game – Console-Based**

_SensorsGame3_ is an immersive C# console game where you play as an Israeli intelligence investigator. Your mission? Uncover and neutralize Iranian agents by deducing their hidden sensor vulnerabilities. Each agent has a unique weakness combination, a defined rank, and must be cracked with strategy, logic, and smart use of advanced sensors.

---

## 🎮 Gameplay Highlights

- Strategic deduction-based gameplay – **not just guessing**
- Agents come in multiple **ranks** (Footsoldier → Commander)
- Each agent has a **hidden sensor combo** required to expose them
- Use a variety of sensors – some detect, some deceive, some jam
- Advanced mechanics: **Jammer** sensors can disable discovered weaknesses
- Levels progress in difficulty; agents get tougher to expose

---

## 🧱 Project Structure (SOLID + Modular)

- `Entities/` – Core domain classes (Agent, Sensor, Player, etc.)
- `Enums/` – Enumerations like `SensorType`, `AgentRank`
- `Managers/` – Main logic handlers (InvestigationManager, PlayerManager)
- `Factory/` – Factories for agent and sensor creation
- `DAL/` – MySQL-based data access layer (reports, scores, players)
- `Tools/` – Menus, input handlers, printers, helpers
- `Run.cs` – Main game loop
- `Program.cs` – Application entry point

---

## 🧠 Advanced Features

### 🕵️ Agent System
- Every agent has a randomized secret sensor combo
- Detection requires multiple correct sensors – based on agent rank
- Some agents (like `JammerAgent`) can block already-discovered weaknesses
- Designed for future extensibility with new agent types

### 🧪 Sensor System
- Sensors with distinct behaviors:
  - Revealing sensors
  - Deceptive sensors
  - Jamming/disabling sensors
- Sensor availability depends on the agent's rank

### 🗂️ Game History & Scoring
- Detailed game history per player:
  - Agent rank & type
  - Sensor usage history
  - Correct vs incorrect attempts
  - Turn count
  - Victory/defeat
  - Timestamped
- Scoring based on:
  - Agent difficulty
  - Number of turns
  - Accuracy of sensor usage
- High score leaderboard support

### 👤 Player Profiles
- Persistent player system (MySQL-backed)
- Tracks:
  - Username (unique)
  - Highest unlocked agent rank
  - Total games played
  - Avrage score and turns
  - Total victories

---

## 🚀 Getting Started

### Prerequisites

- Visual Studio (recommended)
- .NET Framework 4.7.2 or higher
- MySQL Server & SQL Data Connector (via NuGet)

### Installation

```bash
git clone https://github.com/Eclips77/SensorsGame3.git
