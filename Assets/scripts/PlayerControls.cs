﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {

    [Header("Movement Parameters")]
    public float speed;
    public bool grounded = false;
    public bool movesInX = false;
    public bool movesInY = true;

    public GameObject projectile;
    public GameObject generator;
    public float defaultCooldown = 1f;
    public float threshold = .2f;

    protected Vector3 _intialScale;
    protected float _nextTimeAttack = 0;

    [SerializeField]
    float initialXPositionInUnits = 0;
    [SerializeField]
    private float initialYPositionInUnits = 0;
    [SerializeField]
    private float inertiaMultiplier = 0;


    [Header("Audio Input Parameters")]
    public float pitchMin = 100;
     public float pitchMax = 500;
    [SerializeField]
    private int numValues;
    [SerializeField]
    private AnimationCurve pitchBehavior = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField]
    private AnimationCurve loundnessThresholdBehavior = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField]
    private float pitchIn;
    [SerializeField]
    private float inputWave;
    [SerializeField]
    private float loudnessIn;
    [SerializeField]
    private float loudnessThreshold;

    [Header("Wave Attack Parameters")]
    [SerializeField]
    private AnimationCurve waveStartingRadius = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField]
    private AnimationCurve waveRadiusBehavior = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField]
    private AnimationCurve waveLengthBehavior = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    private AnimationCurve waveSpeedBehavior = AnimationCurve.Linear(0, 0, 1, 1);
    [SerializeField]
    private AnimationCurve waveCouldownBehavior = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField]
    private AnimationCurve waveNumberBehavior = AnimationCurve.Linear(0, 1, 1, 1);
    [SerializeField]
    private AnimationCurve waveWidthBehavior = AnimationCurve.Linear(0, 1, 1, 1);

    private GameObject player;
    private ParticleSystem waveGenerator;
    private Vector3 moveDirection = Vector3.zero;
    private float nextWaveTimeMin = 0;
    private float nextWaveDisableTime = 0;
    private MicroHandler micSource = null;
    private float[] values;
    private int curIndex;
    
	// Use this for initialization
	void Start () {
        this.player = this.gameObject;
        this.player.transform.position = new Vector3(initialXPositionInUnits, 0, initialYPositionInUnits);
        this.micSource = this.GetComponentInChildren<MicroHandler>();
        this.values = new float[numValues];
        _intialScale = projectile.transform.localScale;

    }
	
	// Update is called once per frame
	void Update () {
        if(!GameManager.Instance.Running)
            return;

        //if (!grounded)
        //{
        //    if (movesInX) this.moveDirection.x = Input.GetAxis("Horizontal");
        //    if (movesInY) this.moveDirection.y = Input.GetAxis("Vertical");
        //    this.moveDirection.Normalize();
        //    this.moveDirection *= this.speed;
        //}
        //else if (this.moveDirection.magnitude > 0.25) this.moveDirection *= inertiaMultiplier;
        //else this.moveDirection = new Vector3(0, 0, 0);

        //this.player.transform.Translate(this.moveDirection * Time.deltaTime);


        float pitch = micSource.pitch;
        if (pitch > pitchMax) pitch = pitchMax;
        if (pitch < pitchMin) pitch = pitchMin;
        float inputWave = (pitch - pitchMin) / (pitchMax - pitchMin);
        loudnessIn = micSource.loudness;
        //inputWave = pitchBehavior.Evaluate(pitchIn);
        //loudnessThreshold = loundnessThresholdBehavior.Evaluate(inputWave);

        values[curIndex] = micSource.pitch / numValues;
        curIndex = (curIndex + 1) % numValues;

        pitchIn = 0;
        for (int i = 0; i < numValues; i++) pitchIn += values[i];

        var currentTime = Time.realtimeSinceStartup;
        if (loudnessIn >= threshold && currentTime >= _nextTimeAttack)
        {
            float range = 2 - .5f;
            float ratioSize = 1f - inputWave;
            _nextTimeAttack = currentTime + ratioSize * defaultCooldown + defaultCooldown/2;
            projectile.transform.localScale = new Vector3(_intialScale.x, .5f + range * ratioSize, _intialScale.z);
            this.GetComponent<Animator>().SetTrigger("shoot");
            GameObject.Instantiate(projectile, generator.transform.position, Quaternion.identity);
            //// Length
            //var main = this.waveGenerator.main;
            //float lifeTime = waveLengthBehavior.Evaluate(inputWave);
            //main.startLifetime = lifeTime;
            //// Radius
            //var sizeOverLifetimeParam = this.waveGenerator.sizeOverLifetime;
            //sizeOverLifetimeParam.enabled = true;
            //var curve = new AnimationCurve();
            //curve.AddKey(0, waveStartingRadius.Evaluate(inputWave));
            //curve.AddKey(lifeTime, waveRadiusBehavior.Evaluate(inputWave));
            //sizeOverLifetimeParam.y = new ParticleSystem.MinMaxCurve(1, curve);
            //// Width
            //sizeOverLifetimeParam.x = waveWidthBehavior.Evaluate(inputWave);
            //// Speed
            //main.startSpeed = waveSpeedBehavior.Evaluate(inputWave);
            //// Emission rate
            //var emissionRate = emission.rateOverTime;
            //emissionRate.constant = waveNumberBehavior.Evaluate(inputWave);
            //emission.rateOverTime = emissionRate;
            //// CD
            //nextWaveTimeMin = Time.time + waveCouldownBehavior.Evaluate(inputWave);
            //nextWaveDisableTime = Time.time + 1;
        }
	}

    void FixedUpdate()
    {
        if (!grounded /*&& !Input.GetKey("joystick 1")*/)
        {
            float movex = Input.GetAxis("Horizontal");
            float movey = Input.GetAxis("Vertical");
            GetComponent<Rigidbody2D>().AddForce(new Vector2(movex * speed, movey * speed), ForceMode2D.Force);
        }
    }
}
