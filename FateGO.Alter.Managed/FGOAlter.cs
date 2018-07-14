using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using WellFired;
using System.Linq;
using PlayerPrefs = PreviewLabs.PlayerPrefs;

namespace FateGO.Alter.Managed
{
    public class FGOAlter
    {
        enum NPLevel { Default, Need1NP, Always100NP, AlwaysFull }

        private static GameObject asyncActions;

        // 伤害倍率1-10，默认1倍
        internal const string KEY_DAMAGERATE = "damageRate";
        // 战斗速度1-5，默认2倍速
        internal const string KEY_BATTLESPEED = "battleSpeed";
        // 3种 [0]默认 [1]获得1NP直接提升到100NP [2]全程满NP(100%) [3]全程满NP
        internal const string KEY_NPLEVEL = "npLevel";
        // 获取更多暴击星
        internal const string KEY_MORECRITICALPOINT = "moreCriticalPoint";
        // 自动战斗
        internal const string KEY_AUTOCOMBAT = "autoCombat";
        // 战斗续行
        internal const string KEY_KEEPALIVE = "keepAlive";
        // 礼装按优先度排序 活动>羁绊
        internal const string KEY_SORTEBYEQUIPMENT = "sortByEquipment";
        // 非好友增加NP
        internal const string KEY_IGNORENPGAUGE = "igonreNPGauge";
        // 节电模式， 关闭一些粒子效果
        internal const string KEY_BATTERYSAVINGMODE = "batterySavingMode";
        internal const string KEY_FORCEDEATH = "forceDeath";
        internal const string KEY_ISINIT2SPEED = "isInit2Speed";
        internal const string KEY_CUSTOMSORT = "customSort";

        // 默认值
        internal const float DEFAULT_DAMAGERATE = 0.1f;
        internal const float DEFAULT_BATTLESPEED = 0.1f;
        internal const float DEFAULT_NPLEVEL = 0.1f;
        internal const bool DEFAULT_MORECRITICALPOINT = true;
        internal const bool DEFAULT_AUTOCOMBAT = false;
        internal const bool DEFAULT_KEEPALIVE = false;
        internal const bool DEFAULT_SORTEBYEQUIPMENT = false;
        internal const bool DEFAULT_IGNORENPGAUGE = false;
        internal const bool DEFAULT_BATTERYSAVINGMODE = false;
        internal const bool DEFAULT_FORCEDEATH = false;
        internal const bool DEFAULT_ISINIT2SPEED = true;
        internal const int DEFAULT_CUSTOMSORT = 0;

        public static float damageRawRate { get { return PlayerPrefs.GetFloat(KEY_DAMAGERATE, DEFAULT_DAMAGERATE); } }
        public static float damageRate { get { return damageRawRate * 9 + 1f; } }
        public static float battleRawSpeed { get { return PlayerPrefs.GetFloat(KEY_BATTLESPEED, DEFAULT_BATTLESPEED); } }
        public static float battleSpeed { get { return battleRawSpeed * 9 + 1f; } }
        public static float npRawlevel { get { return PlayerPrefs.GetFloat(KEY_NPLEVEL, DEFAULT_NPLEVEL); } }
        public static float nplevel { get { return npRawlevel * 3; } }
        public static bool needMoreCriticalPoint { get { return PlayerPrefs.GetBool(KEY_MORECRITICALPOINT, DEFAULT_MORECRITICALPOINT); } }
        public static bool autoCombat { get { return PlayerPrefs.GetBool(KEY_AUTOCOMBAT, DEFAULT_AUTOCOMBAT); } }
        public static bool keepAlive { get { return PlayerPrefs.GetBool(KEY_KEEPALIVE, DEFAULT_KEEPALIVE); } }
        public static bool sortByEquipment { get { return PlayerPrefs.GetBool(KEY_SORTEBYEQUIPMENT, DEFAULT_SORTEBYEQUIPMENT); } }
        public static bool ignoreNPGauge { get { return PlayerPrefs.GetBool(KEY_IGNORENPGAUGE, DEFAULT_IGNORENPGAUGE); } }
        public static bool batterySavingMode { get { return PlayerPrefs.GetBool(KEY_BATTERYSAVINGMODE, DEFAULT_BATTERYSAVINGMODE); } }
        public static bool forceDeath { get { return PlayerPrefs.GetBool(KEY_FORCEDEATH, DEFAULT_FORCEDEATH); } }
        public static bool isInit2Spped { get { return PlayerPrefs.GetBool(KEY_ISINIT2SPEED, DEFAULT_ISINIT2SPEED); } }
        public static int speedmode { get { return isInit2Spped ? 2 : 1; } }
        public static float thresholdNP {
            get {
                NPLevel level = (NPLevel)nplevel;
                return NPLevel.Need1NP == level ? 0.1f : 0.99f;
            }
        }

        public static int extraCriticalPoint { get { return needMoreCriticalPoint ? UnityEngine.Random.Range(46, 62) : 0; } }

        public static int customSortValue
        {
            get { return PlayerPrefs.GetInt(KEY_CUSTOMSORT, DEFAULT_CUSTOMSORT); }
            set { PlayerPrefs.SetInt(KEY_CUSTOMSORT, value); }
        }

        static FGOAlter()
        {
            asyncActions = new GameObject();
            asyncActions.AddComponent<AsyncActions>();
#if DATAMINE
            resourceDumper = new ResourceDumper();
#endif
        }

        // 在GameOptions后加上自定义UI
        // SetGameOptionComponent::ShowGameOption
        public static void AddAlterOption()
        {
            GameObject gameOptionPanel = GameObject.Find("GameOptionPanel");
            if (!gameOptionPanel)
            {
                FGOAlterLog.print("错误！未能找到GameOptionPanel");
                return;
            }
            try
            {
                GameObject scrollView = gameOptionPanel.transform.FindChild("Option/Scroll View").gameObject;
                if (!scrollView)
                {
                    FGOAlterLog.print("错误！未能找到Scroll View");
                    return;
                }
                string componetName = "AlterOption";
                GameObject ac = NGUIUtil.FindChild(scrollView, componetName);
                if (!ac)
                {
                    ac = new GameObject(componetName);
                    NGUIUtil.SetChild(scrollView, ac);
                    AlterOptionComponet child = ac.AddComponent<AlterOptionComponet>();
                    child.gameOptionPanel = gameOptionPanel;
                    child.scrollView = scrollView;
                    child.font = gameOptionPanel.GetComponentsInChildren<UILabel>()[0].trueTypeFont;
                    ac.transform.localPosition = new Vector3(0f, -1000f, 0);
                    ac.AddComponent<UIDragScrollView>().scrollView = scrollView.GetComponent<UIScrollView>();
                }
            }
            catch (Exception e)
            {
                FGOAlterLog.print(e.ToString());
            }

        }

        // 自动战斗包括点击战斗的策略部分以及选卡
        // endMoveCard
        public static void SelectCard(BattlePerformanceCommandCard commandPerf)
        {
            if (autoCombat)
            {
                asyncActions.GetComponent<AsyncActions>().RunAutoCard(commandPerf);
            }
        }

        // BattleServantData::addNP
        public static void useNoble(BattleServantData svt)
        {
            if (ignoreNPGauge && svt.followerType != Follower.Type.FRIEND)
            {
                svt.followerType = Follower.Type.FRIEND;
            }
        }

        public static void KeepHP(BattleServantData data)
        {
            if (keepAlive && !data.isEnemy && data.hp == 0)
            {
                data.hp = UnityEngine.Random.Range(5, 8) * 100;
            }

        }

        public static string skipTactical(BattleLogic logic, string endproc)
        {
            if (logic.isTutorial())
                return endproc;

            if (autoCombat)
            {
                return "SKIP";
            }
            else
            {
                BattleServantData[] svts = logic.data.getFieldPlayerServantList();
                for (int i = 0; i < svts.Length; i++)
                {
                    BattleServantData svt = svts[i];
                    if (svt.isAlive() && !svt.isTDSeraled() && svt.followerType != Follower.Type.NOT_FRIEND)
                    {
                        NPLevel level = (NPLevel)nplevel;
                        if (NPLevel.Always100NP == level && svt.np < svt.lineMaxNp)
                        {
                            svt.addNp(svt.lineMaxNp);
                        }
                        if (NPLevel.AlwaysFull == level)
                        {
                            svt.addNp(svt.getMaxNp());
                        }
                    }
                }
            }
            return endproc;
        }

        // 列表排序
        public static long SortEquipments(ListViewSort.SortKind kind, FollowerInfo followerInfo, int followerIndex, long sortValue2)
        {
            var leaderinfo = followerInfo.getServantLeaderInfo(followerIndex);
            long score = (leaderinfo != null) ? (leaderinfo.lv << 4 | leaderinfo.limitCount) : 0;
            int esvtid = followerInfo.getEquipSvtId(followerIndex);
            if (esvtid > 0)
            {
                bool totsu = followerInfo.getEquipLimitCount(followerIndex) == 4;
                if (ListViewSort.SortKind.COST == kind)
                {
                    SkillDetailEntity sdent = SingletonMonoBehaviour<DataManager>.Instance.getMasterData(DataNameKind.Kind.SKILL_DETAIL).getEntityFromId<SkillDetailEntity>(followerInfo.getEquipSkillId(followerIndex));
                    if (sdent != null && sdent.detail.Contains("活动期间限定"))
                    {
                        ServantLimitEntity slent = SingletonMonoBehaviour<DataManager>.Instance.getMasterData(DataNameKind.Kind.SERVANT_LIMIT).getEntityFromId<ServantLimitEntity>(esvtid, followerInfo.getEquipLimitCount(followerIndex));
                        long rare = slent.rarity; // 最大8(4bit)为止
                        if (customSortValue > 0 && customSortValue == esvtid)
                            rare = 8;

                        if (customSortValue != 1)
                        {
                            // 稀有优先
                            score |= rare << 13 | Convert.ToInt64(totsu) << 12;
                        }
                        else
                        {
                            // 突破优先
                            score |= rare << 12 | Convert.ToInt64(totsu) << 16;
                        }
                    }
                    sortValue2 = score;
                }
                else if (kind == ListViewSort.SortKind.CLASS)
                {
                    score |= Convert.ToInt64(totsu) << 12;
                    ServantEntity sent = SingletonMonoBehaviour<DataManager>.Instance.getMasterData(DataNameKind.Kind.SERVANT).getEntityFromId<ServantEntity>(esvtid);

                    if (sent.name.Contains("英灵肖像")) // 絆礼装
                    {
                        score |= 6 << 13;
                    }
                    if (sent.id == 9400360) // 旅途的开始
                    {
                        score |= 5 << 13;
                    }
                    if (sent.id == 9401710 && totsu) // 个人练习 礼装EXP10%
                    {
                        score |= 4 << 13;
                    }
                    if (sent.id == 9400780 && totsu) // 蒙娜丽莎 QP10%
                    {
                        score |= 3 << 13;
                    }
                    if (sent.id == 9401340 && totsu) // パーソナル・トレーニング凸 マスターEXP10%
                    {
                        score |= 2 << 13;
                    }
                    else if (sent.id == 9400340) // カレイドスコープ
                    {
                        score |= 1 << 13;
                    }
                    sortValue2 = score;
                }
            }
            return sortValue2;
        }


        // make BattlePerformance::LateUpdate and call this at function
        public static void KillBattleParticle()
        {
            var ground = GameObject.Find("Management/Scene/ActiveObject/BattleScene/Performance/Ground");
            if (ground)
            {
                foreach (var renderer in ground.GetComponentsInChildren<Renderer>())
                {
                    if (renderer is ParticleRenderer || renderer is ParticleSystemRenderer)
                    {
                        renderer.enabled = false;
                    }
                }
            }
        }

        public static void StopFieldParticle(GameObject bg, GameObject front)
        {
            if (bg)
            {
                bg.GetComponentsInChildren<ParticleSystem>().ForEach(psys => psys.Stop());
            }
            if (front) 
            {
                front.GetComponentsInChildren<ParticleSystem>().ForEach(psys => psys.Stop());
            }
        }

        // BattleLogic::setTimeAcceleration() this.prevScaleTime
        public static float getPrevScaleTime()
        {
            float val = Mathf.Max(Mathf.Ceil(battleSpeed), 1f);
            return (val > 1f) ? val : Time.timeScale;
        }

        // BattleLogic::setTimeAcceleration() Time.timeScale
        public static float getTimeScale()
        {
            float val = Mathf.Max(Mathf.Ceil(battleSpeed), 1f);
            return (val > 2f) ? val : 2f;
        }

        public static int ForceInstantDeath(int num2, BattleServantData actor)
        {
            return (actor.checkPlayer() && forceDeath && num2 > 0) ? 1000 : num2;
        }
    }
}
