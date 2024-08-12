using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using AindForceForagingDataSchema.Rig;
using Harp.StepperDriver;
using Bonsai.Harp;
using YamlDotNet.Serialization.ObjectGraphTraversalStrategies;

[Combinator]
[Description("")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class MoveAbsoluteSingleAxis
{
    private Axis axis = Axis.Y1;
    public Axis Axis
    {
        get { return axis; }
        set { axis = value; }
    }

    private MessageType messageType = MessageType.Write;
    public MessageType MessageType
    {
        get { return messageType; }
        set { messageType = value; }
    }


    public IObservable<HarpMessage> Process(IObservable<int> source)
    {
        return source.Select(value => {
            switch (Axis)
            {
                case Axis.X:
                    return Motor0MoveAbsolute.FromPayload(MessageType, value);
                case Axis.Y1:
                    return Motor1MoveAbsolute.FromPayload(MessageType, value);
                case Axis.Y2:
                    return Motor2MoveAbsolute.FromPayload(MessageType, value);
                case Axis.Z:
                    return Motor3MoveAbsolute.FromPayload(MessageType, value);
                default:
                    throw new InvalidOperationException("Invalid axis selection.");
            }
        });
    }
}
