namespace DataMinerObjectsStatistics.Resources
{
	using System;
	using System.Collections.Generic;

	using Skyline.DataMiner.Net.Messages;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;

	public static class PoolProvider
	{
		public static Dictionary<Guid, Pool> GetResourcePools(ResourceManagerHelper resourceHelper)
		{
			var pools = new Dictionary<Guid, Pool>();

			// get all resource pools
			var resourcePools = resourceHelper.GetResourcePools();
			foreach (var rp in resourcePools)
			{
				pools.Add(rp.GUID, new Pool(rp.GUID, rp.Name));
			}

			// get all resources
			var allResourcesFilter = new TRUEFilterElement<Resource>();
			var resources = resourceHelper.GetResources(allResourcesFilter);

			foreach (var r in resources)
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