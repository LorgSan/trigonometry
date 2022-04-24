using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GizmosDraw : MonoBehaviour
{
    const float TAU = 6.28318530718f; //this is a full circle turn in radians
    //const float TAU = 3f; //this is a full circle turn in radians

    int dotCount; //amount of spheres that get created

    int maxBalls; //maximum amount of spheres that can spawn
    float timerAccelerate; //this influences the timer for each round of spheres

    public Text timerUI; //ui elements 
    public Text scoreUI; //for timer and score

    List<GameObject> SpheresCreated = new List<GameObject>(); //list of spheres created

    float timer; //says it itself

    int hitSpheresCount; //this thign helps me count how many spheres were hit

    #region difficulty declaration
    public enum Difficulty
    {
        Easy,
        Normal,
        Hard
    }

    public float rotationSpeed;
    public Difficulty difficulty;

    public void ChangeDifficulty(Difficulty newDifficulty)
    {
        switch (newDifficulty)
        {
            case Difficulty.Easy:
                maxBalls = 6;
                timerAccelerate = 1.7f;
                rotationSpeed = 10f;
                break;
            case Difficulty.Normal:
                maxBalls = 10;
                timerAccelerate = 1.4f;
                rotationSpeed = 40f;
                break;
            case Difficulty.Hard:
                maxBalls = 16;
                timerAccelerate = 1.1f;
                rotationSpeed = 60f;
                break;
        }
    }
    #endregion

    void Start()
    {
        ChangeDifficulty(difficulty); //change the difficulty settings depending on the parameter in the inspector
        SpawnSphereToHit(); //and spawn first spheres
    }

    void SpawnSphereToHit() //all 3d math is here!
    {
        Debug.Log("spawn called");
        

        RandomPick(); //this thing helps me randomize amount of spheres each round, but also makes it new each time

        for ( int i = 0; i < dotCount; i++) //create a new sphere based on the amount we need
        {
            float t = i / (float)dotCount; //this is a number between 0 and 1 that basically returns an angle of the element to draw in turns
            float angRad = t * TAU; //here we transfer turns into radians (cause turns are exactly resamling 1 TAU which is 6.28...)
            float x = Mathf.Cos(angRad);
            float y = Mathf.Sin(angRad); //here we project the angle onto the coordinate system, making two points to draw something there
            Vector2 point = new Vector2(x,y); //smash them together into one vector2
            //Gizmos.DrawSphere(point, 0.06f); //first value is the position, second is the radius of the sphere
            GameObject Sphere = Instantiate(Resources.Load("Sphere") as GameObject, SphereParent.transform);
            Vector3 newPos = new Vector3(point.x, point.y, 0f);
            Sphere.transform.position = newPos; //update the position 
            //Sphere.transform.position = newPos * Random.Range(0.8f,1.2f); //update the position
            SpheresCreated.Add(Sphere); //add it to the list
        }
        
        if (CoroutineRunning == false)
        {
            StartCoroutine("SpawnDelay"); //start the timer until the next round
        }
    }

    public GameObject SphereParent;
    public void Rotator() //this thing just rotates the parent of all the spheres through eulerangles
    {
        Vector3 currentEulerAngles = SphereParent.transform.eulerAngles;
        currentEulerAngles.z += Time.deltaTime * rotationSpeed;
        SphereParent.transform.eulerAngles = currentEulerAngles;
    }

    IEnumerator SpawnDelay()
    {
        CoroutineRunning = true;
        //Debug.Log("coroutine started");
        timer = dotCount * timerAccelerate; //we add a bit of time to each sphere to just have a pinch more time
        //Debug.Log(timer);
        yield return new WaitForSeconds(timer); 
        CoroutineRunning = false;
        ClearSpheres(); //after the timer run off we destroy all the spheres created
        SpawnSphereToHit(); //and spawn it all again
    }

    public void SphereCheck(GameObject Sphere) //but! if we manage to hit all of them
    {
            //Debug.Log("we are checking the sphere amount");
            hitSpheresCount++;
            Debug.Log(hitSpheresCount + " " + SpheresCreated.Count);
            //Debug.Log(SpheresCreated.Count);

            if (hitSpheresCount == SpheresCreated.Count) //we add the amount of spheres we hit to the score and reset everything else 
            //also stop coroutine and etc
            {
                //Debug.Log(Sphere.name);
                //Debug.Log(hitSpheresCount);
                hitSpheresCount = 0; //basically reset everything
                score = score + SpheresCreated.Count; //update score
                timer = 0;
                StopCoroutine("SpawnDelay"); //stop the coroutine!
                CoroutineRunning = false;
                ClearSpheres(); //destroy all prevoius spheres
                SpawnSphereToHit(); //and spawn the spehres again
            }
    }

    public bool CoroutineRunning;
    int score;


    void Update()
    {
        Rotator();
        if (timer != 0)
        {
            timer -= Time.deltaTime;
        }
        Timer (timer);
        Score();
    }

    void Timer(float timeToDisplay)
    {
        if (timeToDisplay < 0)
        {
            timeToDisplay = 0;
        }
        float showTime = Mathf.FloorToInt(timeToDisplay);
        timerUI.text = showTime.ToString();
    }

    void Score()
    { 
        scoreUI.text = score.ToString();
    }

    #region Utilization Functions
    
    void ClearSpheres()
    {
        for (int i = 0; i < SpheresCreated.Count; i++)
        {
            Destroy(SpheresCreated[i]);
        }
        SpheresCreated.Clear();        
    }

    void RandomPick()
    {
        int newRandomCount = dotCount;
        // loop until the random value does not match last one
        while(newRandomCount == dotCount)
        {
            newRandomCount = Random.Range(2, maxBalls);
        }
        // update last one.
        dotCount = newRandomCount;
    }

    #endregion
}
