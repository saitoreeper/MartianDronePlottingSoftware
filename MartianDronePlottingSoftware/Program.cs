class Program
{
    static void Main(string[] args)
    {
        (double upperRightX, double upperRightY) = GetFieldCoordinates();

        int gridPointsX = (int)Math.Floor(upperRightX);
        int gridPointsY = (int)Math.Floor(upperRightY);
        List<(double X, double Y)> gridCoordinates = GenerateGridCoordinates(gridPointsX, gridPointsY);

        DisplayInitialGrid(gridPointsX, gridPointsY);

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

        var paths = ProcessMovements(initialPositions, movementsList, upperRightX, upperRightY, numberOfDrones);

        var intersections = CheckForIntersections(paths, numberOfDrones);

        DisplayFinalLocations(paths, numberOfDrones);
        DisplayIntersections(intersections);

        DrawGridWithDrones(gridPointsX, gridPointsY, paths, numberOfDrones, intersections);

        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static (double upperRightX, double upperRightY) GetFieldCoordinates()
    {
        Console.Write("Enter the X coordinate of the upper-right corner of the field: ");
        double upperRightX = double.Parse(Console.ReadLine());

        Console.Write("Enter the Y coordinate of the upper-right corner of the field: ");
        double upperRightY = double.Parse(Console.ReadLine());

        return (upperRightX, upperRightY);
    }

    static int GetNumberOfDrones()
    {
        Console.Write("Enter the number of drones: ");
        return int.Parse(Console.ReadLine());
    }

    static List<(double X, double Y)> GenerateGridCoordinates(int gridPointsX, int gridPointsY)
    {
        var gridCoordinates = new List<(double X, double Y)>();

        for (int i = 0; i <= gridPointsX; i++)
        {
            for (int j = 0; j <= gridPointsY; j++)
            {
                double x = i;
                double y = j;
                gridCoordinates.Add((x, y));
            }
        }

        return gridCoordinates;
    }

    static void DisplayInitialGrid(int gridPointsX, int gridPointsY)
    {
        Console.WriteLine("Initial Grid:");
        for (int y = gridPointsY; y >= 0; y--)
        {
            for (int x = 0; x <= gridPointsX; x++)
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

    static List<List<(double X, double Y)>> ProcessMovements(List<(double X, double Y)> initialPositions, List<string> movementsList, double upperRightX, double upperRightY, int numberOfDrones)
    {
        var paths = new List<List<(double X, double Y)>>();

        for (int i = 0; i < numberOfDrones; i++)
        {
            double currentX = initialPositions[i].X;
            double currentY = initialPositions[i].Y;
            string movements = movementsList[i];

            var path = new List<(double X, double Y)>();
            path.Add((currentX, currentY));

            foreach (char move in movements)
            {
                switch (move)
                {
                    case 'N':
                        currentY += 1;
                        break;
                    case 'S':
                        currentY -= 1;
                        break;
                    case 'E':
                        currentX += 1;
                        break;
                    case 'W':
                        currentX -= 1;
                        break;
                }

                if (currentX < 0 || currentX > upperRightX || currentY < 0 || currentY > upperRightY)
                {
                    Console.WriteLine($"Drone {i + 1} moved outside the field to ({currentX}, {currentY}). Movement stopped.");
                    break;
                }

                path.Add((currentX, currentY));
            }

            paths.Add(path);
        }

        return paths;
    }

    static List<(int Drone1, int Drone2, double X, double Y)> CheckForIntersections(List<List<(double X, double Y)>> paths, int numberOfDrones)
    {
        var intersections = new List<(int Drone1, int Drone2, double X, double Y)>();
        double tolerance = 0.01;

        for (int i = 0; i < numberOfDrones; i++)
        {
            for (int j = i + 1; j < numberOfDrones; j++)
            {
                var path1 = paths[i];
                var path2 = paths[j];

                foreach (var coord1 in path1)
                {
                    foreach (var coord2 in path2)
                    {
                        if (Math.Abs(coord1.X - coord2.X) < tolerance && Math.Abs(coord1.Y - coord2.Y) < tolerance)
                        {
                            intersections.Add((i + 1, j + 1, coord1.X, coord1.Y));
                        }
                    }
                }
            }
        }

        return intersections;
    }

    static void DisplayFinalLocations(List<List<(double X, double Y)>> paths, int numberOfDrones)
    {
        Console.WriteLine("Final locations of all drones:");
        for (int i = 0; i < numberOfDrones; i++)
        {
            var finalPosition = paths[i][paths[i].Count - 1];
            Console.WriteLine($"Drone {i + 1}: X: {finalPosition.X}, Y: {finalPosition.Y}");
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

    static void DrawGridWithDrones(int gridPointsX, int gridPointsY, List<List<(double X, double Y)>> paths, int numberOfDrones, List<(int Drone1, int Drone2, double X, double Y)> intersections)
    {
        Console.WriteLine("Grid with Drones:");
        for (int y = gridPointsY; y >= 0; y--)
        {
            for (int x = 0; x <= gridPointsX; x++)
            {
                bool isDronePosition = false;
                bool isIntersection = false;

                foreach (var intersection in intersections)
                {
                    if (Math.Abs(intersection.X - x) < 0.01 && Math.Abs(intersection.Y - y) < 0.01)
                    {
                        Console.Write("I ");
                        isIntersection = true;
                        break;
                    }
                }

                if (!isIntersection)
                {
                    for (int i = 0; i < numberOfDrones; i++)
                    {
                        var finalPosition = paths[i][paths[i].Count - 1];
                        if (Math.Abs(finalPosition.X - x) < 0.01 && Math.Abs(finalPosition.Y - y) < 0.01)
                        {
                            Console.Write("D ");
                            isDronePosition = true;
                            break;
                        }
                    }
                }

                if (!isIntersection && !isDronePosition)
                {
                    Console.Write(". ");
                }
            }
            Console.WriteLine();
        }
    }
}