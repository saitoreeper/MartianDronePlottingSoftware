namespace MartianDronePlottingSoftware.Models
{
    public static class GlobalParamaters
    {
        public List<(int Drone1, int Drone2, double X, double Y)> Intersections { get; set; } = new List<(int Drone1, int Drone2, double X, double Y)>();
    }
}
