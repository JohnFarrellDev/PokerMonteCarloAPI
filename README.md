# PokerMonteCarloAPI

[![CI](https://github.com/JohnFarrellDev/PokerMonteCarloAPI/actions/workflows/ci.yml/badge.svg)](https://github.com/JohnFarrellDev/PokerMonteCarloAPI/actions/workflows/ci.yml)

PokerMonteCarloAPI is a .NET-based API that uses Monte Carlo simulation to calculate poker hand probabilities.

## Features

- Monte Carlo simulation for poker hand evaluation
- RESTful API endpoints for poker probability calculations
- Docker support for easy deployment and scaling

## Prerequisites

- .NET 7.0 SDK
- Docker (optional, for containerized deployment)

## Getting Started

### Local Development

1. Clone the repository:

   ```
   git clone https://github.com/yourusername/PokerMonteCarloAPI.git
   cd PokerMonteCarloAPI
   ```

2. Build the project:

   ```
   dotnet build
   ```

3. Run the tests:

   ```
   dotnet test
   ```

4. Run the API locally:

   ```
   cd PokerMonteCarloAPI
   dotnet run
   ```

   The API will be available at `http://localhost:5000`.

### CI/CD

This project uses GitHub Actions for Continuous Integration. The workflow includes:

1. Setup: Configures the .NET environment (version 7.0.x).
2. Restore dependencies: Installs all required packages.
3. Build stage: Compiles the project and checks for build errors.
4. Test stage: Runs all unit tests to ensure code quality and functionality.

You can view the CI workflow status by clicking on the CI badge at the top of this README.

### Docker Deployment

1. Build the Docker image:

   ```
   docker build -t poker-monte-carlo-api .
   ```

2. Run the container:

   ```
   docker run -p 5000:80 poker-monte-carlo-api
   ```

   The API will be available at `http://localhost:5000`.
