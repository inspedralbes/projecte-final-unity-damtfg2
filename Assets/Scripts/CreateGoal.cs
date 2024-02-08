using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGoal : MonoBehaviour
{

    [SerializeField] private string Tag = "Stone";
    public GameObject Goal;
    // Start is called before the first frame update
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag(Tag))
        {
            Goal.SetActive(true);
            Debug.Log("Detecta Stone");
            
        }

    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag(Tag))
        {
            Goal.SetActive(false);
            Debug.Log("Detecta sale la Stone");
            
        }
    }
}