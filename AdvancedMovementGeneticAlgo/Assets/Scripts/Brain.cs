#define UseEthan

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain : MonoBehaviour
{
    /// <summary>
    /// Why Two?
    /// Needs to make two decisions
    /// 1. What to do when i see the ground
    /// 2. What to do when i dont see the ground
    /// </summary>
    int DNALength = 2;

    /// <summary>
    /// When you off the platform you die,
    /// our fitness function is based on which one stays alive the longest
    /// </summary>
    public float timeAlive;

    public float timeWalking;

    public GameObject EthanPrefab;
    GameObject ethan;

    /// <summary>
    /// Link to DNA
    /// </summary>
    public DNA dna;

    /// <summary>
    /// Link to our Eyes
    /// </summary>
    public GameObject eyes;

    bool alive = true;
    bool seeGround = true;

#if UseEthan
    private void OnDestroy()
    {
        Destroy(ethan);
    }
#endif

    /// <summary>
    /// If we end up hitting the dead zone which is the black part, we die
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "dead")
        {
            alive = false;
            timeAlive = 0;
            timeWalking = 0;
        }
    }


    public void Init()
    {
        ///DNA
        ///of Length Two <see cref="Brain.DNALength"/>
        ///dna[0] -> corresponds to when we see the ground what to do
        ///dna[1] -> corresponds to when we don't see the ground what to do

        //initialise DNA with codes
        // 0 forward
        // 1 Left
        // 2 right
        dna = new DNA(DNALength, 3);

        timeAlive = 0;
        alive = true;


#if UseEthan
        GetComponent<MeshRenderer>().enabled = false;
        eyes.GetComponent<MeshRenderer>().enabled = false;

        ethan = Instantiate(EthanPrefab, this.transform.position, this.transform.rotation);
        ethan.GetComponent<UnityStandardAssets.Characters.ThirdPerson.AICharacterControl>().target = transform;
#else
        GetComponent<MeshRenderer>().enabled = true;
        eyes.GetComponent<MeshRenderer>().enabled = true;
#endif
    }

    private void Update()
    {
        //If not alive return
        if (!alive)
        {
            return;
        }

        //For testing draw a ray
        Debug.DrawRay(eyes.transform.position, eyes.transform.forward * 10, Color.red, 0.1f);

        //Reset
        seeGround = false;

        //Raycast out to check if we have a platform infront of us or not
        RaycastHit hit;
        if (Physics.Raycast(eyes.transform.position, eyes.transform.forward * 10, out hit))
        {
            if(hit.collider.gameObject.tag == "platform")
            {
                seeGround = true;
            }
        }
        //Add time alive
        timeAlive = PopulationManager.elapsed;

        //Read DNA
        float turn = 0; //rotation
        float move = 0; // seps

        if (seeGround)
        {
            //make v relative to character and always move forward
            if (dna.GetGene(0) == 0)
            {
                move = 1;
                timeWalking += 1;
            }
            else if (dna.GetGene(0) == 1) turn = -90;
            else if (dna.GetGene(0) == 2) turn = 90;
        }
        else
        {
            if (dna.GetGene(1) == 0)
            {
                move = 1;
                timeWalking += 1;
            }
            else if (dna.GetGene(1) == 1) turn = -90;
            else if (dna.GetGene(1) == 2) turn = 90;
        }

        //Move and rotate accordingly
        transform.Translate(0, 0, move * 0.1f);
        transform.Rotate(0, turn, 0);
    }



}
