using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipGUI : MonoBehaviour
{

    public SpaceshipSpawner spawner;

    private void OnGUI()
    {
        var textStyle = new GUIStyle(GUI.skin.label);
        textStyle.fontSize = 20;
        textStyle.alignment = TextAnchor.MiddleCenter;

        GUI.Box(new Rect(0, 0, 450, 200), string.Empty);
        GUILayout.BeginArea(new Rect(0, 0, 450, 250));
        GUILayout.Box("Voronoi & Spaceships", textStyle);

        if(this.spawner.GetSpaceshipSampler() != null)
        {
            var sampler = this.spawner.GetSpaceshipSampler();
            var recorder = sampler.GetRecorder();
            if(recorder != null)
            {
                GUILayout.Label($"Finding Closest Asteroids (ms): {(recorder.elapsedNanoseconds / 10e5f).ToString("N3")}", textStyle);
            }
        }

        if(this.spawner.IsUsingLookupTable())
        {
            if(GUILayout.Button("Stop Using Lookup Table"))
            {
                this.spawner.UseVoronoiLookupTable(false);
            }

            float oldAlpha = this.spawner.CurrentVoronoiAlpha();
            float alpha = GUILayout.HorizontalSlider(this.spawner.CurrentVoronoiAlpha(), 0.0f, 1.0f);
            if (alpha != oldAlpha)
            {
                this.spawner.SetVoronoiAlpha(alpha);
            }

        } else
        {
            if(GUILayout.Button("Use Lookup Table"))
            {
                this.spawner.UseVoronoiLookupTable(true);
            }


        }

        GUILayout.EndArea();

    }
}
