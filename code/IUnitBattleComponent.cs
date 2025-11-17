using Sandbox;
using System;

namespace TacticsRPG;

public interface IBattleUnitComponent
{
	void Initialize(BattleUnitData unitData){}

	void DumpData(BattleUnitData newUnitData){}
}
