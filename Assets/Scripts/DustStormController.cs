using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustStormController : MonoBehaviour
{
    public GameObject rover;

    public float noStormInterval = 15f;
    public float softStormCyleInterval = 30f;
    public float heavyStormCycleInterval = 40f;
    public float interpolationTime = 1f;
    public float damageRate = 2f;

    private RoverController roverController;
    private ParticleSystem ps;
    private float nextDamageTime = 0f;
    private int momentInCycle, lastMomemntInCycle = 0,fullCycleTime;

    private AudioManager audioManager;

    // Start is called before the first frame update
    void Start()
    {
        roverController = rover.GetComponent<RoverController>();
        ps = GetComponent<ParticleSystem>();
        fullCycleTime = (int) (noStormInterval + softStormCyleInterval + heavyStormCycleInterval);

        audioManager = FindObjectOfType<AudioManager>();
    }

    // Update is called once per frame
    void Update()
    {
        momentInCycle = (int) Time.time % fullCycleTime;
        if (lastMomemntInCycle != momentInCycle)
        {
            lastMomemntInCycle = momentInCycle;
            Debug.Log(momentInCycle);
        }

        var emission = ps.emission;
        var velOverLT = ps.velocityOverLifetime;
        if (momentInCycle <= noStormInterval)
        {
            if(audioManager.IsPlaying("WindStormAudio"))
            {
                audioManager.Stop("WindStormAudio");
            }

            ps.Play();
            emission.rateOverTime = 0;
            velOverLT.speedModifier = new ParticleSystem.MinMaxCurve(0f, 0f);
        }
        else if (momentInCycle <= softStormCyleInterval + noStormInterval)
        {
            if (!audioManager.IsPlaying("WindStormAudio"))
            {
                audioManager.Play("WindStormAudio");
            }

            ps.Play();
            emission.rateOverTime = 1000;
            velOverLT.speedModifier = new ParticleSystem.MinMaxCurve(0.0f, 1f);

            if (Time.time >= nextDamageTime)
            {
                if (!roverController.isCrouched)
                {
                    roverController.TakeDamage(3);
                }
                nextDamageTime = Time.time + 1 / damageRate;
            }
        }
        else
        {
            ps.Play();
            emission.rateOverTime = 2000;
            velOverLT.speedModifier = new ParticleSystem.MinMaxCurve(0.0f, 20.0f);

            if (Time.time >= nextDamageTime)
            {
                if (!roverController.isCrouched)
                {
                    roverController.TakeDamage(5);
                }
                else
                {
                    roverController.TakeDamage(1);
                }
                nextDamageTime = Time.time + 1 / damageRate;
            }
        }
    }
}
