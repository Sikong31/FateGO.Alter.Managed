using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using PlayerPrefs = PreviewLabs.PlayerPrefs;
using System.Linq;

namespace FateGO.Alter.Managed
{
    class AlterOptionComponet: MonoBehaviour
    {
        public GameObject gameOptionPanel;
        public GameObject scrollView;
        public Font font;

        float accruedY = -70f;
        float verMargin = 50f;

        void Start()
        {
            AddHeader();
            addOption();
            AddFooter();

            Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(gameObject.transform, gameObject.transform);
            BoxCollider collider = gameObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(bounds.center.x - 11, bounds.center.y);
            collider.size = new Vector3(ManagerConfig.WIDTH - 22, bounds.size.y);

            scrollView.GetComponent<UIScrollView>().ResetPosition();
        }

        void AddHeader()
        {
            GameObject goTitleImg = NGUIUtil.SetCloneChild(gameObject, scrollView.transform.FindChild("SetVolume/TitleImg").gameObject, "TitleImg");

            NGUIUtil.DestroyChild(goTitleImg, "TitleTxt");
            UILabel label = NGUITools.AddChild<UILabel>(goTitleImg);
            label.name = "Title";
            label.trueTypeFont = font;
            label.fontSize = 30;
            label.text = "额外功能";
            label.width = 400;
            label.overflowMethod = UILabel.Overflow.ShrinkContent;
            label.transform.localPosition = new Vector3(0f, 20f, 0f);
        }
        void addOption()
        {
            addSlider(FGOAlter.KEY_DAMAGERATE, FGOAlter.DEFAULT_DAMAGERATE, "ATK", 10, "OnAtkSliderChanged");
            //addSlider(FGOAlter.KEY_BATTLESPEED, FGOAlter.DEFAULT_BATTLESPEED, "速度", 10, "OnAtkSliderChanged");
            addSlider(FGOAlter.KEY_NPLEVEL, FGOAlter.DEFAULT_NPLEVEL, "NP获取", 4, "OnNPLevelSliderChanged");

            addToggle(FGOAlter.KEY_ISINIT2SPEED, FGOAlter.DEFAULT_ISINIT2SPEED, "默认系统2倍速");
            //addToggle(FGOAlter.KEY_MORECRITICALPOINT, FGOAlter.DEFAULT_MORECRITICALPOINT, "更多暴击星");
            addToggle(FGOAlter.KEY_AUTOCOMBAT, FGOAlter.DEFAULT_AUTOCOMBAT, "自动战斗");
            //addToggle(FGOAlter.KEY_FORCEDEATH, FGOAlter.DEFAULT_FORCEDEATH, "100%即死");
            addToggle(FGOAlter.KEY_KEEPALIVE, FGOAlter.DEFAULT_KEEPALIVE, "血量控制");
            //addToggle(FGOAlter.KEY_IGNORENPGAUGE, FGOAlter.DEFAULT_IGNORENPGAUGE, "嫖非好友宝具");
        }

        private void AddFooter()
        {
            UILabel label = NGUITools.AddChild<UILabel>(gameObject);
            label.gameObject.name = "footerMarginLabel";
            label.trueTypeFont = font;
            label.text = "　";
            label.transform.localPosition = new Vector3(0, GetYAfterIncrement2(label.height), 0);
        }

        private void addSlider(string key, float defaultValue, string name, int numberOfSteps, string actionName)
        {
            EventDelegate onChangeEvent = new EventDelegate(this, actionName);
            GameOptionSlider opSlider = new GameOptionSlider(gameObject, key, defaultValue, onChangeEvent);
            opSlider.SetLocalPosition(0f, GetYAfterIncrement(opSlider), 0f);
            opSlider.slider.numberOfSteps = numberOfSteps;
            opSlider.NameTxtLabel.text = name;
        }

        private void addToggle(string key, bool defaultValue, string name)
        {
            GameOptionToggle toggle = new GameOptionToggle(gameObject, key, defaultValue);
            toggle.SetLocalPosition(0f, GetYAfterIncrement(toggle), 0f);
            toggle.NameTxtLabel.text = name;
        }

        private float GetYAfterIncrement(IHeight parts)
        {
            float prevY = accruedY;
            accruedY -= parts.Height + verMargin;
            return prevY;
        }
        private float GetYAfterIncrement2(float height)
        {
            float prevY = accruedY;
            accruedY -= height + verMargin;
            return prevY;
        }

        void OnEnable()
        {
            PlayerPrefs.Flush();
        }

        void OnDisable()
        {
            PlayerPrefs.Flush();
        }

        public void OnAtkSliderChanged(GameOptionSlider option)
        {
            if (option.slider != UISlider.current) { return; }
            
            float value = option.slider.value;
            
            bool flag = option.value != value;
            option.value = value;

            int num = Mathf.CeilToInt(value * (option.slider.numberOfSteps - 1)) + 1;
            
            option.ValueTxtLabel.text = "x" + num;
        }

        public void OnNPLevelSliderChanged(GameOptionSlider option)
        {
            if (option.slider != UISlider.current) { return; }
            
            float value = option.slider.value;
            bool flag = option.value != value;
            option.value = value;
            /*  */
            int num = Mathf.CeilToInt(option.slider.value * (option.slider.numberOfSteps - 1));
            string s;
            switch (num)
            {
                case 1:
                    s = "Lv1";
                    break;
                case 2:
                    s = "Lv2";
                    break;
                case 3:
                    s = "Lv3";
                    break;
                default:
                    s = "默认";
                    break;
            }
            option.ValueTxtLabel.text = s;
        }
    }
}
