using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using TMPro;
using Afterglow.M01;

namespace Afterglow.Editor
{
    public static class PMMapBuilder
    {
        static Transform root;static TMP_FontAsset font;static Material earth,green,pathMat,wood,metal,dark;
        static readonly Vector3[] Points={new Vector3(-5,.3f,-3),new Vector3(0,.3f,-3),new Vector3(5,.3f,1),new Vector3(0,.3f,5),new Vector3(-5,.3f,5),new Vector3(-9,.3f,5)};
        public static void Build()
        {
            if(EditorApplication.isPlaying)throw new InvalidOperationException("Stop Play Mode first");
            if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new InvalidOperationException("Save current scene first");
            string scenePath=M01Expedition.ScenePath("M01_ACCESO");Directory.CreateDirectory("Tools/PM/Backups");if(!File.Exists("Tools/PM/Backups/M01_ACCESO.unity"))File.Copy(scenePath,"Tools/PM/Backups/M01_ACCESO.unity");
            var scene=EditorSceneManager.OpenScene(scenePath);
            foreach(var old in scene.GetRootGameObjects().SelectMany(go=>go.GetComponentsInChildren<Transform>(true)).Where(t=>t.name=="PM_RouteBoard"||t.name=="PM_MapView"||t.name=="PM_Miniature").ToArray())if(old)UnityEngine.Object.DestroyImmediate(old.gameObject);
            var dialogue=UnityEngine.Object.FindAnyObjectByType<M01Dialogue>();font=dialogue.body.font;
            earth=Mat("Tierra",new Color(.36f,.25f,.17f));green=Mat("Musgo",new Color(.36f,.43f,.22f));pathMat=Mat("Sendero",new Color(.78f,.57f,.31f));wood=Mat("Madera",new Color(.32f,.16f,.07f));metal=Mat("Riel",new Color(.35f,.43f,.45f));dark=Mat("Tunel",new Color(.045f,.055f,.065f));
            root=new GameObject("PM_RouteBoard").transform;root.position=new Vector3(5.7f,0,-1.6f);
            Part("Poste",new Vector3(5.7f,.7f,-1.6f),new Vector3(.15f,1.4f,.15f),wood);Part("Panel de rutas",new Vector3(5.7f,1.6f,-1.6f),new Vector3(1.8f,1,.15f),wood);
            var text=new GameObject("Rotulo").AddComponent<TextMeshPro>();text.transform.SetParent(root);text.transform.position=new Vector3(5.7f,1.65f,-1.7f);text.font=font;text.text="RUTAS\n[ E ]";text.fontSize=2;text.color=new Color(1,.9f,.65f);text.alignment=TextAlignmentOptions.Center;text.rectTransform.sizeDelta=new Vector2(1.8f,1);
            var map=root.gameObject.AddComponent<M01RouteMap>();map.dialogue=dialogue;
            var panel=Rect("PM_MapView",dialogue.transform,new Vector2(1780,940),Vector2.zero);map.view=panel.gameObject;
            panel.localScale=Vector3.one*.7f;
            panel.gameObject.AddComponent<UnityEngine.UI.Image>().color=new Color(.065f,.085f,.10f,1);
            Text("MUNDO UNO · VALLE MINERO",panel,new Vector2(1650,55),new Vector2(0,418),38,new Color(1,.84f,.48f));
            var image=Rect("Relieve y caminos",panel,new Vector2(1250,650),new Vector2(-200,40));var raw=image.gameObject.AddComponent<UnityEngine.UI.RawImage>();
            string rtPath="Assets/Afterglow/Environment/Prologue/MapaRT.renderTexture";var rt=AssetDatabase.LoadAssetAtPath<RenderTexture>(rtPath);if(!rt){rt=new RenderTexture(1500,780,24){name="MapaRT",antiAliasing=2};AssetDatabase.CreateAsset(rt,rtPath);}raw.texture=rt;
            root=new GameObject("PM_Miniature").transform;map.miniature=root.gameObject;
            Part("Isla terreno",new Vector3(1000,-.8f,1),new Vector3(23,1.6f,15),earth);Part("Superficie",new Vector3(1000,.03f,1),new Vector3(23,.12f,15),green);
            for(int i=0;i<5;i++)Road(Points[i],Points[i+1]);
            Road(new Vector3(-9,.3f,-5),Points[0]);
            for(int i=0;i<6;i++)Miniature(i,Points[i]+new Vector3(1000,0,0));
            // Low-poly rock silhouettes are native editable scene pieces; no reference bitmap is used as game art.
            for(int i=0;i<18;i++){float x=-10+(i%9)*2.5f,z=i<9?7.4f:-5.8f;var rock=Part("Roca de borde",new Vector3(1000+x,.65f,z),new Vector3(1.8f,1.4f+(i%3)*.4f,1.7f),earth,PrimitiveType.Sphere);rock.transform.rotation=Quaternion.Euler(10,i*27,15);}
            var miniCam=new GameObject("Camara solo maqueta").AddComponent<Camera>();miniCam.transform.SetParent(root);miniCam.transform.position=new Vector3(1000,20,-18);miniCam.transform.LookAt(new Vector3(1000,0,1));miniCam.orthographic=true;miniCam.orthographicSize=10;miniCam.aspect=1250f/650;miniCam.targetTexture=rt;miniCam.clearFlags=CameraClearFlags.SolidColor;miniCam.backgroundColor=new Color(.1f,.14f,.17f);miniCam.cullingMask=1<<30;miniCam.nearClipPlane=.3f;miniCam.farClipPlane=80;
            var light=new GameObject("Luz solo maqueta").AddComponent<Light>();light.transform.SetParent(root);light.type=LightType.Directional;light.intensity=1.3f;light.cullingMask=1<<30;light.transform.rotation=Quaternion.Euler(55,-40,0);
            foreach(var tr in root.GetComponentsInChildren<Transform>())tr.gameObject.layer=30;
            map.nodes=new RectTransform[6];map.badges=new UnityEngine.UI.Image[6];map.nodeLabels=new TMP_Text[6];
            for(int i=0;i<6;i++)
            {
                var v=miniCam.WorldToViewportPoint(Points[i]+new Vector3(1000,.4f,0));var n=Rect("Nodo "+M01RouteMap.Id(i),image,new Vector2(108,54),new Vector2((v.x-.5f)*1250,(v.y-.5f)*650-47));map.nodes[i]=n;map.badges[i]=n.gameObject.AddComponent<UnityEngine.UI.Image>();map.nodeLabels[i]=Text("N"+(i+1),n,new Vector2(106,52),Vector2.zero,18,Color.white);
            }
            map.marker=Text("▼",image,new Vector2(70,50),Vector2.zero,34,new Color(1,.83f,.25f)).rectTransform;
            map.title=Text("Ruta",panel,new Vector2(355,130),new Vector2(675,265),32,new Color(1,.84f,.48f));map.details=Text("Estado",panel,new Vector2(355,460),new Vector2(675,-35),25,Color.white);
            map.instructions=Text("Controles",panel,new Vector2(1660,60),new Vector2(0,-405),26,Color.white);
            Text("ACCESO SEGURO  →  EXPEDICIONES",panel,new Vector2(1200,50),new Vector2(-200,-320),24,new Color(.75f,.8f,.72f));
            foreach(var point in UnityEngine.Object.FindObjectsByType<M01StoryPoint>()){if(point.eventId=="N1A.INTRO")point.prerequisiteEvent="M01.N01_ENTERED";if(point.eventId=="N1C.PHONE")point.prerequisiteEvent="N1A.INTRO";}
            foreach(var door in UnityEngine.Object.FindObjectsByType<M01RouteDoor>())if(door.destination=="N01S02")door.enterExpedition=false;
            map.view.SetActive(false);map.miniature.SetActive(false);EditorSceneManager.SaveScene(scene);AssetDatabase.SaveAssets();Debug.Log("PM09–10 map integrated into shared access");
        }
        static Material Mat(string name,Color c){string path="Assets/Afterglow/Environment/Prologue/Mapa_"+name+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(m,path);}m.color=c;m.SetFloat("_Smoothness",.08f);return m;}
        static GameObject Part(string name,Vector3 pos,Vector3 size,Material mat,PrimitiveType type=PrimitiveType.Cube){var go=GameObject.CreatePrimitive(type);go.name=name;go.transform.SetParent(root);go.transform.position=pos;go.transform.localScale=size;go.GetComponent<Renderer>().sharedMaterial=mat;UnityEngine.Object.DestroyImmediate(go.GetComponent<Collider>());return go;}
        static RectTransform Rect(string name,Transform parent,Vector2 size,Vector2 pos){var r=new GameObject(name,typeof(RectTransform)).GetComponent<RectTransform>();r.SetParent(parent,false);r.anchorMin=r.anchorMax=r.pivot=new Vector2(.5f,.5f);r.sizeDelta=size;r.anchoredPosition=pos;return r;}
        static TMP_Text Text(string s,Transform parent,Vector2 size,Vector2 pos,float fs,Color color){var r=Rect(s,parent,size,pos);var t=r.gameObject.AddComponent<TextMeshProUGUI>();t.font=font;t.text=s;t.fontSize=fs;t.color=color;t.alignment=TextAlignmentOptions.Center;t.overflowMode=TextOverflowModes.Ellipsis;return t;}
        static void Road(Vector3 a,Vector3 b){var v=b-a;var go=Part("Camino conectado",new Vector3(1000,0,0)+(a+b)*.5f,new Vector3(.5f,.1f,v.magnitude),pathMat);go.transform.rotation=Quaternion.LookRotation(v);}
        static void Miniature(int index,Vector3 p)
        {
            if(index==1){for(int j=0;j<2;j++){var r=Part("Cruce de rieles",p+new Vector3(0,.1f,0),new Vector3(3,.1f,.12f),metal);r.transform.rotation=Quaternion.Euler(0,j*90,0);}Part("Vagon",p+new Vector3(.5f,.45f,.5f),new Vector3(.8f,.55f,.55f),wood);}
            else if(index==3){Part("Via elevada",p+Vector3.up*.7f,new Vector3(3.3f,.18f,.7f),wood);foreach(float x in new[]{-1.2f,1.2f})Part("Soporte puente",p+new Vector3(x,.3f,0),new Vector3(.2f,1.1f,.4f),wood);}
            else if(index==2){Part("Poste senales",p+Vector3.up*.8f,new Vector3(.15f,1.6f,.15f),wood);Part("Senal",p+Vector3.up*1.5f,new Vector3(1.8f,.4f,.15f),pathMat);}
            else
            {
                Part("Colina mina",p+new Vector3(0,.6f,.5f),new Vector3(2.7f,2.4f,2),earth,PrimitiveType.Sphere);Part("Boca oscura",p+new Vector3(0,.5f,-.48f),new Vector3(1.3f,1.6f,.1f),dark);
                foreach(float x in new[]{-.8f,.8f})Part("Marco mina",p+new Vector3(x,.6f,-.6f),new Vector3(.2f,1.9f,.2f),wood);Part("Dintel mina",p+new Vector3(0,1.5f,-.6f),new Vector3(1.9f,.22f,.25f),wood);
                if(index==5)for(int j=0;j<4;j++)Part("Derrumbe",p+new Vector3(-.7f+j*.45f,.3f+j%2*.3f,-.8f),new Vector3(.7f,.65f,.5f),earth,PrimitiveType.Sphere);
                if(index==4)Part("Lampara",p+new Vector3(.6f,1.3f,-.85f),Vector3.one*.3f,pathMat,PrimitiveType.Sphere);
            }
        }
    }
}
