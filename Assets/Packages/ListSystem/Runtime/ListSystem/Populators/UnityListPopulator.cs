using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cradaptive.ListSystem
{
    public class UnityListPopulator : ListPopulatorBase
    {
        [SerializeField] protected GameObject prefab;
        [SerializeField] private ScrollView scrollView;

        public override void Populate(IEnumerable<IListData> listData, bool clearParent = true)
        {
            if (clearParent)
                ClearParent();
            this.listData = listData.ToList();
            int i = 0;
            foreach (var data in listData)
            {
                CreateConfigurePrefab(data, i);
                i++;
            }

            OnListPopulated(this.listData.Count);
        }
        
        public void ResetContentPosition()
        {
            //parent.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
          //  parent.position  = new Vector2(parent.position.x,0);
        }

        private void CreateConfigurePrefab(IListData data, int i)
        {
            var listParent = data as IListParent;
            if (listParent != null)
            {
                listParent.ListParent = this;
            }

            data.Index = i;
            var child = Instantiate(prefab, parent);
            if (child == null)
            {
                return;
            }

            var listDestroyedCallback = child.GetComponent<IListDestroyedCallback>();
            ///Parent list should be able to keep track of items left in the list
            if (listDestroyedCallback != null)
                listDestroyedCallback.OnListItemDestroyed += OnListItemDestroyed;
            var listPrefab = child.GetComponent<IListPrefab>();

            listPrefab?.Init(data);
        }


        public override void AddToList(IEnumerable<IListData> listData)
        {
            int i = this.listData.Count;
            this.listData.AddRange(listData.ToList());
            foreach (var data in listData)
            {
                CreateConfigurePrefab(data, i);
                i++;
            }

            OnListPopulated(this.listData.Count);
        }

        public override void ClearParent()
        {
            int childCount = parent.childCount;
            for (int i = 0; i < childCount; i++)
            {
                Transform tr = parent.GetChild(i);
               // GameObject go = tr.GetComponent<ListPrefab>() != null ? tr.gameObject : null;
              //  if (go)
                    Destroy(tr.gameObject);
            }

            listData.Clear();
        }
    }
}