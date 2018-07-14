using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FateGO.Alter.Managed
{

    internal class NGUIUtil
    {
        private string getTag(Component co, int n) { return getTag(co.gameObject, n); }
        private string getTag(GameObject go, int n)
        {
            return (go.name.Split(':') != null) ? go.name.Split(':')[n] : "";
        }

        private const string INDENT_STRING = " ";
        private static StringBuilder sb;

        static NGUIUtil()
        {
            sb = new StringBuilder();
        }

        internal static void GetChildren(GameObject obj, StreamWriter writer, int generation)
        {

            sb.Length = 0;
            for (int i = 0; i < generation; i++)
                sb.Append(INDENT_STRING);

            sb.AppendFormat("[{0}] {1}", generation, obj.name);

            int len = 60 - sb.Length;
            for (int i = 0; i < len; i++)
                sb.Append(' ');



            sb.AppendFormat(" Pos({0,-13}, {1,-13}, {2,-13})"
                , obj.transform.localPosition.x
                , obj.transform.localPosition.y
                , obj.transform.localPosition.z);

            Bounds bounds = NGUIMath.CalculateRelativeWidgetBounds(obj.transform, obj.transform);
            sb.AppendFormat(" Center({0,-13}, {1,-13}, {2,-13})",
                bounds.center.x, bounds.center.y, bounds.center.z);
            sb.AppendFormat(" size({0,-13}, {1,-13}, {2,-13})",
                bounds.size.x, bounds.size.y, bounds.size.z);

            sb.Append(" Components(");

            Component[] cAry = obj.GetComponents<Component>();

            for (int i = 0; i < cAry.Length; i++)
            {
                sb.Append(cAry[i].GetType().ToString());
                if ((i + 1) < cAry.Length)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(")");

            writer.WriteLine(sb.ToString());
            Transform children = obj.GetComponentInChildren<Transform>();
            if (children.childCount == 0)
            {
                return;
            }
            foreach (Transform ob in children)
            {
                GetChildren(ob.gameObject, writer, generation + 1);
            }
        }

        internal static UIAtlas FindAtlas(string s)
        {
            return new List<UIAtlas>(Resources.FindObjectsOfTypeAll<UIAtlas>()).FirstOrDefault<UIAtlas>(delegate (UIAtlas a) {
                return (a.name == s);
            });
        }

        internal static void SetChild(GameObject parent, GameObject child)
        {
            child.layer = parent.layer;
            child.transform.parent = parent.transform;
            child.transform.localPosition = Vector3.zero;
            child.transform.localScale = Vector3.one;
            child.transform.rotation = Quaternion.identity;
        }

        internal static GameObject SetCloneChild(GameObject parent, GameObject orignal, string name)
        {
            GameObject child = UnityEngine.Object.Instantiate(orignal) as GameObject;
            if (child == null)
                return null;
            child.name = name;
            SetChild(parent, child);
            return child;
        }

        internal static void ReleaseChild(GameObject child)
        {
            child.transform.parent = null;
            child.SetActive(false);
        }

        internal static void DestroyChild(GameObject parent, string name)
        {
            GameObject child = FindChild(parent, name);
            if (child)
            {
                child.transform.parent = null;
                GameObject.Destroy(child);
            }
        }

        internal static Transform FindChild(Transform tr, string s) { return FindChild(tr.gameObject, s).transform; }
        internal static GameObject FindChild(GameObject go, string s)
        {
            if (go == null) return null;
            GameObject target = null;

            foreach (Transform tc in go.transform)
            {
                if (tc.gameObject.name == s) return tc.gameObject;
                target = FindChild(tc.gameObject, s);
                if (target) return target;
            }

            return null;
        }

    }
}
