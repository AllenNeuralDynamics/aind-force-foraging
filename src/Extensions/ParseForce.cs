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

    private SubPixelBilinearInterpolator interpolator;

    public override IObservable<Force> Process(IObservable<Timestamped<short[]>> source)
    {
        var lookUpTable = LookUpTable.Clone();
        var forceOperationControl = ForceOperationControl;

        if (forceOperationControl.PressMode == PressMode.SingleLookupTable)
        {
            if (lookUpTable == null)
            {
                throw new InvalidOperationException("Look-up table must be specified for SingleLookupTable mode.");
            }
            else
            {
                var forceLutSettings = forceOperationControl.ForceLookupTable;
                interpolator = new SubPixelBilinearInterpolator
                {
                    Limits = new ForceLookUpTable
                    {
                        LeftMin = forceLutSettings.LeftMin,
                        LeftMax = forceLutSettings.LeftMax,
                        RightMin = forceLutSettings.RightMin,
                        RightMax = forceLutSettings.RightMax,
                        LeftMinBoundTo = forceLutSettings.LeftMinBoundTo,
                        LeftMaxBoundTo = forceLutSettings.LeftMaxBoundTo,
                        RightMinBoundTo = forceLutSettings.RightMinBoundTo,
                        RightMaxBoundTo = forceLutSettings.RightMaxBoundTo
                    },
                    LookUpTable = lookUpTable
                };
            }
        }

        return source.Select(value =>
        {
            var force = new Force
            {
                Seconds = value.Seconds,
                LeftForce = value.Value[ForceOperationControl.LeftIndex],
                RightForce = value.Value[ForceOperationControl.RightIndex]
            };
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

class SubPixelBilinearInterpolator
{
    public ForceLookUpTable Limits { get; set; }

    public Mat LookUpTable { get; set; }

    private void Validate()
    {
        if (Limits == null)
        {
            throw new InvalidOperationException("Limits must be specified.");
        }
        if (Limits.LeftMin >= Limits.LeftMax || Limits.RightMin >= Limits.RightMax)
        {
            throw new ArgumentException("Minimum must be strictly lower than maximum.");
        }
        if (Limits.LeftMinBoundTo >= Limits.LeftMaxBoundTo || Limits.RightMinBoundTo >= Limits.RightMaxBoundTo)
        {
            throw new ArgumentException("The minimum bound must be less than the maximum bound.");
        }
        if (LookUpTable.Channels > 1)
        {
            throw new ArgumentException("Input matrix must have a single channel");
        }
    }

    public float LookUp(float leftValue, float rightValue)
    {
        leftValue = Rescale(leftValue, (float)Limits.LeftMin, (float)Limits.LeftMax, 0, LookUpTable.Size.Height);
        rightValue = Rescale(rightValue, (float)Limits.RightMin, (float)Limits.RightMax, 0, LookUpTable.Size.Width);

        leftValue = ClampValue(leftValue, (float)Limits.LeftMinBoundTo, (float)Limits.LeftMaxBoundTo);
        rightValue = ClampValue(rightValue, (float)Limits.RightMinBoundTo, (float)Limits.RightMaxBoundTo);

        return GetSubPixel(LookUpTable, leftValue, rightValue);
    }

    static private float Rescale(float value, float minFrom, float maxFrom, float minTo, float maxTo)
    {
        return (value - minFrom) / (maxFrom - minFrom) * (maxTo - minTo) + minTo;
    }
    private static float ClampValue(float value, float MinBoundTo, float MaxBoundTo)
    {
        return Math.Min(Math.Max(value, MinBoundTo), MaxBoundTo);
    }
    private static float GetSubPixel(Mat src, float leftValue, float rightValue)
    {
        var idxL = (int)leftValue;
        var idxR = (int)rightValue;
        float dL = leftValue - idxL;
        float dR = rightValue - idxR;

        var p00 = src[idxL, idxR];
        var p01 = src[idxL, idxR + 1];
        var p10 = src[idxL + 1, idxR];
        var p11 = src[idxL + 1, idxR + 1];
        return (float)(p00.Val0 * (1 - dR) * (1 - dL) + p01.Val0 * dR * (1 - dL) + p10.Val0 * (1 - dR) * dL + p11.Val0 * dR * dL);
    }
}





