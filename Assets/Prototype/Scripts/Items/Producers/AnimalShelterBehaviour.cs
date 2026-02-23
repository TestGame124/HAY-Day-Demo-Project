using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalShelterBehaviour : ProducerBase
{

    private List<AnimalBehaviour> animals;

    private int totalAnimals = 0;


    public override void Gather()
    {

    }

    public override void OnTap()
    {

    }

    protected override IEnumerator InProcess()
    {
        yield return null;
    }
}
