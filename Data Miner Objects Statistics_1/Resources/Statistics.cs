namespace DataMinerObjectsStatistics.Resources
{
	using System.Collections.Generic;
	using System.Threading;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Messages;

	[GQIMetaData(Name = "Total number of resources")]
	public class ResourcesCount : IGQIDataSource, IGQIOnInit
	{
		private ResourceManagerHelper resourceHelper;

		private int _total = 0;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			resourceHelper = new ResourceManagerHelper(args.DMS.SendMessage);

			return default;
		}

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[1]
			{
				new GQIIntColumn("Count"),
			};
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			var pools = PoolProvider.GetResourcePools(resourceHelper);
			foreach (var pool in pools.Values)
				_total += pool.Resources.Count;

			var rows = new List<GQIRow>()
			{
				new GQIRow(new GQICell[]
					{
						new GQICell()
						{
							Value = _total,
						},
					}),
			};

			return new GQIPage(rows.ToArray());
		}
	}

	[GQIMetaData(Name = "Number of resources per pool")]
	public class ResourcesPerPoolCount : IGQIDataSource, IGQIOnInit
	{
		private ResourceManagerHelper resourceHelper;
		private Dictionary<string, int> _counts = new Dictionary<string, int>();

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			resourceHelper = new ResourceManagerHelper(args.DMS.SendMessage);

			return default;
		}

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[2]
			{
				new GQIStringColumn("Pool"),
				new GQIIntColumn("Count"),
			};
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			int timeout = 120;
			while (PoolProvider.IsRunning && timeout-- > 0)
			{
				Thread.Sleep(1000);
			}

			if (timeout <= 0)
				return new GQIPage(new GQIRow[0]);

			var pools = PoolProvider.GetResourcePools(resourceHelper);
			foreach (var pool in pools.Values)
				_counts.Add(pool.Name, pool.Resources.Count);

			var rows = new List<GQIRow>();

			foreach (var count in _counts)
			{
				rows.Add(new GQIRow(
					new GQICell[]
					{
						new GQICell()
						{
							Value = count.Key,
						},
						new GQICell()
						{
							Value = count.Value,
						},
					}));
			}

			return new GQIPage(rows.ToArray());
		}
	}
}