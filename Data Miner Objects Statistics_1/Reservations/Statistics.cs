namespace DataMinerObjectsStatistics.Reservations
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Net.Apps.DataMinerObjectModel;
	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.Net.ResourceManager.Objects;

	[GQIMetaData(Name = "Total number of reservation instances")]
	public class ReservationInstancesCount : IGQIDataSource, IGQIOnInit
	{
		private long _total = 0;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			var resourceManager = new ResourceManagerHelper(args.DMS.SendMessage);

			_total = resourceManager.CountReservationInstances(ReservationInstanceExposers.Name.NotEqual(string.Empty));

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

	[GQIMetaData(Name = "Number of reservation instances per state")]
	public class ReservationInstancesPerStateCount : IGQIDataSource, IGQIOnInit
	{
		private readonly Dictionary<ReservationStatus, long> _counts = new Dictionary<ReservationStatus, long>();

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			var resourceManager = new ResourceManagerHelper(args.DMS.SendMessage);

			foreach (var value in Enum.GetValues(typeof(ReservationStatus)))
				_counts.Add((ReservationStatus)value, resourceManager.CountReservationInstances(ReservationInstanceExposers.Status.Equal((int)value)));

			return default;
		}

		public GQIColumn[] GetColumns()
		{
			return new GQIColumn[2]
			{
				new GQIStringColumn("State"),
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