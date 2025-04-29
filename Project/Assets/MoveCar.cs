using UnityEngine;

public class MoveCar : MonoBehaviour
{
    [SerializeField]
    int speed = 1;
    [SerializeField]
    GameObject backwall;
    [SerializeField]
    GameObject prefab;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);   
    }


    void OnTriggerEnter(Collider other)
    {
        Debug.Log("hit something!!");
        if (other.CompareTag("Backwall"))
        {
            Destroy(gameObject);

            GameObject newCar = Instantiate(prefab, new Vector3(10, 1, 0), Quaternion.identity);

            // Copy the speed from the current object
            MoveCar moveScript = newCar.GetComponent<MoveCar>();
            if (moveScript != null)
            {
                moveScript.SetSpeed(speed); // We’ll add SetSpeed() below
            }
        }
    }

    public void SetSpeed(int newSpeed)
    {
        speed = newSpeed;
    }

}
