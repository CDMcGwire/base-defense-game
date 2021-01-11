using System.IO;
using System.Threading.Tasks;
using combat.components;
using data.refvalues;
using data.service;
using JetBrains.Annotations;
using UnityEngine;

namespace data.game {
[CreateAssetMenu(fileName = "castle-management-service", menuName = "Service/Castle Management", order = 0)]
public class CastleManagementService : DataService<CastleManagementData>, IPersistableService {
#pragma warning disable 0649
	[SerializeField] private long initialHealth = 100;
	[SerializeField] private long initialDamage = 0;
#pragma warning restore 0649

	public IReactiveReadVal<long> Health => Data.Health;
	public IReactiveReadVal<long> Damage => Data.Damage;

	public override void Initialize() {
		Data.Health.Current = initialHealth;
		Data.Damage.Current = initialDamage;
	}

	public void ChangeHealth(int value) {
		if (value < 1) value = 1;
		Data.Health.Current = value;
		// It's expected that systems responsible for declaring the base as
		// destroyed will only be subscribed to the Damage value, thus it updates
		// after.
		if (Data.Damage.Current >= value)
			Data.Damage.Current = value - 1;
	}

	public void RecordDamage(long value)
		=> Data.Damage.Current += value;

	[UsedImplicitly]
	public void RecordDamage(DamageReport report)
		=> RecordDamage(report.damage);

	public async Task WriteLines(StreamWriter stream)
		=> await stream.WriteLineAsync(JsonUtility.ToJson(Data));

	public async Task ReadLines(SectionReader stream)
		=> JsonUtility.FromJsonOverwrite(await stream.ReadLineAsync(), Data);
}
}