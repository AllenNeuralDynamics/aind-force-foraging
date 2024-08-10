using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using OpenCV.Net;

[Combinator]
[Description("Interpolates the intensity value of the input image given a sub-pixel coordinate, using bilinear interpolation.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class SubPixelBilinearInterpolation
{

    [Description("The lower value of X used to linearly scale the input coordinate to.")]
    public float XMin { get; set; }
    [Description("The upper value of X used to linearly scale the input coordinate to.")]
    public float XMax { get; set; }
    [Description("The lower value of Y used to linearly scale the input coordinate to.")]
    public float YMin { get; set; }
    [Description("The upper value of Y used to linearly scale the input coordinate to.")]
    public float YMax { get; set; }

    [Description("The value given to X < XMin. if null it will default to XMin.")]
    public float? XMinBoundTo { get; set; }
    [Description("The value given to X > XMax. if null it will default to XMax.")]
    public float? XMaxBoundTo { get; set; }
    [Description("The value given to Y < YMin. if null it will default to YMin.")]
    public float? YMinBoundTo { get; set; }
    [Description("The value given to Y > YMax. if null it will default to YMax.")]
    public float? YMaxBoundTo { get; set; }


    public IObservable<float> Process(IObservable<Tuple<IplImage, Point2f>> source)
    {
        var XMin = this.XMin;
        var XMax = this.XMax;
        var YMin = this.YMin;
        var YMax = this.YMax;
        var XMinBoundTo = this.XMinBoundTo.HasValue ? this.XMinBoundTo.Value : XMin;
        var XMaxBoundTo = this.XMaxBoundTo.HasValue ? this.XMaxBoundTo.Value : XMax;
        var YMinBoundTo = this.YMinBoundTo.HasValue ? this.YMinBoundTo.Value : YMin;
        var YMaxBoundTo = this.YMaxBoundTo.HasValue ? this.YMaxBoundTo.Value : YMax;

        if (XMin >= XMax || YMin >= YMax)
        {
            throw new ArgumentException("Minimum must be strictly lower than maximum.");
        }
        if (XMinBoundTo >= XMaxBoundTo || YMinBoundTo >= YMaxBoundTo)
        {
            throw new ArgumentException("The minimum bound must be less than the maximum bound.");
        }

        return source.Select(value =>
        {
            var image = value.Item1;
            if (image.Channels > 1){
                throw new ArgumentException("Input image must have a single channel");
            }

            var point = value.Item2;
            point = RescalePointToImage(point, XMin, XMax, YMin, YMax, image.Size);
            point = ClampPoint(point, XMinBoundTo, XMaxBoundTo, YMinBoundTo, YMaxBoundTo);
            return GetSubPixel(image, point);
        });
    }
    public IObservable<float> Process(IObservable<Tuple<Point2f, IplImage>> source)
    {
        return Process(source.Select(value => Tuple.Create(value.Item2, value.Item1)));
    }


    private static float GetSubPixel(IplImage src, Point2f point)
    {
        var x = (int)point.X;
        var y = (int)point.Y;
        float dx = point.X - x;
        float dy = point.Y - y;
        Scalar p00 = src[y, x];
        Scalar p01 = src[y, x + 1];
        Scalar p10 = src[y + 1, x];
        Scalar p11 = src[y + 1, x + 1];
        return (float) (p00.Val0 * (1 - dx) * (1 - dy) + p01.Val0 * dx * (1 - dy) + p10.Val0 * (1 - dx) * dy + p11.Val0 * dx * dy);
    }

    private static Point2f RescalePointToImage(Point2f point, float XMin, float XMax, float YMin, float YMax, Size ImageSize)
    {
        point.X = Rescale(point.X, XMin, XMax, 0, ImageSize.Width);
        point.Y = Rescale(point.Y, YMin, YMax, 0, ImageSize.Height);
        return new Point2f(point.X, point.Y);
    }

    private static Point2f ClampPoint(Point2f point, float XMinBoundTo, float XMaxBoundTo, float YMinBoundTo, float YMaxBoundTo)
    {
        return new Point2f(
            Math.Min(Math.Max(point.X, XMinBoundTo), XMaxBoundTo),
            Math.Min(Math.Max(point.Y, YMinBoundTo), YMaxBoundTo)
        );
    }

    static private float Rescale(float value, float minFrom, float maxFrom, float minTo, float maxTo)
    {
        return (value - minFrom) / (maxFrom - minFrom) * (maxTo - minTo) + minTo;
    }
}
