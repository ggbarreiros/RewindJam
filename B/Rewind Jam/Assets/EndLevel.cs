using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EndLevel : MonoBehaviour
{
    private bool endLevel = false;
    private GameObject player;
    public VolumeProfile volumeProfile;
    public Volume volume;
    private LevelManager lvlManager;
    [SerializeField] private int numberOfTimeRifts;
    Bloom bloom;

    [Space]
    [Header("Materials")]
    private Material defaultMat;
    [SerializeField] private Material glitchMaterial;


    void Start()
    {
        lvlManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<LevelManager>();
        defaultMat = GetComponent<SpriteRenderer>().material;
        
        Bloom bl;
        if (volume.profile.TryGet<Bloom>(out bl))
        {
            bloom = bl;
        }
    }

    void Update()
    {
        if (endLevel && player != null)
        {
            if (bloom.intensity.value < 8000f)
            {
                bloom.intensity.value *= 1.2f;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Player")
        {
            if (numberOfTimeRifts < 1)
            {
                player = other.gameObject;
                endLevel = true;
            }
        }
    }

    public void CheckLevelStatus()
    {
        PlayerScript plScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        //numberOfTimeRifts = plScript.timeRifts.Length;
        int number = 0;

        for(int i = 0; i < plScript.timeRifts.Length; i++)
        {
            if (plScript.timeRifts[i] != null)
                number++;
        }

        numberOfTimeRifts = number;

        if (numberOfTimeRifts < 1)
        {
            GetComponent<SpriteRenderer>().material = defaultMat;
        }
        else
        {
            GetComponent<SpriteRenderer>().material = glitchMaterial;
        }
    }
}
