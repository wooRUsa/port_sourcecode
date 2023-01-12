using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace Project.Game
{
    public class GridManager : MonoBehaviour
    {
        public static GridManager gridManagerInstance = null;
        public static GridManager ReturnStaticInstance()
        {
            return gridManagerInstance;
        }

        [SerializeField]
        private int width, height;
        [Range(3, 5), SerializeField]
        private float tileSizeOffsetX;
        [Range(3, 5), SerializeField]
        private float tileSizeOffsetY;

        [SerializeField]
        private TileData tileDataPrefab;

        [SerializeField]
        private Transform cameraTransform;

        private Dictionary<Vector2Int, TileData> tilesDictionary = new Dictionary<Vector2Int, TileData>();

        private Vector3 moveToPosition;
        public Vector3 GetMoveToPosition
        { get { return moveToPosition; } }
        
        public TileData clickedTileData;

        private void Awake()
        {
            if(gridManagerInstance == null)
            {
                gridManagerInstance = this;
            }
            else
            {
                Destroy(this);
            }

        }

        private void Start()
        {
            GenerateGrid();
        }

        private void GenerateGrid()
        {
            for(int x =0; x < width; x++)
            {
                for(int y =0; y < height; y++)
                {
                    var spawnedTile = Instantiate(tileDataPrefab, new Vector3(x* tileSizeOffsetX, y* tileSizeOffsetY), Quaternion.identity);
                    spawnedTile.name = $"Tile {x} {y} ";
                    spawnedTile.OwnCellPosition = new Vector2Int(x, y);

                    bool isOffset = (x + y) % 2 == 1;
                    spawnedTile.InitializeTileData(isOffset, tileSizeOffsetX, tileSizeOffsetY);

                    tilesDictionary.Add( new Vector2Int(x, y), spawnedTile);
                }
            }

            cameraTransform.GetComponent<Transform>().position = new Vector3((float)width * tileSizeOffsetX / 2 - tileSizeOffsetX / 2, 
                (float)height * tileSizeOffsetY / 2  - tileSizeOffsetY / 2, -10);
            
        }

        public bool CheckAvailableMoveRange(Vector2Int currntCellPos, Vector2Int clickedCellPos)
        {
            if(Mathf.Abs(currntCellPos.x - clickedCellPos.x) <= 1)
            {
                if(Mathf.Abs(currntCellPos.y - clickedCellPos.y) <= 1)
                {
                    return true;
                }
            }    

            Debug.Log("Invalid Range Selected");
            Debug.Log("currnt " + currntCellPos);
            Debug.Log("clicked " + clickedCellPos);
            return false;


        }

        public TileData GetTileDataFromCellpos(Vector2Int pos)
        {
            if(tilesDictionary.TryGetValue(pos, out TileData tile))
            {
                return tile;
            }
            
            return null;
        }


        public Vector2Int GetCellPositionFromVec3(Vector3 position)
        {
            Vector2Int vector2Int = new Vector2Int((int)(Mathf.Ceil(position.x) / tileSizeOffsetX), (int)(Mathf.Ceil(position.y) / tileSizeOffsetY));
            return vector2Int;
        }

        public void SetClickedTileData(TileData tileData = null)
        {
            this.clickedTileData = tileData;
        }
        
    }

}
