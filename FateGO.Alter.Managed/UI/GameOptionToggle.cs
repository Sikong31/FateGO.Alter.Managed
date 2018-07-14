using UnityEngine;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

namespace FateGO.Alter.Managed
{
    public class GameOptionToggle : IHeight
    {
        private string key;

        internal bool Value
        {
            set { PlayerPrefs.SetBool(key, value); }
            get { return PlayerPrefs.GetBool(key, DefaultValue); }
        }

        public const float _Height = 62;
        public float Height { get { return _Height; } }

        public GameObject goButton;
        public UIButton uiButton;

        private readonly bool DefaultValue;

        internal UILabel NameTxtLabel
        {
            get { return NGUIUtil.FindChild(this.goButton, "NameTxt").GetComponent<UILabel>(); }
        }

        internal GameOptionToggle(GameObject parent, string prefKey, bool defaultValue = false, string goname = null)
        {
            this.DefaultValue = defaultValue;
            if (goname == null)
            {
                goname = prefKey;
            }
            key = prefKey;
            string sceneName = SingletonMonoBehaviour<ManagementManager>.Instance.scenemanager.getNowSceneName();
            if (SceneList.getSceneName(SceneList.Type.MyRoom) == sceneName)
            {
                GameObject gop = GameObject.Find(sceneName).transform.FindChild("UI Root/Camera/GameOptionPanel").gameObject;
                GameObject orig = gop.transform.FindChild("Option/Scroll View/SetNotice/ApRecovered/NoticeAp").gameObject;
                GameObject go = NGUIUtil.SetCloneChild(parent, orig, goname);
                this.goButton = go;
                this.uiButton = go.GetComponentInChildren<UIButton>();
                EventDelegate.Set(this.uiButton.onClick, new EventDelegate.Callback(this.OnChangeBtn));
                initButtonState();
            }
        }

        internal void SetLocalPosition(float x, float y, float z)
        {
            goButton.transform.localPosition = new Vector3(x, y, z);
        }

        private void initButtonState()
        {
            if (this.Value)
                this.uiButton.normalSprite = "btn_on";
            else
                this.uiButton.normalSprite = "btn_off";
        }

        public void OnChangeBtn()
        {
            if (UIButton.current == this.uiButton)
            {
                if (this.Value)
                {
                    SoundManager.playSystemSe(SeManager.SystemSeKind.CANCEL);
                    this.uiButton.normalSprite = "btn_off";
                    this.Value = false;
                }
                else
                {
                    SoundManager.playSystemSe(SeManager.SystemSeKind.DECIDE);
                    this.uiButton.normalSprite = "btn_on";
                    this.Value = true;
                }
            }
        }
    }

}
