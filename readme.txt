# Algorithms Visualization - WinForms (OOP Academic Project)

A desktop application built with C# and Windows Forms. This project was developed as an academic assignment for an Object-Oriented Programming (OOP) course to demonstrate the strict application of OOP paradigms, event-driven architecture, and design patterns. It visualizes the execution of three distinct algorithmic modules.

## Architecture & OOP Principles
To fulfill the academic requirements, the project strictly enforces:
* **Separation of Concerns:** The presentation layer (Windows Forms) is completely isolated from the business logic.
* **Orchestrator Pattern:** Dedicated classes handle event routing between the UI and algorithm engines, ensuring technology-agnostic algorithms.
* **Polymorphism & Interfaces:** Algorithm engines (e.g., sorting) are abstracted behind interfaces (`ISorter`) to demonstrate loose coupling.

## Features
* **Dijkstra's Algorithm (Pathfinding):** Visualizes the step-by-step process of finding the shortest path between nodes in a generated layered graph.
* **QuickSort Algorithm:** An in-place array sorting module. Accepts custom user input and provides precise execution time measurement.
* **LZW Compression (Lempel-Ziv-Welch):** A lossless dictionary-based text compression tool demonstrating generating numeric codes and reconstructing strings.

## Technologies Used
* C#
* .NET Framework / .NET (WinForms)
* LINQ

## Screenshots

## License
Distributed under the MIT License.
