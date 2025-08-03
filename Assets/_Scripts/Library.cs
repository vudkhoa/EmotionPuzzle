namespace CustomUtils
{
    using System.Collections.Generic;
    using UnityEngine;

    public class Library : SingletonMono<Library>
    {
        public List<Vector2Int> LibOffsets8;

        private void Start()
        {
            this.LibOffsets8 = new List<Vector2Int>();
            this.LibOffsets8 = GetOffset8();
        }

        public List<Vector2Int> GetOffset8()
        {
            LibOffsets8 = new List<Vector2Int>();
            for (int i = -1; i <= 1; ++i)
            {
                for (int j = -1; j <= 1; ++j)
                {
                    if ((i == 0 && j == 0))
                    {
                        continue;
                    }
                    Vector2Int pos = new Vector2Int(i, j);
                    LibOffsets8.Add(pos);
                }
            }
            return LibOffsets8;
        }
    }
}


