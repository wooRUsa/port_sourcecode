using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data;

public class SonycastSkillFactory : SkillCardFactory
{ 
    
    public override DataRow makeSkillCardData(SkillDataTable.KeyCode_CardName cardName)
    {
        DataTable dataTable;
        dataTable = SkillDataTable.GetDataFromTable();

        DataRow dataRow = dataTable.Rows.Find(cardName);

        
        //skillCard.SetSkillCardData(dataRow.ItemArray[1].ToString(), dataRow.ItemArray[2].ToString(), dataRow.ItemArray[3].ToString());
        
        
       // Debug.Log("sony cast skill card ");

        // skillcard형으로 안돌려도 될것같은데..
        //결국은 인스턴시에이트 하기 위해서 필요한 데이터만 넘겨주면 되긴 함...
        return dataRow;

        throw new System.NotImplementedException();
    }
}
