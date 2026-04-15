using UnityEngine;

public class BurnedSteak : MonoBehaviour
{
    private void Awake()
    {
        if (!TryGetComponent(out Renderer rend))
            return;

        Material mat = rend.material;
        var burnColor = new Color(0.08f, 0.04f, 0.02f);

        if (mat.HasProperty("_BaseColor"))
            mat.SetColor("_BaseColor", burnColor);
        else if (mat.HasProperty("_Color"))
            mat.SetColor("_Color", burnColor);
    }
}
