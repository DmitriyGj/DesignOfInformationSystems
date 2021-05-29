using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Reports
{
	public interface IStatisticBuilder
	{
		string Caption { get; }
		Func<IEnumerable<double>, object> MakeStatistics { get; }
	}

	public interface IFormatBuilder
	{
		Func<string, string> MakeCaption { get; }
		Func<string> BeginList { get; }
		Func<string, string, string> MakeItem { get; }
		Func<string> EndList { get; }
	}

	public class ReportMaker
	{
		IFormatBuilder format;
		IStatisticBuilder stat;
		public string MakeReport(IEnumerable<Measurement> measurements)
		{
			var data = measurements.ToList();
			var result = new StringBuilder();
			result.Append(format.MakeCaption(stat.Caption));
			result.Append(format.BeginList());
			result.Append(format.MakeItem("Temperature", stat.MakeStatistics(data.Select(z => z.Temperature)).ToString()));
			result.Append(format.MakeItem("Humidity", stat.MakeStatistics(data.Select(z => z.Humidity)).ToString()));
			result.Append(format.EndList());
			return result.ToString();
		}
		public ReportMaker(IFormatBuilder format, IStatisticBuilder stat)
        {
			this.format = format;
			this.stat = stat;
        }
	}

	public class HTML:IFormatBuilder
    {
        Func<string,string> IFormatBuilder.MakeCaption => (string caption)=>$"<h1>{caption}</h1>";

        Func<string> IFormatBuilder.BeginList => ()=>"<ul>";

        Func<string, string, string> IFormatBuilder.MakeItem => (string valueType, string entry) => $"<li><b>{valueType}</b>: {entry}";

        Func<string> IFormatBuilder.EndList => ()=> "</ul>";
    }

	public class Markdown: IFormatBuilder
    {
		Func<string, string> IFormatBuilder.MakeCaption => (string caption) => $"## {caption}\n\n";

		Func<string> IFormatBuilder.BeginList => () => "";

		Func<string, string, string> IFormatBuilder.MakeItem => (string valueType, string entry) => $" * **{valueType}**: {entry}\n\n";

		Func<string> IFormatBuilder.EndList => () => "";
	}
    public class MeanAndStdStatistics : IStatisticBuilder
    {
        public string Caption { get => "Mean and Std"; }       
		public Func<IEnumerable<double>, object> MakeStatistics { get => (info) => 
			{
			var parseddata = info.ToList();
			var mean = parseddata.Average();
			var std = Math.Sqrt(info.Select(z => Math.Pow(z - mean, 2)).Sum() / (parseddata.Count - 1));
				return new MeanAndStd
				{
				Mean = mean,
				Std = std
				};
			};  
		}
    }

	public class Median : IStatisticBuilder
	{
		public string Caption { get => "Median"; }
		public Func<IEnumerable<double>, object> MakeStatistics
		{
			get => (info) =>
			{
				var list = info.OrderBy(z => z).ToList();
				if (list.Count % 2 == 0)
					return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
				return list[list.Count / 2].ToString();
			};
		}
	}

	public static class ReportMakerHelper
	{
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)=>
			new ReportMaker(new HTML(),new MeanAndStdStatistics()).MakeReport(data);

		public static string MedianMarkdownReport(IEnumerable<Measurement> data)=>
			new ReportMaker(new Markdown(), new Median()).MakeReport(data);

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)=>
			new ReportMaker(new Markdown(), new MeanAndStdStatistics()).MakeReport(measurements);

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)=>
			new ReportMaker(new HTML(), new Median()).MakeReport(measurements);
	}
}
