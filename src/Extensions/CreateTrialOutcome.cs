using Bonsai;
using System;
using System.Linq;
using System.Reactive.Linq;
using AindForceForagingDataSchema.TaskLogic;

public class CreateTrialOutcome : Source<TrialOutcome>
{
    public HarvestAction HarvestAction { get; set; }

    public int TrialNumber { get; set; }

    public float? Reward { get; set; }

    public bool IsAborted { get; set; }

    public IObservable<TrialOutcome> Generate<TSource>(IObservable<TSource> source)
    {
        return source.Select(value => new TrialOutcome
        {
            HarvestAction = HarvestAction,
            TrialNumber = TrialNumber,
            Reward = Reward,
            IsAborted = IsAborted
        });
    }

    public override IObservable<TrialOutcome> Generate()
    {
        return Observable.Return(new TrialOutcome
        {
            HarvestAction = HarvestAction,
            TrialNumber = TrialNumber,
            Reward = Reward,
            IsAborted = IsAborted
        });
    }
}


public class TrialOutcome
{
    public HarvestAction HarvestAction { get; set; }

    public int TrialNumber { get; set; }

    public float? Reward { get; set; }

    public bool IsAborted { get; set; }
}
