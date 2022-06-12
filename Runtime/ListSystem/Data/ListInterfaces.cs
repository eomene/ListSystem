using System;
using UnityEngine;

namespace Cradaptive.ListSystem
{
    /// <summary>
    /// Base interface for any data that uses the listview, all data must implement this interface
    /// </summary>
    public interface IListData
    {
        int Index { get; set; }
    }

    public interface IListDate
    {
        string DateString { get; }
    }
    public interface IListParent
    {
        ListPopulatorBase ListParent { get; set; }
    }

    public interface IListCallback
    {
        void OnInit(IListData listData);
        void OnListPopulated(int listCount);
    }

    /// <summary>
    /// Used by list element to carryout an action on button click
    /// </summary>
    public interface IButtonAction
    {
        Action OnButtonClicked { get; set; }
    }

    public interface IButtonMethod
    {
        void ButtonClicked();
    }

    /// <summary>
    /// Used by list element to carryout an action on button click
    /// </summary>
    public interface IToggleAction
    {
        void OnToggleClicked(bool state);
        bool CurrentState { get; }
    }
    
    /// <summary>
    /// Holds reference to the main gameobject holding this data in a list
    /// </summary>
    public interface IListDataOwner
    {
        IListPrefab listPrefab { get; set; }
    }

    /// <summary>
    /// Used by list view to identify a prefab holding reference to data
    /// </summary>
    public interface IListPrefab
    {
        GameObject gameObject { get; }
        void Init(IListData listData);
    }

    /// <summary>
    /// Used by list element to display sprite
    /// </summary>
    public interface ISprite
    {
        Sprite SpriteImage { get; set; }
    }

    public interface ITexture
    {
        Texture TextureImage { get; }
    }
    
    public interface IListDestroyedCallback
    {
        System.Action<int> OnListItemDestroyed { get; set; }
    }

    /// <summary>
    /// Used by list element to display names of data
    /// </summary>
    public interface IName
    {
        string Name { get; }
    }
    
    public interface IBodyText
    {
        string bodyText { get; }
    }
    
    public interface IListElementClicked
    {
        void OnListPrefabClicked(IListData listData);
    }
}