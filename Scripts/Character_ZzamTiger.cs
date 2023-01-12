
using UnityEngine;
using Cysharp.Threading.Tasks;
using Photon.Pun;
namespace Project.Game
{
    public class Character_ZzamTiger : Character_BaseClass
    {
        public override async UniTask MoveCharacter(Vector3 moveToPosition, float duration)
        {
            await UniTask.Yield();
        }


    }
}