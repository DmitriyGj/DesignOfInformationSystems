using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Reports
{
	public class Format
    {
		public Func<string> EndList;
		public Func<string> BeginList;
		public Func<string, string> MakeCaption;
		public Func<string, string, string> MakeItem;
		public Format(Func<string> beginList, Func<string> endList, Func<string,string> captMaker, Func<string,string,string> itemMaker)
        {
			EndList = endList;
			BeginList =beginList;
			MakeCaption = captMaker;
			MakeItem = itemMaker;
        }
    }

	public class Processor
    {
		public string Caption;
		public Func<IEnumerable<double>, object> MakeStatistics;
		public Processor(string caption, Func<IEnumerable<double>,object> statMaker)
        {
			Caption = caption;
			MakeStatistics = statMaker;
        }
    }

	public static class ReportMakerHelper
	{
		public static string MakeReport(Format format, Processor processor, IEnumerable<Measurement> data)
		 => $"{format.MakeCaption(processor.Caption)}" +
			$"{format.BeginList()}" +
			$"{format.MakeItem("Temperature", processor.MakeStatistics(data.ToList().Select(z => z.Temperature)).ToString())}" +
			$"{format.MakeItem("Humidity", processor.MakeStatistics(data.ToList().Select(z => z.Humidity)).ToString())}"+
		    $"{format.EndList()}";
		
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
		=> MakeReport(new Format(()=>"<ul>", ()=>"</ul>" , (caption) => $"<h1>{caption}</h1>", (valueType, entry) => $"<li><b>{valueType}</b>: {entry}"),
					  new Processor("Mean and Std", (info) =>{
								    var parseddata = info.ToList();
									var mean = parseddata.Average();
									var std = Math.Sqrt(info.Select(z => Math.Pow(z - mean, 2)).Sum() / (parseddata.Count - 1));
									return new MeanAndStd
									{
										Mean = mean,
										Std = std
									};
								}), data);
		/*ReportMaker.MakeReport(HTML, MeanAndStd, data);*/
		public static string MedianMarkdownReport(IEnumerable<Measurement> data)
	     => MakeReport(new Format(()=>"", () => "",(caption) => $"## {caption}\n\n",(valueType, entry) => $" * **{valueType}**: {entry}\n\n"),
								new Processor("Median", (info) => {
									var list = info.OrderBy(z => z).ToList();
									if (list.Count % 2 == 0)
										return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
									return list[list.Count / 2].ToString();
								}), data);
		/*ReportMaker.MakeReport(MarkDown, Median, data);*/
		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> data)
		=> MakeReport(new Format(() => "", () => "", (caption) => $"## {caption}\n\n", (valueType, entry) => $" * **{valueType}**: {entry}\n\n"),
								new Processor("Mean and Std", (info) =>
								{
									var parseddata = info.ToList();
									var mean = parseddata.Average();
									var std = Math.Sqrt(info.Select(z => Math.Pow(z - mean, 2)).Sum() / (parseddata.Count - 1));
									return new MeanAndStd
									{
										Mean = mean,
										Std = std
									};
								}), data);
		/*ReportMaker.MakeReport(MarkDown, MeanAndStd, data);*/
		public static string MedianHtmlReport(IEnumerable<Measurement> data)
		=> MakeReport(new Format(()=>"<ul>", () => "</ul>", (caption) => $"<h1>{caption}</h1>", (valueType, entry) => $"<li><b>{valueType}</b>: {entry}"),
								new Processor("Median", (info) => {
									var list = info.OrderBy(z => z).ToList();
									if (list.Count % 2 == 0)
										return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
									return list[list.Count / 2].ToString();
								}), data);
		/*ReportMaker.MakeReport(HTML, Median, data);*/
	}
}
