namespace DataMinerObjectsStatistics.Resources
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Messages;

	[GQIMetaData(Name = "Total number of resources")]
	public class ResourcesCount : IGQIDataSource, IGQIOnInit
	{
		private int _total = 0;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			var resourceHelper = new ResourceManagerHelper(args.DMS.SendMessage);

			var pools = PoolProvider.GetResourcePools(resourceHelper);
			foreach (var pool in pools.Values)
				_total += pool.Resources.Count;

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
		private Dictionary<string, int> _counts = new Dictionary<string, int>();

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			var resourceHelper = new ResourceManagerHelper(args.DMS.SendMessage);

			var pools = PoolProvider.GetResourcePools(resourceHelper);
			foreach (var pool in pools.Values)
				_counts.Add(pool.Name, pool.Resources.Count);

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