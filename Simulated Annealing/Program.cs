using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

public class TspSimulatedAnnealingForm : Form
{
    private Panel canvas;
    private TextBox numCitiesBox, initialTempBox, coolingRateBox, iterationsBox;
    private Button runButton;
    private Label resultLabel, iterationLabel, costLabel, temperatureLabel;
    private Random random = new Random();
    private List<PointF> cities;
    private List<int> bestSolution;

    public TspSimulatedAnnealingForm()
    {
        this.Text = "Simulated Annealing - TSP";
        this.WindowState = FormWindowState.Maximized;

        var controlPanel = new Panel { Dock = DockStyle.Top, Height = 150 };
        canvas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };

        var numCitiesLabel = new Label { Text = "Number of Cities:", Left = 10, Top = 10, Width = 120 };
        numCitiesBox = new TextBox { Left = 140, Top = 10, Width = 50 };

        var initialTempLabel = new Label { Text = "Initial Temp:", Left = 10, Top = 40, Width = 120 };
        initialTempBox = new TextBox { Left = 140, Top = 40, Width = 50 };

        var coolingRateLabel = new Label { Text = "Cooling Rate:", Left = 210, Top = 10, Width = 120 };
        coolingRateBox = new TextBox { Left = 340, Top = 10, Width = 50 };

        var iterationsLabel = new Label { Text = "Iterations:", Left = 210, Top = 40, Width = 120 };
        iterationsBox = new TextBox { Left = 340, Top = 40, Width = 50 };

        runButton = new Button { Text = "Run Algorithm", Left = 410, Top = 25, Width = 120 };
        runButton.Click += RunButton_Click;

        resultLabel = new Label { Left = 550, Top = 10, Width = 200 };
        iterationLabel = new Label { Text = "Iteration: 0", Left = 10, Top = 80, Width = 200 };
        costLabel = new Label { Text = "Cost: 0", Left = 220, Top = 80, Width = 200 };
        temperatureLabel = new Label { Text = "Temperature: 0", Left = 430, Top = 80, Width = 200 };

        controlPanel.Controls.Add(numCitiesLabel);
        controlPanel.Controls.Add(numCitiesBox);
        controlPanel.Controls.Add(initialTempLabel);
        controlPanel.Controls.Add(initialTempBox);
        controlPanel.Controls.Add(coolingRateLabel);
        controlPanel.Controls.Add(coolingRateBox);
        controlPanel.Controls.Add(iterationsLabel);
        controlPanel.Controls.Add(iterationsBox);
        controlPanel.Controls.Add(runButton);
        controlPanel.Controls.Add(resultLabel);
        controlPanel.Controls.Add(iterationLabel);
        controlPanel.Controls.Add(costLabel);
        controlPanel.Controls.Add(temperatureLabel);

        this.Controls.Add(controlPanel);
        this.Controls.Add(canvas);
    }

    private void RunButton_Click(object sender, EventArgs e)
    {
        if (!int.TryParse(numCitiesBox.Text, out int numCities) ||
            !double.TryParse(initialTempBox.Text, out double initialTemp) ||
            !double.TryParse(coolingRateBox.Text, out double coolingRate) ||
            !int.TryParse(iterationsBox.Text, out int iterations))
        {
            resultLabel.Text = "Invalid input!";
            return;
        }

        cities = GenerateCities(numCities);
        bestSolution = Enumerable.Range(0, numCities).ToList();

        new Thread(() =>
        {
            SimulatedAnnealing(numCities, initialTemp, coolingRate, iterations);
            Invoke(new Action(() => resultLabel.Text = "Done!"));
        }).Start();
    }

    private List<PointF> GenerateCities(int numCities)
    {
        var cityList = new List<PointF>();
        for (int i = 0; i < numCities; i++)
        {
            cityList.Add(new PointF((float)(random.NextDouble() * canvas.Width), (float)(random.Next(2,10) / 10.0 * canvas.Height)));
        }
        return cityList;
    }

    private void SimulatedAnnealing(int numCities, double initialTemp, double coolingRate, int iterations)
    {
        var currentSolution = bestSolution.ToList();
        double currentCost = CalculateCost(currentSolution);
        bestSolution = currentSolution.ToList();
        double bestCost = currentCost;

        double temperature = initialTemp;

        for (int iter = 0; iter < iterations; iter++)
        {
            var newSolution = GenerateNeighbor(currentSolution);
            double newCost = CalculateCost(newSolution);

            if (newCost < currentCost || random.NextDouble() < Math.Exp((currentCost - newCost) / temperature))
            {
                currentSolution = newSolution;
                currentCost = newCost;
            }

            if (currentCost < bestCost)
            {
                bestSolution = currentSolution.ToList();
                bestCost = currentCost;
            }

            temperature *= coolingRate;

            // Update labels in real-time
            Invoke(new Action(() =>
            {
                iterationLabel.Text = $"Iteration: {iter + 1}";
                costLabel.Text = $"Cost: {bestCost:F2}";
                temperatureLabel.Text = $"Temperature: {temperature:F2}";
            }));

            DrawSolution(bestSolution);
            Thread.Sleep(200); // Slow down iterations
        }
    }

    private List<int> GenerateNeighbor(List<int> solution)
    {
        var newSolution = solution.ToList();
        int i = random.Next(solution.Count);
        int j = random.Next(solution.Count);
        (newSolution[i], newSolution[j]) = (newSolution[j], newSolution[i]);
        return newSolution;
    }

    private double CalculateCost(List<int> solution)
    {
        double cost = 0;
        for (int i = 0; i < solution.Count; i++)
        {
            var a = cities[solution[i]];
            var b = cities[solution[(i + 1) % solution.Count]];
            cost += Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
        }
        return cost;
    }

    private void DrawSolution(List<int> solution)
    {
        if (canvas.InvokeRequired)
        {
            canvas.Invoke(new Action(() => DrawSolution(solution)));
            return;
        }

        using (var g = canvas.CreateGraphics())
        {
            g.Clear(Color.White);

            for (int i = 0; i < cities.Count; i++)
            {
                var city = cities[i];
                g.FillEllipse(Brushes.Red, city.X - 10, city.Y - 10, 20, 20);
                g.DrawString(i.ToString(), new Font("Arial", 10, FontStyle.Bold), Brushes.Black, city.X + 12, city.Y - 12);
            }

            for (int i = 0; i < solution.Count; i++)
            {
                var a = cities[solution[i]];
                var b = cities[solution[(i + 1) % solution.Count]];
                var midPoint = new PointF((a.X + b.X) / 2, (a.Y + b.Y) / 2);

                g.DrawLine(new Pen(Color.DarkRed, 3), a, b);
                g.DrawString(CalculateSegmentCost(a, b).ToString("F2"), new Font("Arial", 10, FontStyle.Bold), Brushes.Green, midPoint);
            }
        }
    }

    private double CalculateSegmentCost(PointF a, PointF b)
    {
        return Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }

    [STAThread]
    public static void Main()
    {
        Application.EnableVisualStyles();
        Application.Run(new TspSimulatedAnnealingForm());
    }
}
