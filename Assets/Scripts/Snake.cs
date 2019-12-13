using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Snake : MonoBehaviour {

    public GameObject food;
    private Transform rBorder;
    private Transform lBorder;
    private Transform tBorder;
    private Transform bBorder;
    public GameObject tailburst;
    private GameManager gameManager;
    private PlayerManager playerManager;

    private List<GameObject> foodList = new List<GameObject>();
    private bool vertical = false;
    private bool horizontal = true;
    private bool eat = false;
    private bool dead = false; 
    
	Vector2 vector = Vector2.up;
	

    public List<GameObject> BodyParts = new List<GameObject>();
    public float mindistance = 0.1f;
    public int beginSize; 
    public float speed = 1f;
    public float acceleration = 0.05f;
    public float rotationspeed = 50f;
    public GameObject bodyprefab;
    private float dis;
    private Transform curBodyPart;
    private Transform prevBodyPart;     

    // Use this for initialization
    void Start () {

        playerManager = GameObject.FindObjectOfType<PlayerManager>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        rBorder = GameObject.Find("border-right").transform;
        lBorder = GameObject.Find("border-left").transform;
        tBorder = GameObject.Find("border-top").transform;
        bBorder = GameObject.Find("border-bottom").transform;
                
        SpawnFood();
        // Setup Snake strating length
        for (int i= 0 ; i < beginSize -1; i++)
        {
            AddBodyPart();
        }
    }
	
	// Update is called once per frame
	void Update () {        
        Move();     
    }

    public void Move()
    {
        float curspeed = speed;
        
        // Speedup
        if (Input.GetKey(KeyCode.LeftShift ))
            curspeed *= 2;
        
        // Add Bodyparts
        if (Input.GetKey(KeyCode.Q))
            AddBodyPart();

        // Change Direction
        if (Input.GetKey(KeyCode.RightArrow) && horizontal)
        {
            horizontal = false;
            vertical = true;
            vector = Vector3.right;
           // Debug.Log("Right");
        }
        else if (Input.GetKey(KeyCode.UpArrow) && vertical)
        {
            horizontal = true;
            vertical = false;
            vector = Vector3.up;
            //Debug.Log("Up");
        }
        else if (Input.GetKey(KeyCode.DownArrow) && vertical)
        {
            horizontal = true;
            vertical = false;
            vector = -Vector3.up;
            //Debug.Log("Down");
        }
        else if (Input.GetKey(KeyCode.LeftArrow) && horizontal)
        {
            horizontal = false;
            vertical = true;
            vector = -Vector3.right;
            //Debug.Log("Left");
        }

        // Move Head
        transform.Translate(vector * curspeed * Time.smoothDeltaTime);

        // Lerp Bodyparts to follow head
        for (int i=0; i<BodyParts.Count; i++)
        {
            curBodyPart = BodyParts[i].transform;
            
            if (i == 0)
                prevBodyPart = transform;
            else
                prevBodyPart = BodyParts[i - 1].transform;

            dis = Vector3.Distance(prevBodyPart.position, curBodyPart.position);

            Vector3 newpos = prevBodyPart.position;
            
            float T = Time.deltaTime * dis / mindistance * curspeed;

            if (T > 0.5f)
                T = 0.5f;
            curBodyPart.position = Vector3.Slerp(curBodyPart.position, newpos, T);            
        }
       
    }

    public void AddBodyPart()
    {        
        Transform newpart = (Instantiate(bodyprefab, BodyParts[BodyParts.Count - 1].transform.position, BodyParts[BodyParts.Count - 1].transform.rotation) as GameObject).transform;
        if (BodyParts.Count > 1) newpart.name = "tail";        
        BodyParts.Add(newpart.gameObject); 
    }




    void OnTriggerEnter(Collider c)
    {
       // Debug.Log(c.name);
        if (c.name.StartsWith("food"))
        {
            eat = true;
            EatFood(c.gameObject);
        }
        else if (c.name.StartsWith("border"))
        {
            dead = true;
            Debug.Log("Boom");
            Renderer renderer; 

            foreach(GameObject t in BodyParts)
            {
                // Spawn Particles
                GameObject p = (GameObject)Instantiate(tailburst, t.transform.position, Quaternion.identity);
                               
                // Hide bodypart 
                renderer = t.gameObject.GetComponent<Renderer>();
                if (t != null) renderer.enabled = false;
            }

            StartCoroutine(DeathOnTimer());
            
        } else if (c.name.StartsWith("tail"))
        {
            //Debug.Log("S - Ouch");
        }


    }
    private IEnumerator DeathOnTimer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            CleanUpFood();
            CleanUpBodyParts();
            gameManager.PlayerDied();
            gameManager.placement.Show();
            Destroy(gameObject);
        }
    }

    private void EatFood(GameObject f)
    {
        foreach (GameObject food in foodList)
        {
            if (food == f)
            {
                foodList.Remove(food);
                Destroy(f);
                Debug.Log("S - Munch");
                AddBodyPart();
                speed += acceleration; 
                break;
            }
        }

        if (foodList.Count == 0)
        {
            gameManager.LevelUp();
            SpawnFood();

        }
        playerManager.SetFoodRemaining(foodList.Count);


    }
    public void SpawnFood()
    {
        int n = gameManager.levels[gameManager.currentLevel-1].food;

        for (int i = 0; i < n; i++)
        {
            float x = (float)Random.Range(lBorder.position.x+1, rBorder.position.x-1);
            float y = (float)Random.Range(bBorder.position.y+1, tBorder.position.y-1);
            GameObject f = Instantiate(food, new Vector3(x, y, -1), Quaternion.identity);
            foodList.Add(f);
        }
        gameManager.foodLevelOveride = 0; 
        playerManager.SetFoodRemaining(foodList.Count);
    }

    public void CleanUpFood()
    {
        foreach(GameObject food in foodList)
        {
            Destroy(food);
            playerManager.SetFoodRemaining(foodList.Count);
        }
    }
    public void CleanUpBodyParts()
    {
        foreach (GameObject part in BodyParts)
        {
            Destroy(part);           
        }
    }

}
