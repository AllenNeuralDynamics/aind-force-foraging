using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using AindForceForagingDataSchema.TaskLogic;
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
    public IObservable<HarvestAction> Process(IObservable<Tuple<HarvestAction, ActionUpdater, double>> source)
    {
        return source.Select(value => {
            var action = value.Item1;
            var updater = value.Item2;
            var eventValue = value.Item3;
            switch (updater.TargetParameter)
            {
                case UpdateTargetParameter.Delay:
                    action.Delay = Update(action.Delay, updater.Updater, eventValue);
                    break;
                case UpdateTargetParameter.LowerForceThreshold:
                    action.LowerForceThreshold = Update(action.LowerForceThreshold, updater.Updater, eventValue);
                    break;
                case UpdateTargetParameter.UpperForceThreshold:
                    action.UpperForceThreshold = Update(action.UpperForceThreshold, updater.Updater, eventValue);
                    break;
                case UpdateTargetParameter.Amount:
                    action.Amount = Update(action.Amount, updater.Updater, eventValue);
                    break;
                case UpdateTargetParameter.Probability:
                    action.Probability = Update(action.Probability, updater.Updater, eventValue);
                    break;
                case UpdateTargetParameter.ForceDuration:
                    action.ForceDuration = Update(action.ForceDuration, updater.Updater, eventValue);
                    break;
                default:
                    throw new NotImplementedException("Invalid target parameter.");
            }
            return action;
            });
    }


    private double Update(double value, NumericalUpdater updater, double eventValue)
    {
        if (updater.Operation == NumericalUpdaterOperation.None){return value;}
        if (double.IsNaN(eventValue)){return value;}

        var updaterParams = updater.Parameters;
        double updated_value;
        var updaterValue = updaterParams.Value.SampleDistribution(Random) * eventValue;
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
