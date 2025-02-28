# Pawn Game Agent

**Pawn Game Agent** is a C# project implementing an AI agent for strategic pawn gameplay. The project offers two executable modes and full Visual Studio integration.

## Overview

A C# AI agent designed for pawn-based strategy games. Features two operational modes:

- **Local Play**: Compete directly against the AI
- **Server Mode**: Connect to external servers for agent-vs-agent competitions

## Key Features

- **Dual Launch Options**
  - **Local Play Executable**: Instant AI opponent with double-click launch
  - **Server Connection Executable**: Competitive multiplayer integration
- **Seamless Integration**
  - Direct Visual Studio execution support
  - No terminal/command-line required
- **Learning Resources**
  - Included demo videos for local/server operation
  - Step-by-step gameplay guides

## Prerequisites

### Installation Requirements

- [Visual Studio](https://visualstudio.microsoft.com/) (C# compatible version)
- [.NET Framework/Core](https://dotnet.microsoft.com/download) (version matching project setup)

### Project Setup

1. **Open Solution**
   - Launch `ChessBot.sln` in Visual Studio

## Usage Guide

#### Please Check the videos for a step by step tutorial on how to run the code

### Server Connection Mode

1. **Server Preparation**
   - Ensure game server is running and listening
2. **Launch Agent**
   - Double-click `Agent_Server.exe`
3. **Game Initialization**
   - Automatic server connection
   - Matches begin when opponent connects

### Local Play Mode

1. **Launch Executable**
   - Run `Agent_Local.exe`
2. **Initial Setup**

   ```shell
   Setup Wa2 Wb2 Wc2 Wd2 We2 Wf2 Wg2 Wh2 Ba7 Bb7 Bc7 Bd7 Be7 Bf7 Bg7 Bh7
   ```

3. **Time Configuration**

- Enter total game time in minutes (e.g., 5)

4. **Game Mode Selection**

- Human vs Human: Manual move entry (e.g., a2a4)

- Human vs AI: Choose your color

- AI vs AI: Autonomous gameplay

### Execution Options

#### Visual Studio Users

Click Run button in IDE

If you want to play locally, comment out the server region in the program.cs and uncomment the local region

If you want to connect the agent to the server, comment out the local region and uncomment the server region in the program.cs file

Select startup project (Local/Server)

Follow on-screen prompts

Additional Resources
Included demonstration videos:

LocalGame.mp4

ServerGame.mp4
