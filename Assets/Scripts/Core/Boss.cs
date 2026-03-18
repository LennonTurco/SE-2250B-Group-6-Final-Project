using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField] private Character recruitedCharacter;

    public Character GetRecruitedCharacter()
    {
        return recruitedCharacter;
    }
}

