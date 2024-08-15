using Bonsai;
using System;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using Bonsai.Harp;
using System.Xml.Serialization;

[Combinator]
[Description("Applies a LoadCellsCalibration object to a sequence of load cell data values.")]
[WorkflowElementCategory(ElementCategory.Transform)]
public class ApplyLoadCellsCalibration
{
    [XmlIgnore]
    public LoadCellsCalibrations Calibration {get; set;}

    public IObservable<Timestamped<short[]>> Process(IObservable<Timestamped<short[]>> source)
    {
        return source.Select(value => {
            if (Calibration == null) return value;

            short[] data = value.Value;
            foreach (var loadCell in Calibration)
            {
                data[loadCell.LoadCellIndex] = (short)(data[loadCell.LoadCellIndex] - loadCell.Baseline);
            }
            return Timestamped.Create(data, value.Seconds);
        });
    }
}
