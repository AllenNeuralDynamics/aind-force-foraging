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

    public IObservable<Timestamped<double[]>> Process(IObservable<Timestamped<short[]>> source)
    {
        return source.Select(value => {
            if (Calibration == null) return Timestamped.Create(value.Value.Select(v => (double) v).ToArray(), value.Seconds);

            var data = new double[value.Value.Length];
            foreach (var loadCell in Calibration)
            {
                data[loadCell.LoadCellIndex] = (value.Value[loadCell.LoadCellIndex] - loadCell.Baseline) * loadCell.Slope;
            }
            return Timestamped.Create(data, value.Seconds);
        });
    }
}
