# Pawn Game Agent

Pawn Game Agent is a C# project that implements an AI agent for playing a game of pawns. This project includes two executable files:

- **Local Play Executable**: Play against the AI agent locally.
- **Server Connection Executable**: Connect the agent to a server to compete with other agents.

Additionally, the complete source code is provided, allowing you to run the project directly from Visual Studio using the run button.

## Overview

Pawn Game Agent is designed to showcase a C# AI that plays the game of pawns. The project supports two modes of operation: a local play mode and a server-connected mode, offering flexibility for both casual play and competitive agent-versus-agent matches.

## Features

- **Dual Executable Files**:
  - **Local Play**: Launch a game against the AI agent with a simple double-click.
  - **Server Connection**: Connect to a server and engage in multiplayer or agent-vs-agent gameplay.
- **No Terminal Required**: Both executables are user-friendly and require only a click to start.
- **Visual Studio Integration**: Run the code directly from Visual Studio using the run button.
- **Demo Videos**: Step-by-step videos guide you through running both executable files are provided in the submission, one that runs the agent locally, and one that runs the agent on the server.

## Prerequisites

- [Visual Studio](https://visualstudio.microsoft.com/) (any version that supports C#)
- [.NET Framework or .NET Core](https://dotnet.microsoft.com/download) (depending on your project setup)

## Installation

1. **Clone or Download the Project**  
   Download the project from the submission package or clone the repository:
   ```bash
   git clone https://github.com/yourusername/pawn-game-agent.git
   ```

Open in Visual Studio
Open the solution file (PawnGameAgent.sln) in Visual Studio to review or modify the code.
Usage
Server Connection Executable
Prepare the Server:
Make sure that the server is already running and listening for connections.

Run the Executable:
Double-click the PawnGameAgent_Server.exe file. The agent will automatically connect to the server.

If no opponent is connected, the agent will wait for one.
If an opponent is already connected, the game starts immediately, and you simply watch the game play out.
Local Play Executable
Setup Pieces:
When you run the PawnGameAgent_Local.exe file, the agent will prompt you to enter the initial setup for the pieces. For example:

Setup Wa2 Wb2 Wc2 Wd2 We2 Wf2 Wg2 Wh2 Ba7 Bb7 Bc7 Bd7 Be7 Bf7 Bg7 Bh7
Set Overall Time:
After pressing Enter, you will be asked to provide an overall time (in minutes) for the game. For example, if you enter 5, each agent will have 5 minutes of play time.

Select Game Mode:
Next, choose one of the following options:

Human vs Human:
The game starts and both players manually input their moves (e.g., entering a2a4 to move the pawn from a2 to a4).

Human vs Agent:
You will be prompted to select which color you wish to play. After making your choice, the game starts with you playing against the AI.

Agent vs Agent:
The game starts immediately with both agents playing automatically, requiring no further input from the user.
