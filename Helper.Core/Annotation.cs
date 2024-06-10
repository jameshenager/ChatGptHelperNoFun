namespace Helper.Core;

public class AnnotationPoint
{
    public DoublePoint Point { get; set; }
    public TimeSpan TimeStamp { get; set; }
}

public class DoublePoint(double x, double y)
{
    public double X { get; set; } = x;
    public double Y { get; set; } = y;
}

public class MovingTextData
{
    public required string Text { get; set; }
    public int X1 { get; set; }
    public int X2 { get; set; }
    public int Y1 { get; set; }
    public int Y2 { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
}