namespace DataMinerObjectsStatistics.DOM
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.Modules;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	[GQIMetaData(Name = "Total number of DOM instances")]
	public class DOMInstancesCount : IGQIDataSource, IGQIOnInit
	{
		private long _total = 0;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			List<ModuleSettings> modulesSettings = new ModuleSettingsHelper(args.DMS.SendMessages).ModuleSettings.ReadAll();

			var filter = DomInstanceExposers.Name.NotEqual(string.Empty);

			foreach (var moduleSettings in modulesSettings)
			{
				var domHelper = new DomHelper(args.DMS.SendMessages, moduleSettings.ModuleId);

				_total += domHelper.DomInstances.Count(filter);
			}

			return default;
		}

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[1]
			{
				new GQIDoubleColumn("Count"),
			};
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			var rows = new List<GQIRow>()
			{
				new GQIRow(new GQICell[]
					{
						new GQICell()
						{
							Value = (double)_total,
						},
					}),
			};

			return new GQIPage(rows.ToArray());
		}
	}

	[GQIMetaData(Name = "Number of DOM instances per module")]
	public class DOMInstancesPerModuleCount : IGQIDataSource, IGQIOnInit
	{
		private readonly Dictionary<string, long> _counts = new Dictionary<string, long>();

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			List<ModuleSettings> modulesSettings = new ModuleSettingsHelper(args.DMS.SendMessages).ModuleSettings.ReadAll();

			var filter = DomInstanceExposers.Name.NotEqual(string.Empty);

			foreach (var moduleSettings in modulesSettings)
				_counts.Add(moduleSettings.ModuleId, new DomHelper(args.DMS.SendMessages, moduleSettings.ModuleId).DomInstances.Count(filter));

			return default;
		}

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[2]
			{
				new GQIStringColumn("Name"),
				new GQIDoubleColumn("Count"),
			};
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			var rows = new List<GQIRow>();

			foreach (var count in _counts)
			{
				rows.Add(new GQIRow(
						new GQICell[]
						{
							new GQICell()
							{
								Value = count.Key.ToString(),
							},
							new GQICell()
							{
								Value = (double)count.Value,
							},
						}));
			}

			return new GQIPage(rows.ToArray());
		}
	}
}