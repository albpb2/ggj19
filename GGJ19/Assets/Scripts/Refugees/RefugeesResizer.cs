using UnityEngine;

namespace Assets.Scripts.Refugees
{
    public class RefugeesResizer
    {
        public void ResizeRefugeeBasingOnSortingLayer(Refugee refugee)
        {
            var spriteRenderer = refugee.GetComponent<SpriteRenderer>();
            switch(spriteRenderer.sortingLayerName)
            {
                case SortingLayers.Floor2:
                    refugee.transform.localScale *= 0.94f;
                    return;
                case SortingLayers.Floor3:
                    refugee.transform.localScale *= 0.86f;
                    return;
                default:
                    return;
            }
        }
    }
}
