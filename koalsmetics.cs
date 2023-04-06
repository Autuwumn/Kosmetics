using System;
using System.IO;
using System.Linq;
using System.Resources;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using System.Threading;
using BepInEx.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Jotunn.Utils;
using R3DCore.Menu;
using UnityEngine.UI;

namespace KWC
{
    [BepInDependency("com.Root.Menu", BepInDependency.DependencyFlags.HardDependency)]
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("ROUNDS 3D.exe")]
    [HarmonyPatch]
    public class KWC : BaseUnityPlugin
    {
        private const string ModId = "koala.wonderous.cosmetics";
        private const string ModName = "KWC";
        public const string Version = "1.0.0";

        public static List<GameObject> eyes = new List<GameObject>();
        public static List<GameObject> mouths = new List<GameObject>();
        public AssetBundle eyebun;
        public AssetBundle mouthbun;
        public AssetBundle menubun;

        private Slider eyeSlide;
        private Slider mouthSlide;
        private Toggle box;

        public static bool setup = false;

        public int[] cosmeticIds = new[] { 25, 18, 0, 0};
        public bool cosmeticBox = false;
        public static KWC instance { get; private set; }
        public static Mesh squareMesh = new Mesh();
        public static Color[] skinColors = new[] {
            new Color(0.6392157f, 0.2862745f, 0.1686275f, 1f), // Orange
            new Color(0.1647059f, 0.3098039f, 0.5843138f, 1f), // Blue
            new Color(0.6313726f, 0.2705882f, 0.2705882f, 1f), // Red
            new Color(0.2627451f, 0.5372549f, 0.3254902f, 1f), // Green
            new Color(0.6235294f, 0.6392157f, 0.172549f, 1f), // Yellow
            new Color(0.3607843f, 0.172549f, 0.6392157f, 1f), // Purple
            new Color(0.6392157f, 0.172549f, 0.3960784f, 1f), // Magenta
            new Color(0.172549f, 0.6392157f, 0.6117647f, 1f), // Cyan
            new Color(0.6392157f, 0.3607843f, 0.2705882f, 1f), // Tangerine
            new Color(0.254902f, 0.3686275f, 0.5843138f, 1f), // Light blue
            new Color(0.6313726f, 0.3686275f, 0.3686275f, 1f), // Peach
            new Color(0.345098f, 0.5372549f, 0.3882353f, 1f), // Lime
            new Color(0.627451f, 0.6392157f, 0.3764706f, 1f), // Light Yellow
            new Color(0.4196078f, 0.2745098f, 0.6392157f, 1f), // Orchid
            new Color(0.6392157f, 0.2745098f, 0.4470588f, 1f), // Pink
            new Color(0.2745098f, 0.6392157f, 0.6156863f, 1f), // Aquamarine
            new Color(0.4392157f, 0.1960784f, 0.1137255f, 1f), // Dark Orange
            new Color(0.1058824f, 0.2f, 0.3843137f, 1f), // Dark Blue
            new Color(0.4313726f, 0.1843137f, 0.1843137f, 1f), // Dark Red
            new Color(0.1647059f, 0.3372549f, 0.2039216f, 1f), // Dark Green
            new Color(0.427451f, 0.4392157f, 0.1176471f, 1f), // Dark Yellow
            new Color(0.2470588f, 0.1176471f, 0.4392157f, 1f), // Indigo
            new Color(0.4392157f, 0.1176471f, 0.2705882f, 1f), // Cerise
            new Color(0.1176471f, 0.4392157f, 0.4196078f, 1f), // Teal
            new Color(0.4392157f, 0.2470588f, 0.1843137f, 1f), // Burnt Orange
            new Color(0.1647059f, 0.2392157f, 0.3843137f, 1f), // Midnight Blue
            new Color(0.4313726f, 0.2509804f, 0.2509804f, 1f), // Maroon
            new Color(0.2156863f, 0.3372549f, 0.2431373f, 1f), // Evergreen
            new Color(0.427451f, 0.4392157f, 0.254902f, 1f), // Gold
            new Color(0.2862745f, 0.1882353f, 0.4392157f, 1f), // Violot
            new Color(0.4392157f, 0.1882353f, 0.3058824f, 1f), // Ruby
            new Color(0.1882353f, 0.4392157f, 0.4196078f, 1f) // Dark Cyan
        };
        public static void GenerateSquare()
        {
            squareMesh.name = "KoalasCube";
            float size = 0.5f;
            Vector3[] vertices = {
                new Vector3(-size, -size, -size),
                new Vector3( size, -size, -size),
                new Vector3( size,  size, -size),
                new Vector3(-size,  size, -size),
                new Vector3(-size,  size,  size),
                new Vector3( size,  size,  size),
                new Vector3( size, -size,  size),
                new Vector3(-size, -size,  size),
            };
            int[] triangles = {
                0, 2, 1,
                0, 3, 2,
                2, 3, 4,
                2, 4, 5,
                1, 2, 5,
                1, 5 ,6,
                0, 7, 4,
                0, 4, 3,
                5, 4, 7,
                5, 7, 6,
                0, 6, 7,
                0, 1, 6
            };
            Vector2[] uvs = {
                new Vector2(0, 0.66f),
                new Vector2(0.25f, 0.66f),
                new Vector2(0, 0.33f),
                new Vector2(0.25f, 0.33f),

                new Vector2(0.5f, 0.66f),
                new Vector2(0.5f, 0.33f),
                new Vector2(0.75f, 0.66f),
                new Vector2(0.75f, 0.33f),
            };
            squareMesh.vertices = vertices;
            squareMesh.triangles = triangles;
            squareMesh.uv = uvs;
            squareMesh.RecalculateNormals();
        }
        private void Awake()
        {
            instance = this;
            instance.cosmeticIds = new[] { (int)PlayerPrefs.GetFloat("PlayerEye", 25), (int)PlayerPrefs.GetFloat("PlayerMouth", 18), 0, 0 };
            var box2 = false;
            if (PlayerPrefs.GetInt("PlayerBox", 0) == 1) box2 = true;
            instance.cosmeticBox = box2;
            new Harmony(ModId).PatchAll();
            eyebun = AssetUtils.LoadAssetBundleFromResources("eyes", typeof(KWC).Assembly);
            mouthbun = AssetUtils.LoadAssetBundleFromResources("mouths", typeof(KWC).Assembly);
            menubun = AssetUtils.LoadAssetBundleFromResources("menu", typeof(KWC).Assembly);
            eyes.Add(new GameObject());
            mouths.Add(new GameObject());
            var menuObj = menubun.LoadAsset<GameObject>("KosmeticsMenu");
            MenuHandler.ResgesterMenu(menuObj.GetComponent<Canvas>(), MenuHandler.MenuType.Mod, null);
            foreach (var obj in eyebun.LoadAllAssets<GameObject>())
            {
                eyes.Add(obj);
            }
            foreach (var obj in mouthbun.LoadAllAssets<GameObject>())
            {
                mouths.Add(obj);
            }
            GenerateSquare();
        }
        private void FixedUpdate()
        {
            if (MenuHandler.instance.ActiveMenu == 4)
            {
                eyeSlide = GameObject.Find("KosmeticsMenu").transform.Find("Eyes").gameObject.GetComponent<Slider>();
                mouthSlide = GameObject.Find("KosmeticsMenu").transform.Find("Mouths").gameObject.GetComponent<Slider>();
                box = GameObject.Find("KosmeticsMenu").transform.Find("BoxMode").gameObject.GetComponent<Toggle>();
                if (!setup)
                {
                    eyeSlide.value = PlayerPrefs.GetFloat("PlayerEye", 25);
                    mouthSlide.value = PlayerPrefs.GetFloat("PlayerMouth", 18);
                    var box2 = false;
                    if (PlayerPrefs.GetInt("PlayerBox", 0) == 1) box2 = true;
                    box.isOn = box2;
                    setup = true;
                }
                else
                {
                    PlayerPrefs.SetFloat("PlayerEye", eyeSlide.value);
                    PlayerPrefs.SetFloat("PlayerMouth", mouthSlide.value);
                    var box2 = 0;
                    if (box.isOn == true) box2 = 1;
                    PlayerPrefs.SetInt("PlayerBox", box2);
                    instance.cosmeticIds[0] = (int)eyeSlide.value;
                    instance.cosmeticIds[1] = (int)mouthSlide.value;
                    instance.cosmeticBox = box.isOn;
                }
                
            if (box.isOn == true)
            {
                GameObject.Find("KosmeticsMenu").transform.Find("Player").GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                GameObject.Find("KosmeticsMenu").transform.Find("Player").GetChild(1).gameObject.SetActive(false);
            }
            var e = GameObject.Find("KosmeticsMenu").transform.Find("Player").Find("EyeObj");
            for (var i = 0; i < e.childCount; i++)
            {
                Destroy(e.GetChild(i).gameObject);
            }
            Instantiate(eyes[(int)eyeSlide.value], e);
            var m = GameObject.Find("KosmeticsMenu").transform.Find("Player").Find("MouthObj");
            for (var i = 0; i < m.childCount; i++)
            {
                Destroy(m.GetChild(i).gameObject);
            }
            Instantiate(mouths[(int)mouthSlide.value], m);
            } else
            {
                setup = false;
            }
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Connection), "OnPlayerEnteredRoom")]
        public static void PatchOnPlayerEntered()
        {
            foreach (var p in FindObjectsOfType<Player>())
            {
                if(p.gameObject.GetComponent<KosmeticData>() == null)
                p.gameObject.AddComponent<KosmeticData>();
            }
            var __instance = FindObjectsOfType<Player>().Where((p) => p.gameObject.GetComponent<PhotonView>().IsMine && PhotonNetwork.IsMasterClient).ToArray()[0];
            __instance.gameObject.GetComponent<PhotonView>().RPC("RPCBoomer", RpcTarget.All);
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(Player), "Awake")]
        public static void PatchStart(Player __instance)
        {
            __instance.gameObject.AddComponent<KosmeticData>();
            var kd = __instance.gameObject.GetComponent<KosmeticData>();
            try
            {
                foreach (var v in FindObjectsOfType<PhotonView>().Where((pv) => pv.gameObject.GetComponent<Player>()).ToArray())
                {
                    var ___player = v.GetComponent<Player>();
                    int index = ___player.refs.view.ControllerActorNr - 1 % 32;
                    switch (___player.refs.view.Controller.NickName)
                    {
                        case "AncientKoala": index = 6; break;
                        case "Anarkey": index = 31; break;
                    }
                    var col = skinColors[index];
                    col.r *= 1.25f;
                    col.g *= 1.25f;
                    col.b *= 1.25f;
                    var colObj = ___player.transform.Find("Collider_Hitbox");
                    var meshRen = colObj.GetComponent<MeshRenderer>();
                    var mat = meshRen.material;
                    var newMat = new Material(mat);
                    newMat.name = "koalaschangedcolor";
                    newMat.color = col;
                    meshRen.material = newMat;
                    foreach (var ren in ___player.transform.Find("Limbs").GetComponentsInChildren<MeshRenderer>())
                    {
                        ren.material = newMat;
                    }
                    ___player.transform.Find("Collider_Map").GetComponent<MeshRenderer>().material = newMat;
                }
            } catch { }
        }
    }

    public class KosmeticData : MonoBehaviourPunCallbacks
    {
        [PunRPC]
        public void RPCBoomer()
        {
            var player = FindObjectsOfType<Player>().Where((p) => p.gameObject.GetComponent<PhotonView>().IsMine).ToArray()[0];
            var view = player.gameObject.GetComponent<PhotonView>();
            print(KWC.instance.cosmeticIds[0]);
            view.RPC("RPCRang", RpcTarget.All, new object[] { view.ControllerActorNr, KWC.instance.cosmeticIds, KWC.instance.cosmeticBox });
        }
        [PunRPC]
        public void RPCRang(int actorNr, int[] cosIds, bool box)
        {
            var targ = FindObjectsOfType<PhotonView>().Where((v) => v.ControllerActorNr == actorNr && v.gameObject.GetComponent<Player>()).ToArray()[0];
            var f = targ.transform.GetChild(3);
            var wasActive = f.gameObject.activeSelf;
            f.gameObject.SetActive(true);
            for (var i = 0; i < f.childCount; i++)
            {
                try
                {
                    Destroy(f.GetChild(i).gameObject);
                } catch { }
            }
            print(cosIds[0]);
            print(cosIds[1]);
            Instantiate(KWC.eyes[cosIds[0]], f);
            Instantiate(KWC.mouths[cosIds[1]], f);
            f.gameObject.SetActive(wasActive);
            if (box == true)
            {
                var cm = targ.transform.Find("Collider_Map");
                cm.GetComponent<MeshFilter>().mesh = KWC.squareMesh;
                var ch = targ.transform.Find("Collider_Hitbox");
                ch.GetComponent<MeshFilter>().mesh = KWC.squareMesh;
                if (!ch.GetComponent<BoxCollider>())
                {
                    ch.gameObject.AddComponent<BoxCollider>().material = ch.GetComponent<SphereCollider>().material;
                    Destroy(ch.GetComponent<SphereCollider>());
                }
            }
        }
    }
}
