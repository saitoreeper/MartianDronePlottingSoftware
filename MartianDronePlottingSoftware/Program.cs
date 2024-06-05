class Program
{
    static void Main(string[] args)
    {
        double fieldSize = GetFieldSize();
        double droneSize = GetDroneSize();

        int gridPoints = (int)Math.Floor(fieldSize / droneSize);
        List<(double X, double Y)> gridCoordinates = GenerateGridCoordinates(gridPoints, droneSize);

        DisplayInitialGrid(gridPoints);

        int numberOfDrones = GetNumberOfDrones();

        var initialPositions = new List<(double X, double Y)>();
        var movementsList = new List<string>();

        for (int i = 0; i < numberOfDrones; i++)
        {
            Console.WriteLine($"Setting up drone {i + 1}");
            var initialPosition = GetInitialPosition(gridCoordinates);
            initialPositions.Add(initialPosition);
            var movements = GetValidMovements(i);
            movementsList.Add(movements);
        }

        var finalPositions = ProcessMovements(initialPositions, movementsList, droneSize, fieldSize, numberOfDrones);

        var intersections = CheckForIntersections(finalPositions, numberOfDrones);

        DisplayFinalLocations(finalPositions, numberOfDrones);
        DisplayIntersections(intersections);

        DrawGridWithDrones(gridPoints, droneSize, finalPositions, numberOfDrones);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static double GetFieldSize()
    {
        Console.Write("Enter the size of the field (in meters, square field): ");
        return double.Parse(Console.ReadLine());
    }

    static double GetDroneSize()
    {
        Console.Write("Enter the size of the drone (in meters): ");
        return double.Parse(Console.ReadLine());
    }

    static int GetNumberOfDrones()
    {
        Console.Write("Enter the number of drones: ");
        return int.Parse(Console.ReadLine());
    }

    static List<(double X, double Y)> GenerateGridCoordinates(int gridPoints, double droneSize)
    {
        var gridCoordinates = new List<(double X, double Y)>();

        for (int i = 0; i <= gridPoints; i++)
        {
            for (int j = 0; j <= gridPoints; j++)
            {
                double x = i * droneSize;
                double y = j * droneSize;
                gridCoordinates.Add((x, y));
            }
        }

        return gridCoordinates;
    }

    static void DisplayInitialGrid(int gridPoints)
    {
        Console.WriteLine("Initial Grid:");
        for (int y = gridPoints; y >= 0; y--)
        {
            for (int x = 0; x <= gridPoints; x++)
            {
                Console.Write(". ");
            }
            Console.WriteLine();
        }
    }

    static (double X, double Y) GetInitialPosition(List<(double X, double Y)> gridCoordinates)
    {
        Console.Write("Enter the initial X coordinate of the drone: ");
        double initialX = double.Parse(Console.ReadLine());

        Console.Write("Enter the initial Y coordinate of the drone: ");
        double initialY = double.Parse(Console.ReadLine());

        if (!gridCoordinates.Contains((initialX, initialY)))
        {
            Console.WriteLine("Initial coordinates are outside the field. Please restart and enter valid coordinates.");
            Environment.Exit(0);
        }

        return (initialX, initialY);
    }

    static string GetValidMovements(int droneIndex)
    {
        string movements;
        bool validMovements;

        do
        {
            validMovements = true;
            Console.Write("Enter the movement string (N for up, S for down, E for right, W for left): ");
            movements = Console.ReadLine().ToUpper();

            foreach (char move in movements)
            {
                if (move != 'N' && move != 'S' && move != 'E' && move != 'W')
                {
                    Console.WriteLine($"Invalid move '{move}' in the movement string for drone {droneIndex + 1}. Please re-enter the movement string.");
                    validMovements = false;
                    break;
                }
            }
        } while (!validMovements);

        return movements;
    }

    static List<(double X, double Y)> ProcessMovements(List<(double X, double Y)> initialPositions, List<string> movementsList, double droneSize, double fieldSize, int numberOfDrones)
    {
        var finalPositions = new List<(double X, double Y)>();

        for (int i = 0; i < numberOfDrones; i++)
        {
            double currentX = initialPositions[i].X;
            double currentY = initialPositions[i].Y;
            string movements = movementsList[i];

            foreach (char move in movements)
            {
                switch (move)
                {
                    case 'N':
                        currentY += droneSize;
                        break;
                    case 'S':
                        currentY -= droneSize;
                        break;
                    case 'E':
                        currentX += droneSize;
                        break;
                    case 'W':
                        currentX -= droneSize;
                        break;
                }

                if (currentX < 0 || currentX > fieldSize || currentY < 0 || currentY > fieldSize)
                {
                    Console.WriteLine($"Drone {i + 1} moved outside the field to ({currentX}, {currentY}). Movement stopped.");
                    break;
                }
            }

            finalPositions.Add((currentX, currentY));
        }

        return finalPositions;
    }

    static List<(int Drone1, int Drone2, double X, double Y)> CheckForIntersections(List<(double X, double Y)> finalPositions, int numberOfDrones)
    {
        var intersections = new List<(int Drone1, int Drone2, double X, double Y)>();
        double tolerance = 0.01;

        for (int i = 0; i < numberOfDrones; i++)
        {
            for (int j = i + 1; j < numberOfDrones; j++)
            {
                if (Math.Abs(finalPositions[i].X - finalPositions[j].X) < tolerance &&
                    Math.Abs(finalPositions[i].Y - finalPositions[j].Y) < tolerance)
                {
                    intersections.Add((i + 1, j + 1, finalPositions[i].X, finalPositions[i].Y));
                }
            }
        }

        return intersections;
    }


    static void DisplayFinalLocations(List<(double X, double Y)> finalPositions, int numberOfDrones)
    {
        Console.WriteLine("Final locations of all drones:");
        for (int i = 0; i < numberOfDrones; i++)
        {
            Console.WriteLine($"Drone {i + 1}: X: {finalPositions[i].X}, Y: {finalPositions[i].Y}");
        }
    }

    static void DisplayIntersections(List<(int Drone1, int Drone2, double X, double Y)> intersections)
    {
        if (intersections.Count > 0)
        {
            Console.WriteLine("Intersections detected:");
            foreach (var intersection in intersections)
            {
                Console.WriteLine($"Drone {intersection.Drone1} and Drone {intersection.Drone2} intersect at ({intersection.X}, {intersection.Y})");
            }
        }
    }

    static void DrawGridWithDrones(int gridPoints, double droneSize, List<(double X, double Y)> finalPositions, int numberOfDrones)
    {
        Console.WriteLine("Grid with Drones:");
        for (int y = gridPoints; y >= 0; y--)
        {
            for (int x = 0; x <= gridPoints; x++)
            {
                bool isDronePosition = false;
                for (int i = 0; i < numberOfDrones; i++)
                {
                    if (Math.Abs(finalPositions[i].X - x * droneSize) < 1e-6 && Math.Abs(finalPositions[i].Y - y * droneSize) < 1e-6)
                    {
                        Console.Write("D ");
                        isDronePosition = true;
                        break;
                    }
                }
                if (!isDronePosition)
                {
                    Console.Write(". ");
                }
            }
            Console.WriteLine();
        }
    } 
}