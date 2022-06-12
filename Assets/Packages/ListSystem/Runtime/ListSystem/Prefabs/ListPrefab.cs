//#define USECRADAPTIVETEXTUREDOWNLOAD
//#define ODINEDITOR
//#define USETEXTMESHPRO

using System.Collections;

#if USECRADAPTIVETEXTUREDOWNLOAD
using Cradaptive.MultipleTextureDownloadSystem;
#endif
#if ODINEDITOR
using Sirenix.OdinInspector;
#endif
#if USETEXTMESHPRO
using TMPro;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Cradaptive.ListSystem
{
    public class ListPrefab : MonoBehaviour, IListPrefab
    {
        public System.Action<int> OnListItemDestroyed;
        public IListCallback[] listCallback;
#if ODINEDITOR
        [SerializeField]
#endif
        protected bool usePrefabDefaults;

        public enum ActionType
        {
            Button,
            Toggle,
            None
        }

        public enum ImageType
        {
            Texture,
            Sprite,
            None
        }

        public enum IsEnabled
        {
            True,
            False
        }

        private IListData Data { set; get; }
#if ODINEDITOR
        [Header("Images")] [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        protected ImageType currentImageType;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("currentImageType", ImageType.Texture)]
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        public RawImage rawImageDisplay;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("currentImageType", ImageType.Sprite)]
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        public Image normalImageDisplay;
#if ODINEDITOR
        [Header("Actions")] [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        protected ActionType currentActionType;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("currentActionType", ActionType.Button)]
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        public Button onClickButton;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("currentActionType", ActionType.Toggle)]
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        public Toggle toggleButton;

        [Header("Icons")]
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        protected IsEnabled isActiveIconEnabled;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("isActiveIconEnabled", IsEnabled.True)]
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        public GameObject selectedIcon;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        protected IsEnabled isLoadingEnabled;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("isLoadingEnabled", IsEnabled.True)]
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        public GameObject loadingIcon;

        [Header("Text")]
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        protected bool useNameText;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("useNameText")] [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        public
#if USETEXTMESHPRO
            TextMeshProUGUI
#else
            Text
#endif
            nameText;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        protected bool useDateText;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("useDateText")] [ShowIfGroup("usePrefabDefaults"),]
#endif
        public 
#if USETEXTMESHPRO
            TextMeshProUGUI
#else
        Text
#endif 
        dateText;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults")] [ShowIfGroup("usePrefabDefaults"), SerializeField]
#endif
        protected bool useBodyText;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"), ShowIf("useBodyText")]
#endif
        public 
#if USETEXTMESHPRO
            TextMeshProUGUI
#else
            Text
#endif 
            bodyText;
#if ODINEDITOR
        [ShowIfGroup("usePrefabDefaults"),]
#endif
        public ToggleGroup toggleGroup;

        public virtual void Init(IListData listData)
        {
            this.Data = listData;
            ResetDisplay();
            SetListDataOwner(listData as IListDataOwner);
            SetTexture(listData as ITexture);
            SetSprite(listData as ISprite);
            SetButtonAction(listData as IButtonAction);
            SetButtonToggleAction(listData as IToggleAction);
            SetButtonAction(listData as IButtonMethod);
            SetName(listData as IName);
            UpdateListParent(listData);
            UpdateCallbacks(listData);
            SetBody(listData as IBodyText);
            SetDate(listData as IListDate);
#if USECRADAPTIVETEXTUREDOWNLOAD
            DownloadSprite(listData as ICradaptiveTextureOwner);
#endif
        }

        private void UpdateCallbacks(IListData listData)
        {
            listCallback = GetComponents<IListCallback>();
            for (int i = 0; i < listCallback.Length; i++)
                listCallback[i]?.OnInit(listData);
        }

        private void UpdateListParent(IListData listData)
        {
            IListParent listParent = listData as IListParent;
            if (listParent != null && listParent.ListParent != null)
            {
                listParent.ListParent.listPopulated += OnListPopulated;
            }
        }

        protected virtual void ResetDisplay()
        {
            if (rawImageDisplay)
                rawImageDisplay.gameObject.SetActive(false);

            if (normalImageDisplay)
                normalImageDisplay.gameObject.SetActive(false);

            if (selectedIcon)
                selectedIcon.SetActive(false);

            if (nameText)
                nameText.text = "";

            if (loadingIcon)
                loadingIcon.SetActive(false);

            if (onClickButton)
                onClickButton.onClick.RemoveAllListeners();

            if (toggleButton)
                toggleButton.onValueChanged.RemoveAllListeners();
        }

        protected virtual void OnListPopulated(int listCount)
        {
            for (int i = 0; i < listCallback.Length; i++)
                listCallback[i]?.OnListPopulated(listCount);
        }

        protected virtual void SetTexture(ITexture texture)
        {
            if (texture == null || texture.TextureImage == null || rawImageDisplay == null)
                return;

            rawImageDisplay.texture = texture.TextureImage;
            rawImageDisplay.gameObject.SetActive(true);
        }

        protected virtual void SetSprite(ISprite sprite)
        {
            if (sprite == null || sprite.SpriteImage == null || normalImageDisplay == null)
                return;

            SetSprite(sprite.SpriteImage, "");

            HideLoader();
        }
#if USECRADAPTIVETEXTUREDOWNLOAD
        protected virtual void DownloadSprite(ICradaptiveTextureOwner sprite)
        {
            if (sprite == null || normalImageDisplay == null)
                return;

            ShowLoader();
            sprite.OnTextureAvailable = SetSprite;
            CradaptiveTexturesDownloader.QueueForDownload(sprite);
        }
#endif

        protected virtual void SetButtonAction(IButtonAction action)
        {
            if (onClickButton == null || action == null)
                return;

            if (action?.OnButtonClicked == null)
            {
                IListElementClicked elementClicked = GetComponentInParent<IListElementClicked>();
                onClickButton.onClick.AddListener(delegate { elementClicked?.OnListPrefabClicked(Data); });
                return;
            }

            onClickButton.onClick.AddListener(delegate { action.OnButtonClicked?.Invoke(); });
        }

        protected virtual void SetButtonAction(IButtonMethod action)
        {
            if (onClickButton == null || action == null)
                return;

            onClickButton.onClick.AddListener(action.ButtonClicked);
        }

        protected virtual void SetButtonToggleAction(IToggleAction action)
        {
            if (toggleButton == null || action == null)
                return;

            toggleGroup = GetComponentInParent<ToggleGroup>();
            if (toggleGroup)
                toggleButton.group = toggleGroup;

            StartCoroutine(SetToggleState(action));
        }

        private IEnumerator SetToggleState(IToggleAction action)
        {
            yield return new WaitForEndOfFrame();
            toggleButton.isOn = action.CurrentState;
            toggleButton.onValueChanged.AddListener(action.OnToggleClicked);
        }

        protected virtual void SetName(IName name)
        {
            if (name == null || string.IsNullOrEmpty(name.Name) || nameText == null)
                return;

            nameText.text = name.Name;
        }

        protected virtual void SetBody(IBodyText body)
        {
            if (body == null || string.IsNullOrEmpty(body.bodyText) || bodyText == null)
                return;

            bodyText.text = body.bodyText;
        }

        private void SetDate(IListDate listDate)
        {
            if (listDate == null || string.IsNullOrEmpty(listDate.DateString) || dateText == null)
                return;

            dateText.text = listDate.DateString;
        }

        private void SetSprite(Sprite spr, string response)
        {
            HideLoader();
            if (spr == null || normalImageDisplay == null)
            {
                //  Debug.LogError(
                //       $"Image Response {response} spr {spr == null} imageDisplay {normalImageDisplay == null}");

                return;
            }

            normalImageDisplay.sprite = spr;
            if (Data is ISprite sprData)
                sprData.SpriteImage = spr;
            normalImageDisplay.gameObject.SetActive(true);
            normalImageDisplay.preserveAspect = true;
            HideLoader();
        }

        protected virtual void SetListDataOwner(IListDataOwner listDataOwner)
        {
            if (listDataOwner == null) return;
            listDataOwner.listPrefab = this;
        }

        public virtual void OnDestroy()
        {
            OnListItemDestroyed?.Invoke(this.Data.Index);
        }

        public void ShowLoader()
        {
            if (loadingIcon)
                loadingIcon.SetActive(true);
        }

        public void HideLoader()
        {
            if (loadingIcon)
                loadingIcon.SetActive(false);
        }
    }
}