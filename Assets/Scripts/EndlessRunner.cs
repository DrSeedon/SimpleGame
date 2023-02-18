using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;

public class EndlessRunner : MonoBehaviour
{
    public float speed = 10f; // speed of the character
    public float moveSpeed = 1f;
    public Transform platform; // platform for the character to run on
    public LayerMask obstacleLayer; // layer mask for obstacles
    private Vector3 leftBound, rightBound, middleBound;
    private bool facingRight = true; // direction the character is facing
    
    public GameObject platformPrefab;
    public GameObject obstaclePrefab;
    public float platformLength;
    public float platformSpacing;
    public float obstacleSpacing;

    private float platformStartPoint;
    private float obstacleStartPoint;
    private Queue<GameObject> platforms = new Queue<GameObject>();
    private Queue<GameObject> obstacles = new Queue<GameObject>();
    private int maxPlatforms = 10;
    public int score = 0;
    
    public TMP_Text scoreText;
    
    
    public UnityAdsManager unityAdsManager;

    void Start()
    {
        unityAdsManager.ToggleBanner();
        unityAdsManager.LoadNonRewardedAd();
        unityAdsManager.ShowNonRewardedAd();
        
        
        // set left and right bounds of the platform
        leftBound = platform.GetChild(0).position;
        rightBound = platform.GetChild(1).position;
        middleBound = platform.GetChild(2).position;
        
        
        platformStartPoint = platformPrefab.transform.position.z;
        obstacleStartPoint = obstaclePrefab.transform.position.z;
        
        // Create a set of platforms and obstacles ahead of time
        for (int i = 0; i < 5; i++)
        {
            CreatePlatform();
            CreateObstacle(false);
        }

        StartCoroutine(Timer());
    }

    void Update()
    {
        // move the character forward
        transform.position += Vector3.forward * speed * Time.deltaTime;

        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0)
        {
            MoveRight();
        }
        else if (horizontal < 0)
        {
            MoveLeft();
        }

        // check for collisions with obstacles
        if (IsCollidingWithObstacle())
        {
            // restart the level if there is a collision
            RestartLevel();
        }
        
        // Check if the front platform has moved out of range and remove it
        while (platforms.Count > 0 && platforms.Peek().transform.position.z < transform.position.z - platformLength)
        {
            GameObject platform = platforms.Dequeue();
            Destroy(platform);
        }
        
        // Check if the front obstacle has moved out of range and remove it
        while (obstacles.Count > 0 && obstacles.Peek().transform.position.z < transform.position.z - obstacleSpacing)
        {
            GameObject obstacle = obstacles.Dequeue();
            Destroy(obstacle);
        }


        var posZ = platforms.Peek().transform.position.z;
        // If the front platform has moved far enough ahead, create a new one
        if (platforms.Count == 0 || posZ < transform.position.z + platformSpacing)
        {
            CreatePlatform();
        }

        posZ = obstacles.Peek().transform.position.z;
        // If the front obstacle has moved far enough ahead, create a new one
        if (obstacles.Count == 0 || posZ < transform.position.z + obstacleSpacing)
        {
            CreateObstacle(true);
        }
    }
    
    public void MoveRight()
    {
        if (transform.position.x < rightBound.x)
        {
            transform.position = new Vector3(transform.position.x + moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }

    public void MoveLeft()
    {
        if (transform.position.x > leftBound.x)
        {
            transform.position = new Vector3(transform.position.x - moveSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        }
    }
    
    void CreatePlatform()
    {
        if(platforms.Count>maxPlatforms)return;
        platformStartPoint += platformLength + platformSpacing;
        GameObject newPlatform = Instantiate(platformPrefab, new Vector3(0, 0, platformStartPoint), Quaternion.identity);
        platforms.Enqueue(newPlatform);
    }

    void CreateObstacle(bool isMid)
    {
        if(obstacles.Count>maxPlatforms)return;
        float xPos = 0;
        switch (Random.Range(0, 3))
        {
            case 0:
                xPos = leftBound.x;
                break;
            case 1:
                xPos = rightBound.x;
                break;
            case 2:
                if(isMid)
                    xPos = middleBound.x;
                else
                    xPos = Random.Range(0, 2) != 0? leftBound.x : rightBound.x;
                break;
        }
        obstacleStartPoint += obstacleSpacing;
        GameObject newObstacle = Instantiate(obstaclePrefab, new Vector3(xPos, 0.25f, obstacleStartPoint),obstaclePrefab.transform.rotation);
        obstacles.Enqueue(newObstacle);
    }

    private bool IsCollidingWithObstacle()
    {
        // check for collisions with obstacles
        Collider[] hit = Physics.OverlapSphere(transform.position, 0.5f, obstacleLayer);
        return hit.Length > 0;
    }

    private void RestartLevel()
    {
        
        unityAdsManager.OnUnityAdsShowComplete(null, UnityAdsShowCompletionState.SKIPPED);
        SceneManager.LoadScene(0);
    }

    IEnumerator Timer()
    {
        while (true)
        {
            score++;
            scoreText.text = "Score:" + score;
            speed += 0.1f;
            yield return new WaitForSeconds(2f);
        }
    }
}
