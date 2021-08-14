using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoverController : MonoBehaviour
{
    [Header ("Rover Attributes")]
    public float moveSpeed = 4f;
    public float minDist = 20;
    public float pointerVelocity = 0.5f;
    public int lifeUpPerGear = 15;
    public int maxHealth = 100;
    public int currentHealth;
    public HealthController healthBar;
    
 
    [Header ("Achievements")]
    public Text WaterText;
    public Text OrganicMatterText;
    public Text MineralsText;
    public int TotalResources = 15;
    [Space(10)]
    public bool isCrouched;

    public GameObject terrain;
    public float borderLimitPerc = 0.80f;
    private Rigidbody rb;
    private GameObject[] rocks;

    int waterR;
    int organicMatterR;
    int mineralsR;

    public GameObject youWin;
    public GameObject youLose;
    public GameObject mainTemplate;

    private Vector3 originalHeight, crouchedHeight;
    private Vector3 forward, right, heading;
    private int pointerDirection = -1;
    private bool taskSuccess = false, isTaskActive = false;
    private float dist = 0;
    private GameObject precisionTaskLeft;
    private GameObject precisionTaskTarget;
    private GameObject precisionTaskRight;
    private GameObject precisionPointer;
    private GameObject precisionPointerAnchor;
    private Transform precisionPointerTransform;
    private Vector3 initialPointerPosition;
    private bool isGameOver = false;

    private List<ResourcesModel> resourcesBag;
    private int water = 0;
    private int organicMatter = 0;
    private int minerals = 0;

    private AudioManager audioManager;

    private void Awake()
    {
        youLose.SetActive(false);
        youWin.SetActive(false);

        audioManager = FindObjectOfType<AudioManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        healthBar.SetHealth(currentHealth);

        resourcesBag = new List<ResourcesModel>();

        precisionTaskLeft = GameObject.Find("PrecisionTaskLeft");
        precisionTaskTarget = GameObject.Find("PrecisionTaskRight");
        precisionTaskRight = GameObject.Find("PrecisionTaskTarget");
        precisionPointer = GameObject.Find("PrecisionPointer");
        precisionPointerAnchor = GameObject.Find("PrecisionPointerAnchor");
        precisionPointerTransform = precisionPointer.transform;

        initialPointerPosition = new Vector3 (
                                                precisionPointerTransform.position.x, 
                                                precisionPointerTransform.position.y, 
                                                precisionPointerTransform.position.z
                                            );

        ShowTaskComponent(false);
        precisionPointerAnchor.SetActive(false);

        rb = GetComponent<Rigidbody>();

        isCrouched = false;

        originalHeight = transform.localScale;
        crouchedHeight = new Vector3 (transform.localScale.x, transform.localScale.y / 2, transform.localScale.z);

        forward = Camera.main.transform.forward;
        forward.y = 0; // planify the move direction
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward; // transposes the horizontal direction to be 90 degrees side from the vertical

        SetRandomResources(TotalResources);
        UpdateScreenResources();
    }

    private void Update()
    {
        if (!isGameOver)
        {
            HandleGameOver();
        }
    }

    private void FixedUpdate()
    {
        if (!isGameOver)
        {
            Control();
            HandlePos();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "Life")
        {
            audioManager.Play("HealthUpAudio");
            currentHealth += lifeUpPerGear;
            healthBar.SetHealth(currentHealth);
            Destroy(other.gameObject);
        }
    }

    void HandleGameOver ()
    {
        if (currentHealth <= 0)
        {
            mainTemplate.SetActive(false);
            youLose.SetActive(true);
            isGameOver = true;
        }

        if (waterR <= water && organicMatterR <= organicMatter && mineralsR <= minerals)
        {
            mainTemplate.SetActive(false);
            youWin.SetActive(true);
            isGameOver = true;
        }
    }

    void HandlePos()
    {

        if (transform.position.x <= borderLimitPerc * terrain.transform.position.x 
         || transform.position.x > -borderLimitPerc * terrain.transform.position.x)
        {
            transform.position = new Vector3(-transform.position.x, transform.position.y, transform.position.z);
        }
        if (transform.position.z <= borderLimitPerc * terrain.transform.position.z 
         || transform.position.z > -borderLimitPerc * terrain.transform.position.z)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -transform.position.z);
        }

    }

    void Control()
    {
        
        // movement control
        if (!isCrouched && (Input.GetAxis("HorizontalKey") != 0 || Input.GetAxis("VerticalKey") != 0))
        {
            Vector3 rightMovement2 = Input.GetAxis("HorizontalKey") * right;  // horizontal movement
            Vector3 upMovement2 = Input.GetAxis("VerticalKey") * forward;      // vertical movement

            heading = Vector3.Normalize(rightMovement2 + upMovement2); // direction used to turn around

            transform.forward = heading /** Time.deltaTime*/; // turn around
            rb.velocity = moveSpeed * heading;
            initialPointerPosition += initialPointerPosition + (moveSpeed * heading);
        }
        else if(Input.GetAxis("HorizontalKey") == 0 && Input.GetAxis("VerticalKey") == 0)
        {
            rb.velocity = Vector3.zero;
        }

        // Interaction control
        if (Input.GetAxis("InteractionKey") != 0)
        {
            ShowTaskComponent(true);
            isTaskActive = true;

            precisionPointerTransform.position += pointerVelocity * pointerDirection * (Quaternion.Euler(0, 90, 0) * forward);
            dist = Vector3.Distance(
                                precisionPointerTransform.position, 
                                precisionPointerAnchor.transform.position
                           );
            if (dist > 3.5)
            {
                pointerDirection = -pointerDirection;
                // Line to prevent border bug
                precisionPointerTransform.position = precisionPointerAnchor.transform.position + (Quaternion.Euler(0, -90 * pointerDirection, 0) * forward) * 3.5f;
            }
   
        }
        else if (Input.GetAxis("InteractionKey") == 0 && isTaskActive) // when space is not pressed anymore
        {
            DelayShowTaskComponent(0.75f, false);

            taskSuccess = dist <= 0.5f;

            rocks = GameObject.FindGameObjectsWithTag("Rock");
            if (rocks.Length > 0 && taskSuccess)
            {
                GetClosestEnemyAndDestroy(rocks);
                Debug.Log("Rocha quebrada");
            }
            else if (!taskSuccess)
            {
                Debug.Log("Pontuação nao atingida");
            }
            else
            {
                Debug.Log("Todas as rochas foram destruidas");
            }
        }

        // Crouch control
        if (Input.GetAxis("AditionalKey") != 0)
        {
            if (transform.localScale.y > crouchedHeight.y + 0.05)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, crouchedHeight, 5 * Time.deltaTime);
                isCrouched = true;

                Debug.Log("Crouch");
            }
        }
        else if (Input.GetAxis("AditionalKey") == 0)
        {
            if (transform.localScale.y < originalHeight.y - 0.05)
            {
                isCrouched = false;
                transform.localScale = Vector3.Lerp(this.transform.localScale, originalHeight, 5 * Time.deltaTime);

                Debug.Log("Stand up");
            }
        }
        
    }

    void GetClosestEnemyAndDestroy(GameObject[] enemies)
    {
        GameObject nearestEnemy = null;
        Vector3 currentPos = transform.position;
        float nearestDist = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float dist = Vector3.Distance(enemy.transform.position, currentPos);
            if (dist < minDist && dist < nearestDist)
            {
                nearestEnemy = enemy;
                nearestDist = dist;
            }
        }

        RockController rockController = nearestEnemy.GetComponent<RockController>();

        CollectResources(rockController.resources);

        audioManager.Play("RockSmashAudio");

        Destroy(nearestEnemy);
    }

    void ShowTaskComponent(bool status)
    {
        precisionTaskLeft.SetActive(status);
        precisionTaskTarget.SetActive(status);
        precisionTaskRight.SetActive(status);
        precisionPointer.SetActive(status);
    }

    void DelayShowTaskComponent(float delayTime, bool status)
    {
        StartCoroutine(DelayAction(delayTime, status));
        isTaskActive = status;
    }

    IEnumerator DelayAction(float delayTime, bool status)
    {
        //Wait for the specified delay time before continuing.
        yield return new WaitForSeconds(delayTime);

        //Do the action after the delay time has finished.
        ShowTaskComponent(status);
    }

    void CollectResources(List<ResourcesModel> resources)
    {
        resourcesBag.AddRange(resources);

        UpdateScreenResources();
    }

    void UpdateScreenResources()
    {
        water = 0;
        organicMatter = 0;
        minerals = 0;

        foreach(var resource in resourcesBag)
        {
            if (resource.idResource == 0) water++;
            else if (resource.idResource == 1) organicMatter++;
            else minerals++;
        }

        WaterText.text = $"Water {water}/{waterR}";
        OrganicMatterText.text = $"Organic Matter {organicMatter}/{organicMatterR}";
        MineralsText.text = $"Minerals {minerals}/{mineralsR}";
    }

    void SetRandomResources(int totalResources)
    {
        waterR = (int) Random.Range(1, totalResources - 2);
        organicMatterR = (int) Random.Range(1, totalResources - waterR - 1);
        mineralsR = (int) totalResources - waterR - organicMatterR;
    }
    
    public void TakeDamage(int dmg)
    {
        currentHealth = Mathf.Clamp(currentHealth - dmg, 0, maxHealth);
        healthBar.SetHealth(currentHealth);
    }
}
