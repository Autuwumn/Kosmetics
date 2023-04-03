using System;
using System.Linq;
using System.Resources;
using BepInEx;
using HarmonyLib;
using R3DCore.Menu;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using R3DCore;
using System.Threading;
using BepInEx.Configuration;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using static System.Net.Mime.MediaTypeNames;

namespace KWC
{
    [BepInPlugin(ModId, ModName, Version)]
    [BepInProcess("ROUNDS 3D.exe")]
    [HarmonyPatch]
    public class KWC : BaseUnityPlugin
    {
        private const string ModId = "koala.wonderous.cosmetics";
        private const string ModName = "KWC";
        public const string Version = "0.0.1";
        public static KWC instance { get; private set; }
        public int yoruSkin = 0;
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
            new Color(0.254902f, 0.3686275f, 0.5843138f, 1f), // Light blue -
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
        private void GenerateSquare()
        {
            squareMesh.name = "KoalasCube";
            float size = 1.1f;
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
            new Harmony(ModId).PatchAll();
            GenerateSquare();
            yoruSkin = UnityEngine.Random.Range(0, skinColors.Length);
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Player), "Start")]
        public static void PatchAwake(Player __instance)
        {
            var a =__instance.gameObject.AddComponent<KosmeticData>();
            a.preferedID = PlayerPrefs.GetInt("PreferedColor");
            a.isBox = false;
            if(PlayerPrefs.GetInt("WantBox") == 1)
            {
                a.isBox = true;
            }
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Connection), "OnPlayerEnteredRoom")]
        public static bool PatchOnPlayerEnteredRoom(Player ___player)
        {
            var vid = ___player.GetComponent<PhotonView>().ControllerActorNr;
            var kd = ___player.gameObject.GetComponent<KosmeticData>();
            var players = FindObjectsOfType<Player>();
            if (players.Length == 1) kd.colorID = kd.preferedID;
            bool getPref = true;
            foreach (var p in players)
            {
                if (p.GetComponent<KosmeticData>().colorID == kd.preferedID)
                {
                    getPref = false; break;
                }
            }
            if (getPref)
            {
                kd.colorID = kd.preferedID;
            }
            int newIndex = 0;
            while (kd.colorID == -1)
            {
                bool skedadle = true;
                foreach (var p in players)
                {
                    if (p.GetComponent<KosmeticData>().colorID == newIndex)
                    {
                        skedadle = false;
                    }
                }
                if (skedadle)
                {
                    kd.colorID = newIndex;
                }
                else { newIndex++; }
            }
            return false;
        }
        [HarmonyPrefix]
        [HarmonyPatch(typeof(PL_Damagable), "RPCA_Die")]
        public static bool PatchRPCDie(Player ___player)
        {
            var kd = ___player.GetComponent<KosmeticData>();
            kd.colorID = ___player.refs.view.ControllerActorNr % 32;
            int index = kd.colorID;
            bool isBox = false;
            switch(___player.refs.view.Controller.NickName)
            {
                case "AncientKoala": index = 6; isBox = true; break;
                case "Anarkey": index = 31; isBox = true; break;
            }
            if(isBox)
            {
                ___player.transform.Find("Collider_Map").GetComponent<MeshFilter>().mesh = squareMesh;
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
            return false;
        }

        
    }

    public class KosmeticData : MonoBehaviourPunCallbacks
    {
        public int colorID = -1;
        public int preferedID;
        public int[] cosmeticIDS = new int[] { 0, 0, 0, 0 };
        public bool isBox = false;

        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            photonView.RPC("RPCstatpush", newPlayer, photonView.ControllerActorNr, );
        }

        [PunRPC]
        public void RPCstatPush(int canr, int cid, int pfid, int[] cosIDS, bool b)
        {
            var player = (PhotonView)FindObjectsOfType<PhotonView>().Where((pv) => pv.ViewID == pid);
            var kd = player.GetComponent<KosmeticData>();
            kd.colorID = cid;
            kd.preferedID = pfid;
            kd.cosmeticIDS = cosIDS;
            kd.isBox = b;
        }
    }
}
