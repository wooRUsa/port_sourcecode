using System.Data;

//스킬카드 추상 팩토리
public abstract class SkillCardFactory 
{
	public abstract DataRow makeSkillCardData(SkillDataTable.KeyCode_CardName cardName);
	
}

