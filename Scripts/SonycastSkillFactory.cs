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

        // skillcard������ �ȵ����� �ɰͰ�����..
        //�ᱹ�� �ν��Ͻÿ���Ʈ �ϱ� ���ؼ� �ʿ��� �����͸� �Ѱ��ָ� �Ǳ� ��...
        return dataRow;

        throw new System.NotImplementedException();
    }
}
