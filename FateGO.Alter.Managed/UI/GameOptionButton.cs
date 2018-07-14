using System;
using UnityEngine;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

namespace FateGO.Alter.Managed
{
    public class GameOptionButton : IHeight
    {


        public GameObject goButton;
        public UIButton uiButton;

        public const float _Height = 66;
        public float Height { get { return _Height; } }

        internal UILabel Label
        {
            get { return NGUIUtil.FindChild(this.goButton, "SubmitTxt").GetComponent<UILabel>(); }
        }

        internal GameOptionButton(GameObject parent, string goname)
        {
            string sceneName = SingletonMonoBehaviour<ManagementManager>.Instance.scenemanager.getNowSceneName();
            if (SceneList.getSceneName(SceneList.Type.MyRoom) == sceneName)
            {
                GameObject gop = GameObject.Find(sceneName).transform.FindChild("UI Root/Camera/MasterProfilePanel").gameObject;
                GameObject orig = GameObject.Find(sceneName).transform.FindChild("UI Root/Camera/MasterProfilePanel/ChangeProfileDlg/BaseWindow/SubmitBtn").gameObject;
                GameObject go = NGUIUtil.SetCloneChild(parent, orig, goname);
                this.goButton = go;
                this.uiButton = go.GetComponentInChildren<UIButton>();
                this.uiButton.onClick.Clear();
            }
        }

        internal void SetLocalPosition(float x, float y, float z)
        {
            goButton.transform.localPosition = new Vector3(x, y, z);
        }

        internal void SetOnClick(Action action)
        {
            if (action == null)
                return;
            if (this.uiButton.onClick.Count > 0)
            {
                this.uiButton.onClick.Clear();
            }
            EventDelegate.Set(this.uiButton.onClick, new EventDelegate.Callback(action));
        }

    }
}
