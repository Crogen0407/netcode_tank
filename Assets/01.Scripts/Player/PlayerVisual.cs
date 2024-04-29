using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] sprites;

    public void SetTintColor(Color color)
    {
        //sprites.ToList().ForEach(sr => sr.color = color);

        foreach(SpriteRenderer sr in sprites)
        {
            sr.color = color;
        }
    }
}
