using System;
using System.Collections.Generic;

class SimulatedAnnealingComisVoiajor
{
    static Random random = new Random();

    static void Main(string[] args)
    {
        // Exemplu matrice de distanțe între orașe
        double[,] distances = {
            { 0, 10, 15, 20 },
            { 10, 0, 35, 25 },
            { 15, 35, 0, 30 },
            { 20, 25, 30, 0 }
        };

        int numCities = distances.GetLength(0);
        List<int> bestRoute = SolveTSP(distances, numCities);

        Console.WriteLine("Cel mai bun drum găsit:");
        foreach (int city in bestRoute)
        {
            Console.Write($"{city} ");
        }
    }

    static List<int> SolveTSP(double[,] distances, int numCities)
    {
        // Soluție inițială: orașele în ordine
        List<int> currentSolution = new List<int>();
        for (int i = 0; i < numCities; i++) currentSolution.Add(i);
        double currentCost = CalculateRouteCost(currentSolution, distances);

        // Parametri Simulated Annealing
        double temperature = 10000;
        double coolingRate = 0.995;
        double absoluteTemperature = 0.00001;

        List<int> bestSolution = new List<int>(currentSolution);
        double bestCost = currentCost;

        while (temperature > absoluteTemperature)
        {
            // Generează o nouă soluție (swap între două orașe)
            List<int> newSolution = GenerateNeighbor(new List<int>(currentSolution));
            double newCost = CalculateRouteCost(newSolution, distances);

            // Decide dacă acceptă noua soluție
            if (newCost < currentCost || AcceptWorseSolution(currentCost, newCost, temperature))
            {
                currentSolution = newSolution;
                currentCost = newCost;
            }

            // Actualizează cea mai bună soluție
            if (currentCost < bestCost)
            {
                bestSolution = new List<int>(currentSolution);
                bestCost = currentCost;
            }

            // Reduce temperatura
            temperature *= coolingRate;
        }

        return bestSolution;
    }

    static double CalculateRouteCost(List<int> route, double[,] distances)
    {
        double totalCost = 0;
        for (int i = 0; i < route.Count - 1; i++)
        {
            totalCost += distances[route[i], route[i + 1]];
        }
        totalCost += distances[route[^1], route[0]]; // Adaugă întoarcerea la punctul de start
        return totalCost;
    }

    static List<int> GenerateNeighbor(List<int> route)
    {
        int index1 = random.Next(route.Count);
        int index2 = random.Next(route.Count);

        // Swap între două orașe
        int temp = route[index1];
        route[index1] = route[index2];
        route[index2] = temp;

        return route;
    }

    static bool AcceptWorseSolution(double currentCost, double newCost, double temperature)
    {
        double acceptanceProbability = Math.Exp((currentCost - newCost) / temperature);
        return random.NextDouble() < acceptanceProbability;
    }
}