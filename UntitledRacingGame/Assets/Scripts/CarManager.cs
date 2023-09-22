using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour
{
    [SerializeField] GameObject[] cars;

    private void Awake()
    {
        int selectedCar = Profile.instance.GetCarIndex();

        for (int i = 0; i < cars.Length; i++)
        {
            if (i != selectedCar) cars[i].SetActive(false);
            else cars[i].SetActive(true);
        }
    }
}
