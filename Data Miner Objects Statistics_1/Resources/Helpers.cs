namespace DataMinerObjectsStatistics.Resources
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;

	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	public static class PoolProvider
	{
		private static readonly object _lock = new object();
		private static Task<Dictionary<Guid, Pool>> _poolsTask;

		public static Task<Dictionary<Guid, Pool>> GetResourcePoolsAsync(ResourceManagerHelper resourceHelper)
		{
			lock (_lock)
			{
				if (_poolsTask is null)
				{
					_poolsTask = Task.Run(() => GetResourcePools(resourceHelper));
				}
			}

			return _poolsTask;
		}

		private static Dictionary<Guid, Pool> GetResourcePools(ResourceManagerHelper resourceHelper)
		{
			var pools = new Dictionary<Guid, Pool>();

			var allResourcesFilter = new TRUEFilterElement<Resource>();
			var allResources = new HashSet<Resource>();

			// get all resource pools
			var resourcePools = resourceHelper.GetResourcePools();
			foreach (var rp in resourcePools)
			{
				pools.Add(rp.GUID, new Pool(rp.GUID, rp.Name));
				allResources.AddRange(resourceHelper.GetResources(ResourceExposers.PoolGUIDs.Contains(rp.GUID)));
			}

			foreach (var r in allResources)
			{
				foreach (var pg in r.PoolGUIDs)
				{
					if (pools.TryGetValue(pg, out var pool))
					{
						var resource = new PoolResource(r.GUID, r.Name);
						pool.Resources.Add(r.GUID, resource);
					}
				}
			}

			return pools;
		}
	}

	public static class Extensions
	{
		public static void AddRange(this HashSet<Resource> collection, Resource[] range)
		{
			foreach (var item in range)
			{
				collection.Add(item);
			}
		}
	}

	public class Pool
	{
		public Pool(Guid guid, string name)
		{
			Guid = guid;
			Name = name;
		}

		public Guid Guid { get; }

		public string Name { get; }

		public Dictionary<Guid, PoolResource> Resources { get; } = new Dictionary<Guid, PoolResource>();
	}

	public class PoolResource
	{
		public PoolResource(Guid guid, string name)
		{
			Guid = guid;
			Name = name;
		}

		public Guid Guid { get; }

		public string Name { get; }
	}
}