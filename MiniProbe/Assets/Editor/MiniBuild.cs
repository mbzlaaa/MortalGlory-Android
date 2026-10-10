using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MiniBuild
{
    public static void Build()
    {
        const int S = 2048;
        var tex = new Texture2D(S, S, TextureFormat.RGBA32, false, false);
        var px = new Color32[S * S];
        for (int y = 0; y < S; y++)
            for (int x = 0; x < S; x++)
                px[y * S + x] = new Color32((byte)(x ^ y), (byte)(x * 3), (byte)(y * 5), 255);
        tex.SetPixels32(px);
        tex.Apply(false, false);
        AssetDatabase.CreateAsset(tex, "Assets/bigtex.asset");

        var spr = Sprite.Create(tex, new Rect(0, 0, S, S), new Vector2(0.5f, 0.5f), 100f);
        AssetDatabase.CreateAsset(spr, "Assets/bigspr.asset");
        AssetDatabase.SaveAssets();

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        var camGo = new GameObject("MainCamera");
        camGo.AddComponent<Camera>();
        var go = new GameObject("BigSprite");
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = spr;
        for (int i = 0; i < 6000; i++)
        {
            var o = new GameObject("Obj" + i);
            var r = o.AddComponent<SpriteRenderer>();
            r.sprite = spr;
            o.transform.position = new Vector3((i % 100), (i / 100), 0f);
        }
        EditorSceneManager.SaveScene(scene, "Assets/MiniScene.unity");
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene("Assets/MiniScene.unity", true) };

        string repoRoot = Path.GetFullPath(Path.Combine(Application.dataPath, "..", ".."));
        string outDir = Path.Combine(repoRoot, "build", "Android");
        Directory.CreateDirectory(outDir);
        string apk = Path.Combine(outDir, "MiniProbe.apk");

        PlayerSettings.companyName = "TestCo";
        PlayerSettings.productName = "MiniProbe";
        PlayerSettings.bundleVersion = "1.0";
        PlayerSettings.SetApplicationIdentifier(BuildTargetGroup.Android, "com.test.miniprobe");
        PlayerSettings.Android.bundleVersionCode = 1;
        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android);
        var opts = new BuildPlayerOptions();
        opts.scenes = new[] { "Assets/MiniScene.unity" };
        opts.locationPathName = apk;
        opts.target = BuildTarget.Android;
        opts.options = BuildOptions.None;

        var report = BuildPipeline.BuildPlayer(opts);
        Debug.Log("MINIBUILD result=" + report.summary.result + " apk=" + apk + " exists=" + File.Exists(apk));
        if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
            EditorApplication.Exit(1);
    }
}
