using UnityEngine;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

namespace FateGO.Alter.Managed
{

    public class GameOptionSlider : IHeight
    {
        private string key;

        public const float _Height = 36;
        public float Height { get { return _Height; } }


        public GameObject container;
        // UISlider取值范围float [0, 1]
        public UISlider slider;

        public EventDelegate eventDelegate;

        private readonly float defaultValue;

        // UISlider没有和value绑定,需要手动设置value
        internal float value
        {
            set { PlayerPrefs.SetFloat(key, value); }
            get { return PlayerPrefs.GetFloat(key, defaultValue); }
        }

        internal UILabel ValueTxtLabel
        {
            get { return NGUIUtil.FindChild(this.container, "ValueTxt").GetComponent<UILabel>(); }
        }

        internal UILabel NameTxtLabel
        {
            get { return NGUIUtil.FindChild(this.container, "NameTxt").GetComponent<UILabel>(); }
        }

        internal GameOptionSlider(GameObject parent, string key, float defaultValue = 0, EventDelegate eventDelegate = null, string goname = null)
        {
            this.key = key;
            this.defaultValue = defaultValue;
            if (goname == null) { goname = key; }

            string sceneName = SingletonMonoBehaviour<ManagementManager>.Instance.scenemanager.getNowSceneName();
            if (SceneList.getSceneName(SceneList.Type.MyRoom) == sceneName)
            {
                GameObject gop = GameObject.Find(sceneName).transform.FindChild("UI Root/Camera/GameOptionPanel").gameObject;
                GameObject bgmSlider = gop.transform.FindChild("Option/Scroll View/SetVolume/SetBgmInfo").gameObject;
                GameObject copy = NGUIUtil.SetCloneChild(parent, bgmSlider, goname);
                container = copy;
                this.slider = copy.GetComponentInChildren<UISlider>();
                if (eventDelegate != null)
                {
                    eventDelegate.parameters[0] = new EventDelegate.Parameter(this);
                    this.slider.onChange.Clear();
                    this.slider.onChange.Add(eventDelegate);
                }
                this.slider.value = this.value;
                
            }
        }

        internal void SetLocalPosition(float x, float y, float z)
        {
            container.transform.localPosition = new Vector3(x, y, z);
        }

        //public void OnSliderChanged()
        //{
        //    if (UISlider.current == this.slider)
        //    {
        //        float newValue = this.slider.value;
        //        bool flag = this.value != newValue;
        //        this.value = newValue;
        //        int num = Mathf.FloorToInt(newValue * 10f);
        //        NGUIUtil.FindChild(this.container, "ValueTxt").GetComponent<UILabel>().text = num.ToString();
        //        if (flag)
        //        {
        //            SoundManager.playSystemSe(SeManager.SystemSeKind.DECIDE);
        //        }
        //    }
        //}
    }

}
