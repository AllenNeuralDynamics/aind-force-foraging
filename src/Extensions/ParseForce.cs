using Bonsai;
using System;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;
using AindForceForagingDataSchema.TaskLogic;
using System.Xml.Serialization;
using System.ComponentModel;
using OpenCV.Net;

public class ParseForce : Transform<Timestamped<short[]>, Force>
{

    public ForceOperationControl ForceOperationControl { get; set; }

    [XmlIgnore]
    [TypeConverter("Bonsai.Dsp.MatConverter, Bonsai.Dsp")]
    public Mat LookUpTable { get; set; }


    public override IObservable<Force> Process(IObservable<Timestamped<short[]>> source)
    {
        Mat lookUpTable = LookUpTable.Clone();
        ForceOperationControl forceOperationControl = ForceOperationControl;
        SubPixelBilinearInterpolator interpolator = new SubPixelBilinearInterpolator();

        if (forceOperationControl.PressMode == PressMode.SingleLookupTable)
        {
            if (lookUpTable == null)
            {
                throw new InvalidOperationException("Look-up table must be specified for SingleLookupTable mode.");
            }
            else
            {
                var forceLutSettings = forceOperationControl.ForceLookupTable;
                interpolator = new SubPixelBilinearInterpolator(
                    new ForceLookUpTable
                    {
                        LeftMin = forceLutSettings.LeftMin,
                        LeftMax = forceLutSettings.LeftMax,
                        RightMin = forceLutSettings.RightMin,
                        RightMax = forceLutSettings.RightMax,
                    },
                    LookUpTable = lookUpTable
                );
                interpolator.Validate();
            }
        }

        return source.Select(value =>
        {
            var force = new Force(
                value.Seconds,
                value.Value[forceOperationControl.LeftIndex],
                value.Value[forceOperationControl.RightIndex]
            );
            return SolveMode(force, forceOperationControl.PressMode, interpolator);
        });
    }

    static Force SolveMode(Force value, PressMode pressMode, SubPixelBilinearInterpolator interpolator)
    {
        switch (pressMode)
        {
            case PressMode.Double:
                return value;
            case PressMode.SingleLeft:
                value.RightForce = value.LeftForce;
                return value;
            case PressMode.SingleRight:
                value.LeftForce = value.RightForce;
                return value;
            case PressMode.SingleAverage:
                value.LeftForce = (value.LeftForce + value.RightForce) / 2;
                value.RightForce = value.LeftForce;
                return value;
            case PressMode.SingleMax:
                value.LeftForce = Math.Max(value.LeftForce, value.RightForce);
                value.RightForce = value.LeftForce;
                return value;
            case PressMode.SingleMin:
                value.LeftForce = Math.Min(value.LeftForce, value.RightForce);
                value.RightForce = value.LeftForce;
                return value;
            case PressMode.SingleLookupTable:
                if (interpolator == null)
                {
                    throw new InvalidOperationException("Interpolator must be specified for SingleLookupTable mode.");
                }
                value.LeftForce = interpolator.LookUp(value.LeftForce, value.RightForce);
                value.RightForce = value.LeftForce;
                return value;
            default:
                throw new ArgumentOutOfRangeException("Unknown press mode.");
        }
    }
}


public class Force
{
    public double Seconds { get; set; }

    public float LeftForce { get; set; }

    public float RightForce { get; set; }

    public Force(double seconds, float leftForce, float rightForce)
    {
        Seconds = seconds;
        LeftForce = leftForce;
        RightForce = rightForce;
    }

    public float this[HarvestActionLabel key]
    {
        get
        {
            if (key == HarvestActionLabel.Left) return LeftForce;
            if (key == HarvestActionLabel.Right) return RightForce;
            if (key == HarvestActionLabel.None)
            {
                throw new IndexOutOfRangeException();
            };
            throw new IndexOutOfRangeException();
        }
    }

}

public class SubPixelBilinearInterpolator
{
    public SubPixelBilinearInterpolator(ForceLookUpTable limits, Mat lookUpTable)
    {
        Limits = limits;
        LookUpTable = lookUpTable;
    }

    public SubPixelBilinearInterpolator(){}

    public ForceLookUpTable Limits { get; private set; }

    public Mat LookUpTable { get; private set; }

    public void Validate()
    {
        if (Limits == null)
        {
            throw new InvalidOperationException("Limits must be specified.");
        }
        if (Limits.LeftMin >= Limits.LeftMax || Limits.RightMin >= Limits.RightMax)
        {
            throw new ArgumentException("Minimum must be strictly lower than maximum.");
        }
        if (LookUpTable.Channels > 1)
        {
            throw new ArgumentException("Input matrix must have a single channel");
        }
    }

    public float LookUp(float leftValue, float rightValue)
    {
        leftValue = Rescale(leftValue, Limits.LeftMin, Limits.LeftMax, 0, LookUpTable.Size.Height);
        rightValue = Rescale(rightValue, Limits.RightMin, Limits.RightMax, 0, LookUpTable.Size.Width);

        leftValue = ClampValue(leftValue, Limits.LeftMin, Limits.LeftMax);
        rightValue = ClampValue(rightValue, Limits.RightMin, Limits.RightMax);

        return GetSubPixel(LookUpTable, leftValue, rightValue);
    }

    private static float Rescale(float value, double minFrom, double maxFrom, double minTo, double maxTo)
    {
        return (float) ((value - minFrom) / (maxFrom - minFrom) * (maxTo - minTo) + minTo);
    }

    private static float ClampValue(float value, double MinBoundTo, double MaxBoundTo)
    {
        return (float) Math.Min(Math.Max(value, MinBoundTo), MaxBoundTo);
    }

    private static float GetSubPixel(Mat src, float leftValue, float rightValue)
    {
        var idxL = (int)leftValue;
        var idxR = (int)rightValue;
        float dL = leftValue - idxL;
        float dR = rightValue - idxR;

        idxL = Math.Min(idxL, src.Size.Height - 2);
        idxR = Math.Min(idxR, src.Size.Width - 2);

        var p00 = src[idxL, idxR];
        var p01 = src[idxL, idxR + 1];
        var p10 = src[idxL + 1, idxR];
        var p11 = src[idxL + 1, idxR + 1];
        return (float)(p00.Val0 * (1 - dR) * (1 - dL) + p01.Val0 * dR * (1 - dL) + p10.Val0 * (1 - dR) * dL + p11.Val0 * dR * dL);
    }
}





