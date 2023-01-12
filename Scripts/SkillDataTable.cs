using System.Data;
using UnityEngine.UI;
using UnityEngine;

public static class SkillDataTable
{
    private static DataTable sonyCastSkillDataTable = new DataTable();
    private static DataTable zzamTigerSkillDataTable = new DataTable();
    private static DataTable kimDoeSkillDataTable = new DataTable();


    public enum KeyCode_CardName
    {
        moveCard,
        attackCard_1,
        attackCard_2,
        attackCard_3,
        guardCard,
        restoreCard
    }

    private static Vector2Int[] haveNoRange =
    {
        new Vector2Int(99,99)
    };

    #region SonyCast_AttackCardData
    private static Vector2Int[] sonyCastAttack1Range =
    {
        new Vector2Int(2,0),
        new Vector2Int(-2,0)
    };
    private static Vector2Int[] sonyCastAttack2Range =
    {
        //십자포
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(0,-1),
        new Vector2Int(-1,0),
        new Vector2Int(1,0)
    };
    #endregion
    public static void SetSkillDataTable()
    {
        //키벨류 , 카드명 , 코스트 , 카드설명 , 카드그림
        sonyCastSkillDataTable.Columns.Add("KeyCode_CardName", typeof(int));
        sonyCastSkillDataTable.Columns.Add("CardName", typeof(string));
        sonyCastSkillDataTable.Columns.Add("Cost", typeof(int));
        sonyCastSkillDataTable.Columns.Add("Damage", typeof(int));
        sonyCastSkillDataTable.Columns.Add("Description", typeof(string));
        sonyCastSkillDataTable.Columns.Add("NeedSelectRange", typeof(bool));
        sonyCastSkillDataTable.Columns.Add("AttackRange", typeof(Vector2Int[]));
        sonyCastSkillDataTable.Columns.Add("Artwork", typeof(Image)); //　

        zzamTigerSkillDataTable.Columns.Add("KeyCode_CardName", typeof(int));
        zzamTigerSkillDataTable.Columns.Add("Description", typeof(string));
        zzamTigerSkillDataTable.Columns.Add("Cost", typeof(int));
        zzamTigerSkillDataTable.Columns.Add("Artwork", typeof(Image)); //　

        kimDoeSkillDataTable.Columns.Add("KeyCode_CardName", typeof(int));
        kimDoeSkillDataTable.Columns.Add("Description", typeof(string));
        kimDoeSkillDataTable.Columns.Add("Cost", typeof(int));
        kimDoeSkillDataTable.Columns.Add("Artwork", typeof(Image)); //　

        sonyCastSkillDataTable.Rows.Add(new object[] { KeyCode_CardName.moveCard, "이름:이동카드", 10, 0, "이동카드설명", true, haveNoRange });
        sonyCastSkillDataTable.Rows.Add(new object[] { KeyCode_CardName.attackCard_1, "할머니 펀치!", 10, 20, "공격카드_1설명", false, sonyCastAttack1Range });
        sonyCastSkillDataTable.Rows.Add(new object[] { KeyCode_CardName.attackCard_2, "아하핫 죽어랏!", 10, 30, "공격카드_2설명", false, sonyCastAttack2Range });
        sonyCastSkillDataTable.Rows.Add(new object[] { KeyCode_CardName.attackCard_3, "이름:공격카드_3", 10, 50, "공격카드_3설명", false, sonyCastAttack2Range });
        sonyCastSkillDataTable.Rows.Add(new object[] { KeyCode_CardName.guardCard, "이름:방어카드", 10, -15, "방어카드설명", false , haveNoRange });
        sonyCastSkillDataTable.Rows.Add(new object[] { KeyCode_CardName.restoreCard, "이름:휴식카드", 10, 0, "휴식카드설명", false , haveNoRange });
        sonyCastSkillDataTable.PrimaryKey = new DataColumn[] { sonyCastSkillDataTable.Columns["KeyCode_CardName"] };
    }

    public static DataTable GetDataFromTable()
    {
        //todo
        DataTable dt = new DataTable();
        dt = sonyCastSkillDataTable;
        return dt;
    }
}
