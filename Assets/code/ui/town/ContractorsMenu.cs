using System;
using System.Collections.Generic;
using data.game;
using data.reactive;
using data.refvalues;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ui.town {
public class ContractorsMenu : MonoBehaviour {
#pragma warning disable 0649
	[SerializeField] private CastleManagementService castleService;
	[SerializeField] private RefValue<long> playerWallet;
	[Space(10)]
	[SerializeField] private TMP_Text currentHealthDisplay;
	[SerializeField] private TMP_Text maxHealthDisplay;
	[SerializeField] private Image healthBarMask;
	[SerializeField] private PurchaseButton partialRepairButton;
	[SerializeField] private PurchaseButton fullRepairButton;
	[SerializeField] private PurchaseButton healthUpgradeButton;
	[SerializeField] private double repairCostPerPoint = 0.5;
	[Range(0.1f, 0.5f)]
	[SerializeField] private double partialRepairFactor = 0.2;
	[SerializeField] private long healthPerUpgrade = 20;
	[SerializeField] private long healthUpgradeCostPerLevel = 40;
	[Space(10)]
	[SerializeField] private List<MoatListing> moatListings;
	[Space(10)]
	[SerializeField] private List<TurretListing> turretListings;
	//[SerializeField] private Transform turretSlotDiagramParent; TODO: Replace static diagram with a diagram determined by max base health
	[SerializeField] private TurretPlacementDiagram turretPlacementDiagram;
#pragma warning restore 0649

	private readonly RxExp<long, long> healthInput = RxExp<long>.Passthrough;
	private readonly RxExp<long, long> damageInput = RxExp<long>.Passthrough;
	private readonly RxExp<long, long> fundsInput = RxExp<long>.Passthrough;
	private readonly RxExp<int, int> moatTierInput = RxExp<int>.Passthrough;
	private readonly RxExp<int, int> turretSlotsInput = RxExp<int>.Passthrough;
	private readonly Dictionary<string, RxExp<int, int>> turretCountInputs = new Dictionary<string, RxExp<int, int>>();
	private readonly Dictionary<string, RxExp<int, int>> turretTierInputs = new Dictionary<string, RxExp<int, int>>();
	private readonly RxListPassthrough<OwnedTurret> ownedTurretsInput = new RxListPassthrough<OwnedTurret>();
	private readonly RxListPassthrough<OwnedTurret> equippedTurretsInput = new RxListPassthrough<OwnedTurret>();

	private void Awake() {
		// Hook up health management systems.
		var canRepair = new RxExp<long, bool>(damage => damage > 0);
		canRepair.Attach(damageInput);
		var partialRepairAmnt = new RxExp<long, long>(CalcPartialRepairAmnt);
		partialRepairAmnt.Attach(healthInput);
		var partialRepairCost = new RxExp<long, long, long>(CalcPartialRepairCost);
		partialRepairCost.Attach(partialRepairAmnt, damageInput);
		var fullRepairCost = new RxExp<long, long>(CalcFullRepairCost);
		fullRepairCost.Attach(damageInput);
		var healthUpgradeCost = new RxExp<long, long>(CalcHealthUpgradeCost);
		healthUpgradeCost.Attach(healthInput);

		partialRepairButton.Wire(partialRepairCost, canRepair, fundsInput);
		partialRepairButton.OnPurchase = () => {
			playerWallet.Value.Current -= partialRepairCost.Current;
			castleService.RecordDamage(-partialRepairAmnt.Current);
		};
		fullRepairButton.Wire(fullRepairCost, canRepair, fundsInput);
		fullRepairButton.OnPurchase = () => {
			playerWallet.Value.Current -= fullRepairCost.Current;
			castleService.RecordDamage(-castleService.Damage.Current);
		};
		healthUpgradeButton.Wire(healthUpgradeCost, RxConst.True, fundsInput);
		healthUpgradeButton.OnPurchase = () => {
			playerWallet.Value.Current -= healthUpgradeCost.Current;
			castleService.SetHealth(castleService.Health.Current + healthPerUpgrade);
		};

		healthInput.Bind(health => maxHealthDisplay.text = $"{health:N0}");
		var currentHealth = new RxExp<long, long, long>((health, damage) => health - damage);
		currentHealth.Attach(healthInput, damageInput);
		currentHealth.Bind(health => currentHealthDisplay.text = $"{health:N0}");
		var healthRatio = new RxExp<long, long, float>((max, current) => (float) current / (float) max);
		healthRatio.Attach(healthInput, currentHealth);
		healthRatio.Bind(ratio => healthBarMask.fillAmount = ratio);

		for (var i = 0; i < moatListings.Count; i++) {
			var listing = moatListings[i];
			var moatCost = new RxConst<long>(listing.price);
			var moatTier = i + 1;
			var moatAvailable = new RxExp<int, bool>(currentTier => currentTier == moatTier - 1);

			moatAvailable.Attach(moatTierInput);
			listing.button.Wire(moatCost, moatAvailable, fundsInput);
			listing.button.OnPurchase = () => {
				playerWallet.Value.Current -= moatCost.Current;
				castleService.ChangeMoatTier(moatTier);
			};
		}

		foreach (var listing in turretListings) {
			var type = listing.element.Data.Type;
			var countInput = RxExp<int>.Passthrough;
			countInput.Attach(RxConst.Zero);
			turretCountInputs[type] = countInput;
			var tierInput = RxExp<int>.Passthrough;
			tierInput.Attach(RxConst.Zero);
			turretTierInputs[type] = tierInput;

			var purchaseCost = new RxConst<long>(listing.purchasePrice);
			var available = new RxExp<int, int, bool>((owned, limit) => owned < limit);
			available.Attach(countInput, turretSlotsInput);
			var upgradeCost = new RxExp<int, long>(tier => listing.upgradePrice * tier);
			upgradeCost.Attach(tierInput);
			var canUpgrade = new RxExp<int, bool>(owned => owned > 0);
			canUpgrade.Attach(countInput);

			listing.element.Wire(countInput, tierInput);
			listing.element.PurchaseButton.Wire(purchaseCost, available, fundsInput);
			listing.element.PurchaseButton.OnPurchase = () => {
				playerWallet.Value.Current -= purchaseCost.Current;
				castleService.PurchaseTurret(listing.element.Data);
			};
			listing.element.UpgradeButton.Wire(upgradeCost, canUpgrade, fundsInput);
			listing.element.UpgradeButton.OnPurchase = () => {
				playerWallet.Value.Current -= upgradeCost.Current;
				castleService.UpgradeTurret(type);
			};
		}
		ownedTurretsInput.OnItemAdded += NewTurretAddedAction;

		turretPlacementDiagram.Wire(ownedTurretsInput, equippedTurretsInput);
		turretPlacementDiagram.OnSlotChanged += (slot, turretType) => castleService.EquipTurret(slot, turretType);
	}

	private void OnEnable() {
		healthInput.Attach(castleService.Health);
		damageInput.Attach(castleService.Damage);
		fundsInput.Attach(playerWallet.Value);
		moatTierInput.Attach(castleService.MoatTier);
		turretSlotsInput.Attach(castleService.TurretSlots);
		foreach (var turret in castleService.OwnedTurrets)
			AttachTurretInputs(turret);
		ownedTurretsInput.Attach(castleService.OwnedTurrets);
		equippedTurretsInput.Attach(castleService.EquippedTurrets);
	}

	private void OnDisable() {
		healthInput.Detach();
		damageInput.Detach();
		fundsInput.Detach();
		moatTierInput.Detach();
		turretSlotsInput.Detach();
		foreach (var countInput in turretCountInputs.Values)
			countInput.Detach();
		foreach (var tierInput in turretTierInputs.Values)
			tierInput.Detach();
		ownedTurretsInput.Detach();
		equippedTurretsInput.Detach();
	}

	private long CalcPartialRepairAmnt(long health)
		=> Convert.ToInt64(Math.Ceiling(health * partialRepairFactor));

	private long CalcPartialRepairCost(long maxRepair, long damage)
		=> maxRepair < damage
			? Convert.ToInt64(maxRepair * repairCostPerPoint)
			: Convert.ToInt64(damage * repairCostPerPoint);

	private long CalcFullRepairCost(long damage)
		=> Convert.ToInt64(damage * repairCostPerPoint);

	private long CalcHealthUpgradeCost(long health)
		=> (health - castleService.BaseHealth)
		   / healthPerUpgrade
		   * healthUpgradeCostPerLevel
		   + healthUpgradeCostPerLevel;

	private void AttachTurretInputs(OwnedTurret turret) {
		var type = turret.Data.Type;
		if (turretCountInputs.ContainsKey(type))
			turretCountInputs[type].Attach(turret.Count);
		if (turretTierInputs.ContainsKey(type))
			turretTierInputs[type].Attach(turret.Tier);
	}

	private void NewTurretAddedAction(OwnedTurret turret, int index) {
		Debug.Log("Turret Added");
		AttachTurretInputs(turret);
	}
}

[Serializable]
public struct MoatListing {
	public PurchaseButton button;
	public long price;
}

[Serializable]
public struct TurretListing {
	public TurretStoreElement element;
	public long purchasePrice;
	public long upgradePrice;
}
}