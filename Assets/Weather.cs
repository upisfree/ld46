using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Weather : MonoBehaviour
{
    public GameObject fpsController;
    public PostProcessVolume ppv;
    public TMP_Text subtitle;
    public AudioSource hardTimes;
    public AudioSource wantTimes;
    public AudioSource loveTimes;

    private Bloom bloomLayer = null;
    private ColorGrading colorLayer = null;
    private LensDistortion lensLayer = null;

    private float walkingLastZ = 0;
    private float walkingSpeed;

    private int direction = 0;
    private int walkingDirection = 0;

    private float lensIntensity = -30;
    private float lensScale = 0.61f;

    public Dictionary<float, string> subtitles = new Dictionary<float, string>() {
        { 0f, "I only have one desire" },
        { 20f, "I want to keep us alive" },
        { 40f, "and how I wish" },
        { 60f, "how I wish you were here" },
        { 80f, " " },

        { 100f, "the angels are all in the shining" },
        { 120f, "the one next to You is the one I love" },
        { 140f, "what You want, answer" },
        { 160f, "my body and soul, my life and death," },
        { 180f, "anything that's not yet ripe, a place in Your paradise" },
        { 200f, "just give me the one I love" },
        { 222f, " " },
    };

    void Start()
    {
        ppv.profile.TryGetSettings(out bloomLayer);
        ppv.profile.TryGetSettings(out colorLayer);
        ppv.profile.TryGetSettings(out lensLayer);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateLens();
        UpdateSaturation();
        UpdateSound();
    }

    void FixedUpdate()
    {
        UpdateSpeed();
        UpdateSubtitles();
        UpdateFalling();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void UpdateSubtitles()
    {
        float z = fpsController.transform.position.z;

        //subtitle.text = z.ToString() + "\n\n";
        //return;

        foreach (var item in subtitles)
        {
            if (z < item.Key)
            {
                break;
            }

            subtitle.text = item.Value + "\n\n";
        }
    }

    void UpdateFalling()
    {
        if (fpsController.transform.position.y < -2000)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
        }
    }

    void UpdateSpeed()
    {
        float z = fpsController.transform.position.z;
        float delta = z - walkingLastZ;
        float absDelta = Mathf.Abs(delta);
        float signDelta = Math.Sign(delta);
        walkingDirection = Math.Sign(delta);

        walkingLastZ = z;
        walkingSpeed = Mathf.Lerp(walkingSpeed, absDelta, 1.5f * Time.deltaTime);
    }

    void UpdateSaturation()
    {
        colorLayer.saturation.Override(walkingSpeed * walkingDirection * -250);
        //colorLayer.contrast.Override(walkingSpeed * saturationCoef * -1);
    }

    void UpdateLens()
    {
        float intensityDelta = -30 - walkingSpeed * walkingDirection * 500;
        lensIntensity = Mathf.Lerp(lensIntensity, intensityDelta, 1f * Time.deltaTime);
        lensLayer.intensity.Override(lensIntensity);

        float scaleDelta = 0.6f + walkingSpeed * walkingDirection * 20f;
        lensScale = Mathf.Lerp(lensScale, scaleDelta, 1f * Time.deltaTime);

        if (lensScale < 0.4)
        {
            lensScale = 0.4f;
        }

        if (lensScale > 4)
        {
            lensScale = 4f;
        }

        lensLayer.scale.Override(lensScale);
    }

    void UpdateSound()
    {
        hardTimes.pitch = 1 + walkingSpeed * walkingDirection * -1f;
        wantTimes.pitch = 1 + walkingSpeed * walkingDirection * 1f;
        wantTimes.pitch = 1 + walkingSpeed * walkingDirection * -0.8f;
    }
}
