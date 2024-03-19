using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using AindForceForagingDataSchema.AindForceForagingTask;
using System.Reactive;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]


public class UpdateHarvestAction
{
    private Random random = new Random();
    public Random Random
    {
        get { return random; }
        set { random = value; }
    }
    public IObservable<HarvestAction> Process(IObservable<Tuple<HarvestAction, ActionUpdater, float?>> source)
    {
        return source.Select(value => {
            var action = value.Item1;
            var updater = value.Item2;
            var event_value = value.Item3.HasValue ? value.Item3.Value : 1.0;
            switch (updater.TargetParameter)
            {
                case UpdateTargetParameter.Delay:
                    action.Delay = Update(action.Delay, updater.Updater, event_value);
                    break;
                case UpdateTargetParameter.LowerForceThreshold:
                    action.LowerForceThreshold = Update(action.LowerForceThreshold, updater.Updater, event_value);
                    break;
                case UpdateTargetParameter.UpperForceThreshold:
                    action.UpperForceThreshold = Update(action.UpperForceThreshold, updater.Updater, event_value);
                    break;
                case UpdateTargetParameter.Amount:
                    action.Amount = Update(action.Amount, updater.Updater, event_value);
                    break;
                case UpdateTargetParameter.Probability:
                    action.Probability = Update(action.Probability, updater.Updater, event_value);
                    break;
                default:
                    throw new NotImplementedException("Invalid target parameter.");
            }
            return action;
            });
    }

    public IObservable<HarvestAction> Process(IObservable<Tuple<HarvestAction, ActionUpdater>> source)
    {
        return Process(source.Select(input => Tuple.Create(input.Item1, input.Item2, (float?)null)));
    }

    public IObservable<HarvestAction> Process(IObservable<Tuple<HarvestAction, ActionUpdater, Unit>> source)
    {
        return Process(source.Select(input => Tuple.Create(input.Item1, input.Item2, (float?)null)));
    }

    private double Update(double value, NumericalUpdater updater, double event_value)
    {
        if (updater.Operation == NumericalUpdaterOperation.None)
        {
            return value;
        }
        var updaterParams = updater.Parameters;
        double updated_value;
        var updaterValue = updaterParams.Value.SampleDistribution(Random) * event_value;
        switch (updater.Operation)
        {
            case NumericalUpdaterOperation.Offset:
                updated_value =  value + updaterValue;
                break;
            case NumericalUpdaterOperation.Set:
                updated_value = updaterValue;
                break;
            case NumericalUpdaterOperation.Gain:
                updated_value = value * updaterValue;
                break;
            default:
                throw new ArgumentException("Invalid updater type.");
        }

        // Clamp the update
        updated_value = Math.Min(updated_value, updaterParams.Maximum);
        updated_value = Math.Max(updated_value, updaterParams.Minimum);
        return updated_value;
    }

}
