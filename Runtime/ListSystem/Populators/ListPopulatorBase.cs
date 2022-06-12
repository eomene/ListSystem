using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cradaptive.ListSystem
{
    public abstract class ListPopulatorBase : MonoBehaviour
    {
        [SerializeField] protected Transform parent;
        public event System.Action<int> listPopulated;
        protected List<IListData> listData = new List<IListData>();
        private event System.Action<int> listItemDestroyed;
        public abstract void ClearParent();

        public abstract void Populate(IEnumerable<IListData> listData,bool clearParent = true);

        public abstract void AddToList(IEnumerable<IListData> listData);

        public virtual void Populate(IEnumerable<IListData> listData, System.Action<int> onListPopulated, System.Action<int> onListItemDestroyed = null)
        {
            this.listPopulated = onListPopulated;
            this.listItemDestroyed = onListItemDestroyed;
            Populate(listData);
        }

        protected virtual void OnListItemDestroyed(int index)
        {
            RemoveFromList(index);
        }
        
        protected void OnListPopulated(int count)
        {
            listPopulated?.Invoke(count);
        }
        
        public virtual Transform GetParent()
        {
            return parent;
        }

        public virtual int GetListCount()
        {
            return listData.Count;
        }

        public virtual List<IListData> GetListData()
        {
            return listData;
        }

        protected virtual void RemoveFromList(int dataIndex)
        {
            var data = listData.FirstOrDefault(x => x.Index == dataIndex);
            if (data != null)
            {
                listData.Remove(data);
                listItemDestroyed?.Invoke(dataIndex);
            }
        }
    }
}