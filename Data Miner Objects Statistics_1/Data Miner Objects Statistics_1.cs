namespace Data_Miner_Objects_Statistics_1
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Apps.Modules;
	using Skyline.DataMiner.Net.ManagerStore;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	[GQIMetaData(Name = "DOM Instance Count")]
	public class DOMInstancesStatistics : IGQIDataSource, IGQIOnInit
	{
		private long _total = 0;

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[1]
			{
				new GQIStringColumn("Number of DOM instances"),
			};
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			var rows = new List<GQIRow>();

			rows.Add(new GQIRow(
					new GQICell[]
					{
						new GQICell()
						{
							Value = _total.ToString(),
						},
					}));

			return new GQIPage(rows.ToArray());
		}

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
	}
}
