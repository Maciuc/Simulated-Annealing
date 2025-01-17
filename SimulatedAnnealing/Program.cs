using System.Diagnostics;

class SimulatedAnnealingProject
{
    static Random random = new Random();

    static void Main()
    {
        Console.SetIn(new StreamReader(Console.OpenStandardInput(8192)));

        double[,] distances = {
            {   0, 29, 20, 21, 16, 31, 100, 12,   4, 31 },
            {  29,  0, 15, 29, 28, 40,  72, 21,  29, 41 },
            {  20, 15,  0, 15, 14, 25,  81,  9,  23, 27 },
            {  21, 29, 15,  0,  4, 12,  92, 12,  25, 13 },
            {  16, 28, 14,  4,  0, 16,  94,  9,  20, 16 },
            {  31, 40, 25, 12, 16,  0,  98, 24,  36,  3 },
            { 100, 72, 81, 92, 94, 98,   0, 90, 101, 99 },
            {  12, 21,  9, 12,  9, 24,  90,  0,  15, 25 },
            {   4, 29, 23, 25, 20, 36, 101, 15,   0, 35 },
            {  31, 41, 27, 13, 16,  3,  99, 25,  35,  0 }
        };

        int numCities = distances.GetLength(0);

        Console.WriteLine("Introdu valoarea temperaturii initiale:");
        double temperature = double.Parse(Console.ReadLine());

        Console.WriteLine("Introdu valoarea ratei de racire (0 < rata < 1):");
        double coolingRate = double.Parse(Console.ReadLine());

        Console.WriteLine("Introdu valoarea temperaturii minime:");
        double absoluteTemperature = double.Parse(Console.ReadLine());

        /*long totalTime = 0;
        for (int i = 1; i <= 100; i++)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            List<int> bestRoute = SimulatedAnnealing(distances, numCities, temperature, coolingRate, absoluteTemperature);

            stopwatch.Stop();
            totalTime += stopwatch.ElapsedMilliseconds;
        }
        Console.WriteLine($"Time average{totalTime / 100}");*/

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        List<int> bestRoute = SimulatedAnnealing(distances, numCities, temperature, coolingRate, absoluteTemperature);

        stopwatch.Stop();


        Console.WriteLine("\nCel mai bun drum gasit:");
        foreach (int city in bestRoute)
        {
            Console.Write($"{city} ");
        }

        Console.WriteLine($"\nCost total: {CalculateRouteCost(bestRoute, distances)}");

        Console.WriteLine($"\nTimpul de executie: {stopwatch.ElapsedMilliseconds} ms");
    }

    static List<int> SimulatedAnnealing(double[,] distances, int numCities, double temperature, double coolingRate, double absoluteTemperature)
    {
        List<int> currentSolution = new List<int>();
        for (int i = 0; i < numCities; i++)
        {
            currentSolution.Add(i);
        }
        //List<int> currentSolution = GenerateInitialSolution(distances, numCities);

        double currentCost = CalculateRouteCost(currentSolution, distances);

        List<int> bestSolution = new List<int>(currentSolution);
        double bestCost = currentCost;

        Console.WriteLine("\nSolutiile generate pe parcurs:");

        while (temperature > absoluteTemperature)
        {
            List<int> newSolution = SwapCities(new List<int>(currentSolution));//New list because it also changes the current solution
            double newCost = CalculateRouteCost(newSolution, distances);

            if (newCost < currentCost || AcceptWorseSolution(currentCost, newCost, temperature))
            {
                currentSolution = newSolution;
                currentCost = newCost;
            }

            if (currentCost < bestCost)
            {
                bestSolution = new List<int>(currentSolution);
                bestCost = currentCost;
            }

            Console.WriteLine($"Temperatura: {temperature:F2}, Cost curent: {currentCost:F2}, Solutie: {string.Join(" ", currentSolution)}");

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
        totalCost += distances[route[^1], route[0]];
        return totalCost;
    }

    static List<int> SwapCities(List<int> route)
    {
        int index1;
        int index2;
        do
        {
            index1 = random.Next(route.Count);
            index2 = random.Next(route.Count);
        }
        while (index1 == index2);

        int aux = route[index1];
        route[index1] = route[index2];
        route[index2] = aux;

        return route;
    }

    static bool AcceptWorseSolution(double currentCost, double newCost, double temperature)
    {
        double acceptanceProbability = Math.Exp((currentCost - newCost) / temperature);
        return random.NextDouble() < acceptanceProbability;
    }


    static List<int> GenerateInitialSolution(double[,] distances, int numCities)
    {
        List<int> initialSolution = new List<int>();
        HashSet<int> visited = new HashSet<int>();

        int currentCity = random.Next(numCities);
        initialSolution.Add(currentCity);
        visited.Add(currentCity);

        while (visited.Count < numCities)
        {
            double nearestDistance = double.MaxValue;
            int nearestCity = -1;

            for (int nextCity = 0; nextCity < numCities; nextCity++)
            {
                if (!visited.Contains(nextCity) && distances[currentCity, nextCity] < nearestDistance)
                {
                    nearestDistance = distances[currentCity, nextCity];
                    nearestCity = nextCity;
                }
            }

            currentCity = nearestCity;

            initialSolution.Add(currentCity);
            visited.Add(currentCity);
        }

        return initialSolution;
    }

    /*static List<int> TwoOptSwap(List<int> route, int i, int k)
    {
        List<int> newRoute = new List<int>(route);
        newRoute.Reverse(i, k - i + 1);
        return newRoute;
    }

    static List<int> SwapCities(List<int> route)
    {
        int i = random.Next(route.Count - 1);
        int k = random.Next(i + 1, route.Count);
        return TwoOptSwap(route, i, k);
    }*/

}