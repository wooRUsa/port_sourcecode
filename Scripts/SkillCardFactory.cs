using System.Data;

//��ųī�� �߻� ���丮
public abstract class SkillCardFactory 
{
	public abstract DataRow makeSkillCardData(SkillDataTable.KeyCode_CardName cardName);
	
}

