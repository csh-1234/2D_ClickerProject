using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartCutScene : MonoBehaviour
{
    public Animator CanAnim;
    //public static bool isCutSceneOn;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //isCutSceneOn = true;
        //CanAnim.SetBool("cutscene1", true);
        Invoke(nameof(StopCutScene), 3f);
    }

    public void StopCutScene()
    {
        //isCutSceneOn = false;
        CanAnim.SetBool("cutscene1", false);
    }
}
