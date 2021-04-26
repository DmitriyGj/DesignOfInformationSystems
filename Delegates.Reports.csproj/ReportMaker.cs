using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Reports
{
	public static class ReportMaker
	{
		public static string MakeReport(Format style, Processor info,IEnumerable<Measurement> measurements)
		{
			var data = measurements.ToList();
			var result = new StringBuilder();
			result.Append(style.MakeCaption(info.Caption));
			result.Append(style.BeginList);
			result.Append(style.MakeItem("Temperature", info.MakeStatistics(data.Select(z => z.Temperature)).ToString()));
			result.Append(style.MakeItem("Humidity", info.MakeStatistics(data.Select(z => z.Humidity)).ToString()));
			result.Append(style.EndList);
			return result.ToString();
		}
	}

	public class Format
    {
		public string EndList, BeginList;
		public Func<string, string> MakeCaption;
		public Func<string, string, string> MakeItem;
		public Format(string beginList, string endList, Func<string,string> captMaker, Func<string,string,string> itemMaker)
        {
			EndList = endList;
			BeginList = beginList;
			MakeCaption = captMaker;
			MakeItem = itemMaker;
        }
    }

	public class Processor
    {
		public string Caption;
		public Func<IEnumerable<double>,object> MakeStatistics;
		public Processor(string caption, Func<IEnumerable<double>,object> statMaker)
        {
			Caption = caption;
			MakeStatistics = statMaker;
        }
    }

	public static class ReportMakerHelper
	{
		static readonly Format HTML = new Format("<ul>", "</ul>",
						     (caption) => $"<h1>{caption}</h1>",
							(valueType, entry) => $"<li><b>{valueType}</b>: {entry}");
		static readonly Format MarkDown = new Format("", "",
							    (caption) => $"## {caption}\n\n",
								(valueType, entry) => $" * **{valueType}**: {entry}\n\n");

		static readonly Processor Median = new Processor("Median", (data) =>
		{
			var list = data.OrderBy(z => z).ToList();
			if (list.Count % 2 == 0)
				return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;

			return list[list.Count / 2];
		});

		static readonly Processor MeanAndStd = new Processor("Mean and Std", (data) =>
		{
			var parseddata = data.ToList();
			var mean = parseddata.Average();
			var std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (parseddata.Count - 1));

			return new MeanAndStd
			{
				Mean = mean,
				Std = std
			};
		});
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data) 
							 => ReportMaker.MakeReport(HTML, MeanAndStd, data);
		public static string MedianMarkdownReport(IEnumerable<Measurement> data) 
							 => ReportMaker.MakeReport(MarkDown, Median, data);
		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> data) 
							 => ReportMaker.MakeReport(MarkDown, MeanAndStd, data);
		public static string MedianHtmlReport(IEnumerable<Measurement> data) 
							 => ReportMaker.MakeReport(HTML, Median, data);
	}
}
