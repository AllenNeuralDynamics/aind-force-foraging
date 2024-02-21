using Bonsai;
using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;
using AindForceForagingDataSchema.AindForceForagingTask;


public class ParseForce: Transform<Timestamped<short[]>, Force>
{
    public int LeftLoadCellIndex { get; set; }

    public int RightLoadCellIndex { get; set; }


    public override IObservable<Force> Process(IObservable<Timestamped<short[]>> source)
    {
        return source.Select(value => {
            var force = new Force();
            force.Seconds = value.Seconds;
            force.LeftForce = value.Value[LeftLoadCellIndex];
            force.RightForce = value.Value[RightLoadCellIndex];
            return force;
        });
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
