using Bonsai;
using System;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;
using AindForceForagingDataSchema.AindForceForagingTask;


public class ParseForce: Transform<Timestamped<short[]>, Force>
{
    public int LeftLoadCellIndex { get; set; }

    public int RightLoadCellIndex { get; set; }

    private PressMode pressMode = PressMode.Double;
    public PressMode PressMode
    {
        get { return pressMode; }
        set { pressMode = value; }
    }


    public override IObservable<Force> Process(IObservable<Timestamped<short[]>> source)
    {
        return source.Select(value => {
            var force = new Force
            {
                Seconds = value.Seconds,
                LeftForce = value.Value[LeftLoadCellIndex],
                RightForce = value.Value[RightLoadCellIndex]
            };
            return SolveMode(force, pressMode);
        });
    }

    public IObservable<Force> Process(IObservable<Force> source)
    {
        return source.Select(value => {
            var force = new Force
            {
                Seconds = value.Seconds,
                LeftForce = value.LeftForce,
                RightForce = value.RightForce
            };
            return SolveMode(force, pressMode);
        });
    }

    static Force SolveMode(Force value, PressMode pressMode){
        switch (pressMode){
            case PressMode.Double:
                return value;
            case PressMode.SingleLeft:
                value.RightForce = value.LeftForce;
                return value;
            case PressMode.SingleRight:
                value.LeftForce = value.RightForce;
                return value;
            case PressMode.SingleAverage:
                value.LeftForce = (short)((value.LeftForce + value.RightForce) / 2);
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
            default:
                throw new ArgumentOutOfRangeException("Unknown press mode.");
        }
    }
}


public class Force{
    public double Seconds { get; set; }

    public short LeftForce { get; set; }

    public short RightForce { get; set; }

    public short this[HarvestActionLabel key]
    {
        get {
            if (key == HarvestActionLabel.Left) return LeftForce;
            if (key == HarvestActionLabel.Right) return RightForce;
            if (key == HarvestActionLabel.None) {
                throw new IndexOutOfRangeException();
            };
            throw new IndexOutOfRangeException();
        }
    }

}
