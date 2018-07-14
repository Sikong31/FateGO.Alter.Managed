using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace FateGO.Alter.Managed
{
    class AsyncActions : MonoBehaviour
    {
        IEnumerator runningCoroutine = null;
        string message;
        BattlePerformanceCommandCard commandPerf;

        void Awake()
        {
            DontDestroyOnLoad(this);
        }

        private void Update()
        {
            if (runningCoroutine != null)
            {
                MyWaitForSeconds sec = runningCoroutine.Current as MyWaitForSeconds;
                if (sec != null)
                {
                    if (Time.time < sec.m_Seconds)
                    {
                        return;
                    }
                }
                bool result = runningCoroutine.MoveNext();
                if (!result)
                    runningCoroutine = null;
            }
        }

        public void RunInfoMsg(string message)
        {
            if (runningCoroutine == null)
            {
                this.message = message;
                runningCoroutine = coroutineInfoMsg();
            }
        }

        public void RunAutoCard(BattlePerformanceCommandCard commandPerf)
        {
            if (runningCoroutine == null)
            {
                this.commandPerf = commandPerf;
                runningCoroutine = coroutineAutoCard();
            }
        }

        public IEnumerator coroutineInfoMsg()
        {
            GameObject go = GameObject.Find("CombineScene");
            Color prevColor;
            string prevText;
            if (go != null)
            {
                var combineRoot = go.GetComponent<CombineRootComponent>();
                UILabel detailInfoLb = combineRoot.svtCombineCtr.detailInfoLb;
                UIWidget component = detailInfoLb.GetComponent<UIWidget>();
                prevColor = component.color;
                prevText = detailInfoLb.text;
                component.color = Color.white;
                detailInfoLb.text = message;
                yield return new MyWaitForSeconds(2f);

                go = GameObject.Find("CombineScene");
                if (go != null)
                {
                    component.color = prevColor;
                    detailInfoLb.text = prevText;
                }
            }
            this.message = null;
        }

        public IEnumerator coroutineAutoCard()
        {
            int markIndex = -1;
            List<BattleCommandComponent> orders = detectOrder(commandPerf);

            for (int i = 0; i < 10; i++)
            {
                markIndex = -1;
                foreach (BattleCommandComponent bcc in orders)
                {
                    if (!bcc.isSelect())
                    {
                        markIndex = bcc.getMarkIndex();
                        break;
                    }
                }
                if (markIndex == -1)
                {
                    yield break;
                }
                commandPerf.touchCommandCard(markIndex);
                yield return new MyWaitForSeconds(0.25f);
            }
            this.commandPerf = null;
        }

        public List<BattleCommandComponent> detectOrder(BattlePerformanceCommandCard perf)
        {
            List<BattleCommandComponent> result = new List<BattleCommandComponent>();
            try
            {
                List<BattleCommandComponent> bccl = new List<BattleCommandComponent>();
                for (int i = 0; i < perf.p_commandlist.Length; i++)
                {
                    BattleCommandComponent component = perf.p_commandlist[i].GetComponent<BattleCommandComponent>();
                    if (!component.isSelect() && 0 <= component.getMarkIndex())
                    {
                        bccl.Add(component);
                    }
                }
                foreach (BattleCommandComponent item in bccl)
                {
                    //if (BattleCommand.isBLANK(item.getCommandType()) || item.isDontAction)
                    if (BattleCommand.isBLANK(item.getCommandType()) || item.isTreasureDvc() || item.isDontAction)
                    {
                        continue;
                    }
                    if (result.Count < 2) { result.Add(item); }
                    else if (result.Count == 2)
                    {
                        int n1 = result[0].getUniqueID();
                        int n2 = result[1].getUniqueID();
                        int n3 = item.getUniqueID();
                        if (n1 == n2)
                        {
                            if (n1 != n3)
                            {
                                result.Insert(1, item);
                                break;
                            }
                        }
                        else if (n2 == n3)
                        {
                            result.Insert(0, item);
                            break;
                        }
                        else
                        {
                            result.Add(item);
                            break;
                        }
                    }
                }
                if (result.Count < 3)
                {
                    result = bccl.GetRange(0, 3);
                }

            }
            catch (Exception e)
            {
                FGOAlterLog.print(e.ToString());
            }
            return result;
        }
    }
}
