using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fmb_Groggy_Wall : MonoBehaviour
{
    [SerializeField] private GameObject Obj_Warning;
    [SerializeField] private GameObject Obj_Warning_Position;

    private Coroutine auto_destruct_coroutine = null;
    private Coroutine warning_coroutine = null;

    private GameObject Spiritual = null;

    private bool is_Time_up = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(float Total_Time, float Warning_Time, GameObject fmb_)
    {
        auto_destruct_coroutine = StartCoroutine(Auto_Destruct(Total_Time));
        warning_coroutine = StartCoroutine(Warning(Warning_Time));

        Spiritual = fmb_;
    }

    private IEnumerator Auto_Destruct(float Time)
    {
        yield return new WaitForSeconds(Time);
        is_Time_up = true;
        Destroy(this.gameObject);
        yield return null;
    }

    private IEnumerator Warning(float Time)
    {
        yield return new WaitForSeconds(Time);
        GameObject Warning = Instantiate(Obj_Warning, Obj_Warning_Position.transform.position, Quaternion.identity);
        Warning.transform.localScale = new Vector3(14.0f, 4.0f, 1.0f);
        Warning.GetComponent<Warning_Object>().Initialize(1.0f, 0.8f, 0.0f);
        yield return null;
    }

    private void OnDestroy()
    {
        Debug.Log("Groggy Wall Destroyed");
        if (is_Time_up)
        {
            // Time up
            Spiritual.GetComponent<Fmb_Spiritual>().End_Groggy_Wall();
        }
        else
        {
            Spiritual.GetComponent<Fmb_Spiritual>().Start_Groggy();

            if (auto_destruct_coroutine != null)
            {
                StopCoroutine(auto_destruct_coroutine);
            }
            if (warning_coroutine != null)
            {
                StopCoroutine(warning_coroutine);
            }
        }
    }

    public void Force_Destroy()
    {
        StopAllCoroutines();
        Destroy(this.gameObject);
    }
}
